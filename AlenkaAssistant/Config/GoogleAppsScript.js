/**
 * Google Apps Script for AlenkaAssistant
 * 
 * DEPLOYMENT INSTRUCTIONS:
 * 1. Create a new Google Apps Script project at https://script.google.com
 * 2. Copy this entire code into the Script Editor
 * 3. Add your spreadsheet IDs to ALLOWED_SPREADSHEET_IDS below
 * 4. Deploy as a web app: Deploy > New Deployment > Web app
 *    - Execute as: Your Google account
 *    - Who has access: Anyone
 * 5. Copy the deployment URL and add it to GoogleSheetsConfig.json as "deploymentUrl"
 * 
 * MULTI-SPREADSHEET SUPPORT:
 * This script supports multiple spreadsheets through dynamic parameters:
 * - POST requests: Include "spreadsheetId" in JSON payload
 * - GET requests: Include "spreadsheetId" as query parameter
 * - Sheet names are passed dynamically as "sheetName" parameter
 * - If spreadsheetId is not provided, falls back to SPREADSHEET_ID below
 * 
 * SECURITY:
 * - Only spreadsheets in ALLOWED_SPREADSHEET_IDS can be accessed
 * - Unauthorized IDs are rejected with an error response
 * - To add new spreadsheets, just add their ID to ALLOWED_SPREADSHEET_IDS
 * 
 * CONFIGURATION IN GoogleSheetsConfig.json:
 * - "spreadsheetId": Main spreadsheet for purchase requests
 * - "noRmSpreadsheetId": Separate spreadsheet for NoRM patient data (optional)
 * - "noRmSheetName": Sheet name in NoRM spreadsheet (default "NoRM")
 * - "sheetName": Sheet name in main spreadsheet (default "2026")
 */

// SECURITY: Whitelist of allowed spreadsheet IDs
// To add a new spreadsheet, simply add its ID to this array
const ALLOWED_SPREADSHEET_IDS = [
  "YOUR_SPREADSHEET_ID_1",  // Main purchase sheet
  "YOUR_SPREADSHEET_ID_2"  // NoRM patient sheet
];

// Configuration - Fallback spreadsheet ID (must be in ALLOWED_SPREADSHEET_IDS)
const SPREADSHEET_ID = ALLOWED_SPREADSHEET_IDS[0];

/**
 * Validate that a spreadsheet ID is in the allowed list
 * @param {string} spreadsheetId - The spreadsheet ID to validate
 * @returns {boolean} True if allowed, false otherwise
 */
function isAllowedSpreadsheet(spreadsheetId) {
  const idToCheck = spreadsheetId || SPREADSHEET_ID;
  return ALLOWED_SPREADSHEET_IDS.includes(idToCheck);
}

