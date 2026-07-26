/**
 * Google Apps Script for AlenkaAssistant
 * 
 * DEPLOYMENT INSTRUCTIONS:
 * 1. Create a new Google Apps Script project at https://script.google.com
 * 2. Copy this entire code into the Script Editor
 * 3. Set SPREADSHEET_ID to your Google Sheet ID (from the URL)
 * 4. Deploy as a web app: Deploy > New Deployment > Web app
 *    - Execute as: Your Google account
 *    - Who has access: Anyone
 * 5. Copy the deployment URL and add it to GoogleSheetsConfig.json as "deploymentUrl"
 */

// Configuration - Set this to your Google Sheet ID
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";

/**
 * Handle POST requests for data submission
 */
function doPost(e) {
  try {
	Logger.log("=== doPost START ===");
	Logger.log("SPREADSHEET_ID: " + SPREADSHEET_ID);
	Logger.log("e object exists: " + (e !== null && e !== undefined));

	if (!e || !e.postData || !e.postData.contents) {
	  Logger.log("ERROR: No POST data received");
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "No POST data received" }))
		.setMimeType(ContentService.MimeType.JSON);
	}

	Logger.log("POST data received, parsing...");
	const payload = JSON.parse(e.postData.contents);
	const sheetName = payload.sheetName || "Sheet1";
	const appendColumn = payload.appendColumn !== undefined ? payload.appendColumn : null;

	Logger.log("Sheet Name: " + sheetName);
	Logger.log("Append Column: " + appendColumn);
	Logger.log("Values count: " + (payload.values ? payload.values.length : "undefined"));

	if (!payload.values || !Array.isArray(payload.values) || payload.values.length === 0) {
	  Logger.log("ERROR: No values to append");
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "No values to append" }))
		.setMimeType(ContentService.MimeType.JSON);
	}

	Logger.log("Calling appendToSheet...");
	const result = appendToSheet(sheetName, payload.values, appendColumn);
	Logger.log("appendToSheet completed successfully");

	Logger.log("=== doPost SUCCESS ===");
	return ContentService
	  .createTextOutput(JSON.stringify({ success: true, message: "Data appended successfully", result: result }))
	  .setMimeType(ContentService.MimeType.JSON);
  } catch (error) {
	Logger.log("=== doPost ERROR ===");
	Logger.log("Error type: " + error.name);
	Logger.log("Error message: " + error.toString());
	Logger.log("Stack: " + error.stack);

	return ContentService
	  .createTextOutput(JSON.stringify({ success: false, error: error.toString(), errorName: error.name }))
	  .setMimeType(ContentService.MimeType.JSON);
  }
}

/**
 * Handle GET requests for patient lookup
 */
function doGet(e) {
  try {
	const action = e.parameter.action || "lookup";
	const sheetName = e.parameter.sheetName || "NoRM";
	const searchColumn = parseInt(e.parameter.searchColumn || 0);
	const searchValue = (e.parameter.searchValue || "").trim();
	const resultColumn = parseInt(e.parameter.resultColumn || 1);

	if (action === "lookup") {
	  const result = lookupPatientData(sheetName, searchColumn, searchValue, resultColumn);
	  return ContentService
		.createTextOutput(JSON.stringify(result))
		.setMimeType(ContentService.MimeType.JSON);
	}

	return ContentService
	  .createTextOutput(JSON.stringify({ success: false, error: "Unknown action" }))
	  .setMimeType(ContentService.MimeType.JSON);
  } catch (error) {
	Logger.log("Error in doGet: " + error.toString());
	return ContentService
	  .createTextOutput(JSON.stringify({ success: false, error: error.toString() }))
	  .setMimeType(ContentService.MimeType.JSON);
  }
}

/**
 * Lookup patient data from a sheet
 */
