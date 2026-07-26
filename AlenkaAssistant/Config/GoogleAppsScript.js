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
 * Do NOT modify - Required for web app deployment
 */
function doPost(e) {
  try {
	const payload = JSON.parse(e.postData.contents);
	const sheetName = payload.sheetName || "Sheet1";
	const appendColumn = payload.appendColumn || null;

	// Log received data
	Logger.log("Received payload: " + JSON.stringify(payload));
	Logger.log("Sheet name: " + sheetName);
	Logger.log("Append column: " + appendColumn);
	Logger.log("Values count: " + payload.values.length);
	if (payload.values.length > 0) {
	  Logger.log("First row values: " + JSON.stringify(payload.values[0]));
	}

	// Append the data to the sheet
	const result = appendToSheet(sheetName, payload.values, appendColumn);

	return ContentService
	  .createTextOutput(JSON.stringify({ success: true, message: "Data appended successfully", result: result }))
	  .setMimeType(ContentService.MimeType.JSON);
  } catch (error) {
	Logger.log("Error in doPost: " + error.toString());
	return ContentService
	  .createTextOutput(JSON.stringify({ success: false, error: error.toString() }))
	  .setMimeType(ContentService.MimeType.JSON);
  }
}

/**
 * Append data to a specific sheet
 * @param {string} sheetName - Name of the sheet to append to
 * @param {Array<Array>} values - 2D array of values to append
 * @param {number} appendColumn - Column to check for last empty row (0-based index). If null, uses getLastRow()
 * @returns {Object} Append operation details
 */
function appendToSheet(sheetName, values, appendColumn) {
  const sheet = SpreadsheetApp.openById(SPREADSHEET_ID).getSheetByName(sheetName);

  if (!sheet) {
	throw new Error(`Sheet "${sheetName}" not found`);
  }

  // Determine the starting row
  let startRow;
  if (appendColumn !== null && appendColumn !== undefined) {
	// Find the first empty row in the specified column (1-based for getRange)
	const columnLetter = String.fromCharCode(65 + appendColumn); // Convert 0-based to A, B, C, etc.
	const columnRange = sheet.getRange(columnLetter + ":" + columnLetter);
	const columnData = columnRange.getValues();

	Logger.log("Column letter: " + columnLetter);
	Logger.log("Column data length: " + columnData.length);

	// Find first empty cell from top to bottom
	startRow = null;
	for (let i = 0; i < columnData.length; i++) {
	  const cellValue = columnData[i][0];
	  if (cellValue === "" || cellValue === null || cellValue === undefined) {
		startRow = i + 1; // Convert to 1-based row number
		Logger.log("Found first empty cell at row: " + startRow);
		break;
	  }
	}

	// If no empty cell found, append after last row
	if (startRow === null) {
	  startRow = sheet.getLastRow() + 1;
	  Logger.log("No empty cells found, appending after last row: " + startRow);
	}
  } else {
	// Fallback to original behavior: last row with any data
	const lastRow = sheet.getLastRow();
	startRow = lastRow + 1;
	Logger.log("No appendColumn specified, using getLastRow() behavior: " + startRow);
  }

  Logger.log("Appending to sheet: " + sheetName);
  Logger.log("Using appendColumn: " + appendColumn);
  Logger.log("Start row: " + startRow);
  Logger.log("Number of rows to append: " + values.length);
  Logger.log("Columns per row: " + (values.length > 0 ? values[0].length : 0));

  // Ensure we have data
  if (!values || values.length === 0) {
	throw new Error("No values to append");
  }

  // Get the number of columns
  const numCols = values[0].length;

  // Append the rows
  const range = sheet.getRange(startRow, 1, values.length, numCols);
  range.setValues(values);

  Logger.log("Successfully appended data to range: " + range.getA1Notation());

  return {
	appendedRows: values.length,
	startRow: startRow,
	sheetName: sheetName,
	columnsAppended: numCols
  };
}

/**
 * Test function - Run this to test locally before deployment
 * Open Execution Log (Ctrl+Enter) to see output
 */
function test() {
  const testData = {
	sheetName: "Sheet1",
	values: [
	  ["2024", "JANUARI", "15", "100000", "50000", "RM001", "Test Treatment", "Rawat Jalan", "Dr. Smith", "Nurse A"]
	]
  };

  try {
	const result = appendToSheet(testData.sheetName, testData.values);
	Logger.log("Test successful: " + JSON.stringify(result));
  } catch (error) {
	Logger.log("Test failed: " + error.toString());
  }
}