/**
 * Handle POST requests for data submission and patient addition
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
	const action = payload.action || "append";
	const spreadsheetId = payload.spreadsheetId || SPREADSHEET_ID;

	Logger.log("Action: " + action);
	Logger.log("Using spreadsheetId: " + spreadsheetId);

	// SECURITY: Validate spreadsheet ID against whitelist
	if (!isAllowedSpreadsheet(spreadsheetId)) {
	  Logger.log("SECURITY ERROR: Spreadsheet ID not in allowed list: " + spreadsheetId);
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "Spreadsheet ID not authorized" }))
		.setMimeType(ContentService.MimeType.JSON);
	}

	// Handle patient addition
	if (action === "addPatient") {
	  Logger.log("Processing addPatient action");
	  const sheetName = payload.sheetName || "NoRM";
	  const rmNumber = payload.rmNumber || "";
	  const patientName = payload.patientName || "";
	  const rmColumn = payload.rmColumn !== undefined ? payload.rmColumn : 0;
	  const patientNameColumn = payload.patientNameColumn !== undefined ? payload.patientNameColumn : 1;

	  try {
		const result = addPatientToSheet(sheetName, rmNumber, patientName, rmColumn, patientNameColumn, spreadsheetId);
		Logger.log("=== doPost addPatient SUCCESS ===");
		return ContentService
		  .createTextOutput(JSON.stringify(result))
		  .setMimeType(ContentService.MimeType.JSON);
	  } catch (addPatientError) {
		Logger.log("ERROR in addPatient: " + addPatientError.toString());
		return ContentService
		  .createTextOutput(JSON.stringify({ success: false, error: addPatientError.toString() }))
		  .setMimeType(ContentService.MimeType.JSON);
	  }
	}

	// Handle regular data appending
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

	// Sanitize values: ensure all cells are strings and safe to append
	const sanitizedValues = payload.values.map(row => {
	  return row.map(cell => {
		if (cell === null || cell === undefined) {
		  return "";
		}
		return String(cell);
	  });
	});

	Logger.log("Values sanitized. First row: " + JSON.stringify(sanitizedValues[0]));

	const result = appendToSheet(sheetName, sanitizedValues, appendColumn, spreadsheetId);
	Logger.log("appendToSheet completed successfully");

	Logger.log("=== doPost SUCCESS ===");
	try {
	  const responseObj = {
		success: true,
		message: "Data appended successfully",
		appendedRows: result.appendedRows || 0,
		startRow: result.startRow || 0,
		sheetName: result.sheetName || sheetName
	  };
	  return ContentService
		.createTextOutput(JSON.stringify(responseObj))
		.setMimeType(ContentService.MimeType.JSON);
	} catch (responseError) {
	  Logger.log("ERROR creating response: " + responseError.toString());
	  return ContentService
		.createTextOutput(JSON.stringify({ success: true, message: "Data appended but error creating response" }))
		.setMimeType(ContentService.MimeType.JSON);
	}
  } catch (error) {
	Logger.log("=== doPost ERROR ===");
	Logger.log("Error type: " + error.name);
	Logger.log("Error message: " + error.toString());
	Logger.log("Stack: " + error.stack);

	try {
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: error.toString(), errorName: error.name }))
		.setMimeType(ContentService.MimeType.JSON);
	} catch (responseError) {
	  Logger.log("ERROR creating error response: " + responseError.toString());
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "An error occurred" }))
		.setMimeType(ContentService.MimeType.JSON);
	}
  }
}

/**
 * Handle GET requests for patient lookup and RM retrieval
 */
function doGet(e) {
  try {
	const action = e.parameter.action || "lookup";
	const sheetName = e.parameter.sheetName || "NoRM";
	const searchColumn = parseInt(e.parameter.searchColumn || 0);
	const rmColumn = parseInt(e.parameter.rmColumn || searchColumn);  // rmColumn defaults to searchColumn
	const searchValue = (e.parameter.searchValue || "").trim();
	const resultColumn = parseInt(e.parameter.resultColumn || 1);
	const rm = (e.parameter.rm || "").trim();
	const patientName = (e.parameter.patientName || "").trim();
	const patientNameColumn = parseInt(e.parameter.patientNameColumn || 1);
	const spreadsheetId = e.parameter.spreadsheetId || SPREADSHEET_ID;

	Logger.log("doGet - Action: " + action + ", SpreadsheetId: " + spreadsheetId);

	// SECURITY: Validate spreadsheet ID against whitelist
	if (!isAllowedSpreadsheet(spreadsheetId)) {
	  Logger.log("SECURITY ERROR: Spreadsheet ID not in allowed list: " + spreadsheetId);
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "Spreadsheet ID not authorized" }))
		.setMimeType(ContentService.MimeType.JSON);
	}

	if (action === "lookup") {
	  try {
		const result = lookupPatientData(sheetName, searchColumn, searchValue, resultColumn, spreadsheetId);
		return ContentService
		  .createTextOutput(JSON.stringify(result))
		  .setMimeType(ContentService.MimeType.JSON);
	  } catch (lookupError) {
		Logger.log("ERROR in lookup: " + lookupError.toString());
		return ContentService
		  .createTextOutput(JSON.stringify({ success: false, error: lookupError.toString() }))
		  .setMimeType(ContentService.MimeType.JSON);
	  }
	}

	if (action === "getLastRm") {
	  try {
		const result = getLastRmFromSheet(sheetName, rmColumn, patientNameColumn, spreadsheetId);
		return ContentService
		  .createTextOutput(JSON.stringify(result))
		  .setMimeType(ContentService.MimeType.JSON);
	  } catch (rmError) {
		Logger.log("ERROR in getLastRm: " + rmError.toString());
		return ContentService
		  .createTextOutput(JSON.stringify({ success: false, error: rmError.toString() }))
		  .setMimeType(ContentService.MimeType.JSON);
	  }
	}

	if (action === "addPatient") {
	  try {
		const result = addPatientToSheet(sheetName, rm, patientName, rmColumn, patientNameColumn, spreadsheetId);
		return ContentService
		  .createTextOutput(JSON.stringify(result))
		  .setMimeType(ContentService.MimeType.JSON);
	  } catch (addPatientError) {
		Logger.log("ERROR in addPatient (doGet): " + addPatientError.toString());
		return ContentService
		  .createTextOutput(JSON.stringify({ success: false, error: addPatientError.toString() }))
		  .setMimeType(ContentService.MimeType.JSON);
	  }
	}

	return ContentService
	  .createTextOutput(JSON.stringify({ success: false, error: "Unknown action" }))
	  .setMimeType(ContentService.MimeType.JSON);
  } catch (error) {
	Logger.log("Error in doGet: " + error.toString());
	try {
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: error.toString() }))
		.setMimeType(ContentService.MimeType.JSON);
	} catch (responseError) {
	  Logger.log("ERROR creating error response in doGet: " + responseError.toString());
	  return ContentService
		.createTextOutput(JSON.stringify({ success: false, error: "An error occurred" }))
		.setMimeType(ContentService.MimeType.JSON);
	}
  }
}

