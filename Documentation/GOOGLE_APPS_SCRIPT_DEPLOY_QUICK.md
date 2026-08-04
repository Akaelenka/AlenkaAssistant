# Quick Deploy Guide - Google Apps Script Update

## For Your Google Sheet

### Step 1: Copy the Updated Code
1. Open the file: `AlenkaAssistant/Config/GoogleAppsScript.js` in this repository
2. Copy ALL the code

### Step 2: Update Your Google Apps Script

1. Go to https://script.google.com
2. Open your deployment project (the one with your `SPREADSHEET_ID`)
3. Select all code in the editor (Ctrl+A)
4. Delete it
5. Paste the new code
6. Press Ctrl+S to save

**That's it!** No need to redeploy. Changes take effect immediately.

### Step 3: Verify

Test the new `getLastRm` action:

1. Open a browser and navigate to:
```
https://your-deployment-url/exec?action=getLastRm&sheetName=NoRM&searchColumn=0
```

2. You should see JSON like:
```json
{
  "success": true,
  "lastRm": "A.0050",
  "lastRow": 51
}
```

---

## What Changed?

### New GET Action
- **`getLastRm`** - Fetches the last RM number from the NoRM sheet
  - Used by the "Tambah Pasien" button to suggest next RM

### New POST Action  
- **`addPatient`** - Adds a new patient row to the NoRM sheet
  - Called automatically when you Print or Send to Google Sheets
  - Includes RM number and patient name

### Updated Functions
- `doGet()` - Now handles `getLastRm` action
- `doPost()` - Now handles `addPatient` action and routing

### New Helper Functions
- `getLastRmFromSheet()` - Scans NoRM sheet for last RM
- `addPatientToSheet()` - Writes new patient record to sheet

---

## Troubleshooting

### "Sheet not found" Error
- Verify the NoRM sheet exists in your Google Sheet
- Check spelling and capitalization match exactly

### No RM values returned
- Make sure you have at least one RM value in the NoRM sheet
- Verify the column index is correct (default: 0 = column A)

### Action not found
- Clear your browser cache and try again
- Verify you copied all the new code

### Still having issues?
- Check the Google Apps Script logs:
  1. Go to script.google.com
  2. Click "Executions" 
  3. Look for error messages with timestamps
  4. Copy error details for troubleshooting

---

## Rollback (If Needed)

If you need to go back to the previous version:

1. The previous code is in: `AlenkaAssistant/Config/GoogleAppsScript_Fixed.js`
2. Copy and paste it into your Google Apps Script project
3. Save and test

---

## Notes

- The new code is **backward compatible** - old requests still work
- All new features include **comprehensive logging** for debugging
- No need to change your spreadsheet structure
- Works with existing `GoogleSheetsConfig.json` configuration

---

## Questions?

Refer to: `AlenkaAssistant/Config/GOOGLE_APPS_SCRIPT_NEW_FEATURES.md`
