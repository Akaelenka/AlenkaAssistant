# Google Apps Script - New Patient Management Features

## Overview

The Google Apps Script has been updated to support the new patient creation workflow in AlenkaAssistant. This document describes the new actions and how to use them.

## New Actions

### 1. `getLastRm` - Retrieve the Last RM Number

**Purpose**: Get the last RM (patient record number) from the NoRM sheet to suggest the next sequential RM.

**Request Type**: GET

**Parameters**:
- `action`: "getLastRm"
- `sheetName`: Name of the sheet containing RMs (default: "NoRM")
- `searchColumn`: Column index where RMs are stored (default: 0)

**Example URL**:
```
https://your-deployment-url/exec?action=getLastRm&sheetName=NoRM&searchColumn=0
```

**Response**:
```json
{
  "success": true,
  "lastRm": "A.0050",
  "lastRow": 51,
  "message": "Last RM found"
}
```

**Response (No Data)**:
```json
{
  "success": true,
  "lastRm": null,
  "lastRow": 1,
  "message": "No data in sheet, start with A.0001"
}
```

---

### 2. `addPatient` - Add a New Patient to NoRM Sheet

**Purpose**: Add a new patient record to the NoRM sheet with RM number and patient name.

**Request Type**: POST

**Payload Structure**:
```json
{
  "action": "addPatient",
  "sheetName": "NoRM",
  "rmNumber": "A.0051",
  "patientName": "John Doe",
  "rmColumn": 0,
  "patientNameColumn": 1
}
```

**Example POST Request** (from C#):
```csharp
var payload = new {
	action = "addPatient",
	sheetName = "NoRM",
	rmNumber = "A.0051",
	patientName = "John Doe",
	rmColumn = 0,
	patientNameColumn = 1
};

var json = JsonSerializer.Serialize(payload);
var content = new StringContent(json, Encoding.UTF8, "application/json");
var response = await httpClient.PostAsync(deploymentUrl, content);
```

**Response (Success)**:
```json
{
  "success": true,
  "message": "Patient added successfully",
  "newRow": 52,
  "rmNumber": "A.0051",
  "patientName": "John Doe"
}
```

**Response (Error)**:
```json
{
  "success": false,
  "error": "Sheet not found: NoRM"
}
```

---

## Workflow Integration

### Complete Patient Creation Flow

```
User clicks "Tambah Pasien" button
	↓
Dialog opens
	↓
App calls: GET ?action=getLastRm&sheetName=NoRM
	↓
Dialog shows suggested next RM (e.g., "A.0051")
	↓
User enters patient name and confirms
	↓
Data stored in local form (not yet saved to sheet)
	↓
User clicks "Print" or "Kirim ke Google Sheets"
	↓
App calls: POST action=addPatient with RM and patient name
	↓
App saves patient to NoRM sheet
	↓
App continues with normal transaction flow
```

---

## Configuration

The patient lookup configuration is stored in `GoogleSheetsConfig.json`:

```json
"patientLookup": {
  "sheetName": "NoRM",
  "rmColumn": 0,
  "patientNameColumn": 1
}
```

These values are automatically passed to the new functions by the C# client.

---

## Error Handling

Both new actions include comprehensive error handling:

- **Sheet not found**: Returns error message
- **Missing data**: Returns error message
- **Empty RM/Patient Name**: Returns validation error
- **All errors**: Logged in Google Apps Script logs for debugging

### Debugging

To debug issues:
1. Open your Google Apps Script project at https://script.google.com
2. Go to **Executions** to see logs from all actions
3. Look for messages starting with `=== getLastRmFromSheet ===` or `=== addPatientToSheet ===`

---

## Deployment Steps

### If You're Updating an Existing Deployment

1. Open your Google Apps Script project at https://script.google.com
2. Go to the Script Editor
3. Replace the entire code with the updated `GoogleAppsScript.js`
4. Press **Ctrl+S** to save
5. No re-deployment needed! The changes take effect immediately.

### Verify It Works

Use **Test Deployment** to verify:
1. Create a test tab in your spreadsheet named "TestNoRM"
2. Add a few test RM values (e.g., "A.0001", "A.0002")
3. Try the `getLastRm` action with URL:
   ```
   https://your-deployment-url/exec?action=getLastRm&sheetName=TestNoRM&searchColumn=0
   ```
4. You should get back the last RM value

---

## Migration Checklist

- [ ] Update Google Apps Script with new code
- [ ] Verify the `NoRM` sheet exists in your Google Sheet
- [ ] Test `getLastRm` action in a browser
- [ ] Test `addPatient` action via Postman or similar tool
- [ ] Update AlenkaAssistant to use the new deployment URL (if changed)
- [ ] Verify "Tambah Pasien" button works in the app
- [ ] Test complete workflow: create patient → print → verify NoRM sheet updated

---

## FAQ

**Q: What if the NoRM sheet doesn't exist?**
A: The script will return an error. Make sure the sheet name matches exactly (case-sensitive).

**Q: Can I change the column numbers?**
A: Yes! The `rmColumn` and `patientNameColumn` are passed as parameters, so they can be any column index (0-based).

**Q: What RM format should I use?**
A: Any format is fine (e.g., "A.0051", "P001", "001"). The format is flexible.

**Q: Does it create duplicate entries?**
A: Currently, the script adds the patient every time you submit. If you want duplicate prevention, the C# client can check if the patient already exists before calling `addPatient`.

---

## Support

If you encounter issues:
1. Check the Google Apps Script execution logs
2. Verify your spreadsheet ID matches `SPREADSHEET_ID` in the script
3. Ensure the sheet names match your actual Google Sheet
4. Test each action independently using a browser or Postman