/**
 * Lookup patient data from a sheet
 */
function lookupPatientData(sheetName, searchColumn, searchValue, resultColumn, spreadsheetId) {
  try {
	const ssId = spreadsheetId || SPREADSHEET_ID;
	// SECURITY: Validate spreadsheet ID before accessing
	if (!isAllowedSpreadsheet(ssId)) {
	  return { success: false, error: "Spreadsheet ID not authorized" };
	}
	const ss = SpreadsheetApp.openById(ssId);
	const sheet = ss.getSheetByName(sheetName);

	if (!sheet) {
	  return { success: false, error: "Sheet not found: " + sheetName };
	}

	if (!searchValue || searchValue.length === 0) {
	  return { success: false, error: "Search value is empty" };
	}

	const lastRow = sheet.getLastRow();
	if (lastRow < 2) {
	  return {
		success: true,
		found: false,
		error: "Patient not found"
	  };
	}

	const columnLetter = String.fromCharCode(65 + searchColumn);
	// Only read data rows, not entire column (much faster!)
	const columnData = sheet.getRange(columnLetter + "1:" + columnLetter + lastRow).getValues();

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
	Logger.log("ERROR in lookupPatientData: " + error.toString());
	return { success: false, error: error.toString() };
  }
}

/**
 * Append data to a specific sheet
 */