function lookupPatientData(sheetName, searchColumn, searchValue, resultColumn) {
  try {
	const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
	const sheet = ss.getSheetByName(sheetName);

	if (!sheet) {
	  return { success: false, error: "Sheet not found: " + sheetName };
	}

	if (!searchValue || searchValue.length === 0) {
	  return { success: false, error: "Search value is empty" };
	}

	const columnLetter = String.fromCharCode(65 + searchColumn);
	const columnData = sheet.getRange(columnLetter + ":" + columnLetter).getValues();

	// Search for matching RM with zero-padding support
	for (let i = 0; i < columnData.length; i++) {
	  const cellValue = String(columnData[i][0]).trim();

	  // Match various formats: "A.0032", "A.32", "0032", "32"
	  const isMatch = cellValue === searchValue ||
					  cellValue === "A." + searchValue ||
					  cellValue.endsWith("." + searchValue) ||
					  cellValue === searchValue.replace(/^0+/, '') ||
					  cellValue === "A." + searchValue.replace(/^0+/, '');

	  if (isMatch) {
		const resultColumnLetter = String.fromCharCode(65 + resultColumn);
		const patientName = sheet.getRange(resultColumnLetter + (i + 1)).getValue();

		return {
		  success: true,
		  found: true,
		  patientName: String(patientName).trim(),
		  rmValue: cellValue,
		  rowNumber: i + 1
		};
	  }
	}

	return {
	  success: true,
	  found: false,
	  error: "Patient not found"
	};
  } catch (error) {
	Logger.log("Error in lookupPatientData: " + error.toString());
	return { success: false, error: error.toString() };
  }
}

/**
 * Append data to a specific sheet
 */
function appendToSheet(sheetName, values, appendColumn) {
  try {
	Logger.log("=== appendToSheet START ===");
	Logger.log("Sheet Name: " + sheetName);
	Logger.log("Values: " + values.length + " rows");
	Logger.log("Append Column: " + appendColumn);

	Logger.log("Opening spreadsheet...");
	const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
	Logger.log("Spreadsheet opened successfully");

	Logger.log("Getting sheet: " + sheetName);
	const sheet = ss.getSheetByName(sheetName);

	if (!sheet) {
	  Logger.log("ERROR: Sheet not found - " + sheetName);
	  throw new Error("Sheet not found: " + sheetName);
	}
	Logger.log("Sheet found successfully");

	if (!values || values.length === 0) {
	  Logger.log("ERROR: No values to append");
	  throw new Error("No values to append");
	}

	let startRow;
	Logger.log("Determining start row...");

	if (appendColumn !== null && appendColumn !== undefined) {
	  Logger.log("Using appendColumn logic for column: " + appendColumn);
	  const columnLetter = String.fromCharCode(65 + appendColumn);
	  Logger.log("Column letter: " + columnLetter);

	  const columnData = sheet.getRange(columnLetter + ":" + columnLetter).getValues();
	  Logger.log("Column data retrieved, length: " + columnData.length);

	  startRow = null;
	  for (let i = 0; i < columnData.length; i++) {
		const cellValue = columnData[i][0];
		if (cellValue === "" || cellValue === null || cellValue === undefined) {
		  startRow = i + 1;
		  Logger.log("Found first empty cell at row: " + startRow);
		  break;
		}
	  }

	  if (startRow === null) {
		const lastRow = sheet.getLastRow();
		startRow = lastRow + 1;
		Logger.log("No empty cells found, using last row + 1: " + startRow);
	  }
	} else {
	  const lastRow = sheet.getLastRow();
	  startRow = lastRow + 1;
	  Logger.log("No appendColumn specified, using last row + 1: " + startRow);
	}

	Logger.log("Final start row: " + startRow);
	const numCols = values[0].length;
	Logger.log("Number of columns: " + numCols);
	Logger.log("Setting values...");

	const range = sheet.getRange(startRow, 1, values.length, numCols);
	range.setValues(values);

	Logger.log("Values set successfully");
	Logger.log("=== appendToSheet SUCCESS ===");

	return {
	  appendedRows: values.length,
	  startRow: startRow,
	  sheetName: sheetName,
	  columnsAppended: numCols
	};
  } catch (error) {
	Logger.log("=== appendToSheet ERROR ===");
	Logger.log("Error type: " + error.name);
	Logger.log("Error message: " + error.toString());
	Logger.log("Stack: " + error.stack);
	throw error;
  }
}

/**
 * Test function
 */
function test() {
  try {
	const testData = {
	  sheetName: "2026",
	  values: [
		["2024", "JANUARI", "15", "100000", "50000", "0", "RM001", "Test Treatment", "Rawat Jalan", "Dr. Smith"]
	  ]
	};

	const result = appendToSheet(testData.sheetName, testData.values, 1);
	Logger.log("Test successful: " + JSON.stringify(result));
  } catch (error) {
	Logger.log("Test failed: " + error.toString());
  }
}