function appendToSheet(sheetName, values, appendColumn, spreadsheetId) {
  try {
	Logger.log("=== appendToSheet START ===");
	Logger.log("Sheet Name: " + sheetName);
	Logger.log("Values: " + values.length + " rows");
	Logger.log("Append Column: " + appendColumn);

	Logger.log("Opening spreadsheet...");
	const ssId = spreadsheetId || SPREADSHEET_ID;
	// SECURITY: Validate spreadsheet ID before accessing
	if (!isAllowedSpreadsheet(ssId)) {
	  Logger.log("SECURITY ERROR: Spreadsheet ID not in allowed list: " + ssId);
	  return { success: false, error: "Spreadsheet ID not authorized" };
	}
	const ss = SpreadsheetApp.openById(ssId);
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

	  const lastRow = sheet.getLastRow();
	  Logger.log("Last row in sheet: " + lastRow);

	  // Only fetch data from existing rows, not the entire column
	  const columnData = sheet.getRange(columnLetter + "1:" + columnLetter + lastRow).getValues();
	  Logger.log("Column data retrieved, rows: " + columnData.length);

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

	// Log first row to see what we're trying to set
	Logger.log("First row values: " + JSON.stringify(values[0]));

	try {
	  const range = sheet.getRange(startRow, 1, values.length, numCols);
	  range.setValues(values);
	} catch (setValuesError) {
	  Logger.log("ERROR in setValues: " + setValuesError.toString());
	  Logger.log("Error name: " + setValuesError.name);
	  Logger.log("Error message: " + setValuesError.message);
	  throw new Error("Failed to set values in sheet: " + setValuesError.toString());
	}

	Logger.log("Values set successfully");
	Logger.log("=== appendToSheet SUCCESS ===");

	const safeResult = {
	  appendedRows: parseInt(values.length) || 0,
	  startRow: parseInt(startRow) || 0,
	  sheetName: String(sheetName || ""),
	  columnsAppended: parseInt(numCols) || 0
	};

	Logger.log("Returning result: appendedRows=" + safeResult.appendedRows + ", startRow=" + safeResult.startRow);
	return safeResult;
  } catch (error) {
	Logger.log("=== appendToSheet ERROR ===");
	Logger.log("Error type: " + error.name);
	Logger.log("Error message: " + error.toString());
	Logger.log("Stack: " + error.stack);

	// Try to get more details about what value caused the error
	Logger.log("Number of values: " + (values ? values.length : "undefined"));
	if (values && values.length > 0) {
	  Logger.log("First row length: " + values[0].length);
	  try {
		Logger.log("First row: " + JSON.stringify(values[0]));
	  } catch (logError) {
		Logger.log("Could not stringify first row: " + logError.toString());
	  }
	}

	throw error;
  }
}

/**
 * Get the last RM number from a sheet
 */
function getLastRmFromSheet(sheetName, rmColumn, patientNameColumn, spreadsheetId) {
  try {
    const ssId = spreadsheetId || SPREADSHEET_ID;
    // SECURITY: Validate spreadsheet ID before accessing
    if (!isAllowedSpreadsheet(ssId)) {
      return { success: false, error: "Spreadsheet ID not authorized" };
    }
    const ss = SpreadsheetApp.openById(ssId);
    const sheet = ss.getSheetByName(sheetName);

    if (!sheet) {
      return { success: false, error: "Sheet not found: " + sheetName };
    }

    const columnLetter = String.fromCharCode(65 + rmColumn);
    const lastRow = sheet.getLastRow();

    if (lastRow < 2) {
      return { 
        success: true, 
        lastRm: null,
        nextRm: "A.0001",
        lastRow: lastRow,
        message: "No data in sheet, start with A.0001"
      };
    }

    // Get the RM column values
    const rmRange = sheet.getRange(columnLetter + "2:" + columnLetter + lastRow);
    const rmValues = rmRange.getValues();

    let lastRm = null;
    let lastValidIndex = -1;

    // Scan from top to bottom to find the last continuous data (before empty rows)
    for (let i = 0; i < rmValues.length; i++) {
      const rmValue = String(rmValues[i][0]).trim();

      if (rmValue.length > 0) {
        // Found a non-empty RM, remember it as the last valid one so far
        lastRm = rmValue;
        lastValidIndex = i;
      } else {
        // Found empty cell
        // If we've found valid data before, and now hit empty, stop here
        if (lastValidIndex >= 0) {
          break;
        }
      }
    }

    if (!lastRm) {
      return { 
        success: true, 
        lastRm: null,
        nextRm: "A.0001",
        lastRow: lastRow,
        message: "No RM values found, start with A.0001"
      };
    }

    // Increment the last RM found
    const nextRm = incrementRmNumber(lastRm);

    return {
      success: true,
      lastRm: lastRm,
      nextRm: nextRm,
      lastRow: lastRow
    };
  } catch (error) {
    Logger.log("ERROR in getLastRmFromSheet: " + error.toString());
    return { success: false, error: error.toString() };
  }
}

/**
 * Add a new patient to the NoRM sheet via POST
 */
function addPatientToSheet(sheetName, rmNumber, patientName, rmColumn, patientNameColumn, spreadsheetId) {
  try {
    Logger.log("=== addPatientToSheet START ===");
    Logger.log("Sheet Name: " + sheetName);
    Logger.log("RM Number: " + rmNumber);
    Logger.log("Patient Name: " + patientName);
    Logger.log("RM Column: " + rmColumn);
    Logger.log("Patient Name Column: " + patientNameColumn);

    const ssId = spreadsheetId || SPREADSHEET_ID;
    // SECURITY: Validate spreadsheet ID before accessing
    if (!isAllowedSpreadsheet(ssId)) {
      Logger.log("SECURITY ERROR: Spreadsheet ID not in allowed list: " + ssId);
      return { success: false, error: "Spreadsheet ID not authorized" };
    }
    const ss = SpreadsheetApp.openById(ssId);
    const sheet = ss.getSheetByName(sheetName);

    if (!sheet) {
      Logger.log("ERROR: Sheet not found - " + sheetName);
      return { success: false, error: "Sheet not found: " + sheetName };
    }

    if (!rmNumber || !patientName) {
      Logger.log("ERROR: Missing RM or patient name");
      return { success: false, error: "RM number and patient name are required" };
    }

    // Extract just the number part from RM (e.g., "A.1001" -> "1001")
    let rmNumberToSave = rmNumber;
    if (rmNumber.includes(".")) {
      const parts = rmNumber.split(".");
      rmNumberToSave = parts[parts.length - 1]; // Get the last part after the dot
    }

    Logger.log("RM to save: " + rmNumberToSave);

    // Find the first empty row in the RM column (same logic as appendToSheet)
    Logger.log("Finding first empty row in RM column: " + rmColumn);
    const columnLetter = String.fromCharCode(65 + rmColumn);
    Logger.log("Column letter: " + columnLetter);

    const lastRow = sheet.getLastRow();
    Logger.log("Last row in sheet: " + lastRow);

    // Only fetch data from existing rows, not the entire column
    const columnData = sheet.getRange(columnLetter + "1:" + columnLetter + lastRow).getValues();
    Logger.log("Column data retrieved, rows: " + columnData.length);

    let newRow = null;
    for (let i = 0; i < columnData.length; i++) {
      const cellValue = columnData[i][0];
      if (cellValue === "" || cellValue === null || cellValue === undefined) {
        newRow = i + 1;
        Logger.log("Found first empty cell at row: " + newRow);
        break;
      }
    }

    // If no empty cells found, append to the last row + 1
    if (newRow === null) {
      newRow = lastRow + 1;
      Logger.log("No empty cells found, using last row + 1: " + newRow);
    }

    Logger.log("Adding new patient at row: " + newRow);

    // Create a row with the RM and patient name at the correct columns
    const maxColumns = Math.max(rmColumn, patientNameColumn) + 1;
    const newValues = new Array(maxColumns).fill("");

    newValues[rmColumn] = rmNumberToSave;  // Save only the number part (xxxx)
    newValues[patientNameColumn] = patientName;

    Logger.log("Setting values at row " + newRow + ": RM=" + rmNumber + ", Patient=" + patientName);
    const range = sheet.getRange(newRow, 1, 1, maxColumns);
    range.setValues([newValues]);

    Logger.log("=== addPatientToSheet SUCCESS ===");
    return {
      success: true,
      message: "Patient added successfully",
      newRow: newRow,
      rmNumber: rmNumber,
      patientName: patientName
    };
  } catch (error) {
    Logger.log("=== addPatientToSheet ERROR ===");
    Logger.log("Error: " + error.toString());
    return { success: false, error: error.toString() };
  }
}

/**
 * Increment RM number from format "A.xxxx" to next number
 * Example: "A.0001" ? "A.0002", "A.0009" ? "A.0010"
 */
function incrementRmNumber(rmValue) {
  if (!rmValue) return "A.0001";

  // Extract the number part (remove "A." prefix)
  let numberPart = String(rmValue).trim();

  if (numberPart.startsWith("A.")) {
	numberPart = numberPart.substring(2);
  } else if (numberPart.startsWith("A")) {
	numberPart = numberPart.substring(1);
  }

  // Convert to number, increment, and format back to 4 digits
  let number = parseInt(numberPart) || 0;
  number++;

  // Pad to 4 digits and format
  let padded = String(number).padStart(4, "0");
  return "A." + padded;
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
