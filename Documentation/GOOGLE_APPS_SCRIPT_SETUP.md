# Google Apps Script Deployment Guide

## Overview
This guide explains how to set up Google Apps Script to replace the Google Cloud implementation for AlenkaAssistant.

## Prerequisites
- A Google account
- A Google Sheet where you want to store the data
- The Google Sheet ID (from the URL: `https://docs.google.com/spreadsheets/d/{SPREADSHEET_ID}/edit`)

## Step-by-Step Setup

### 1. Create a Google Apps Script Project
1. Go to [Google Apps Script](https://script.google.com)
2. Click "New project"
3. Give it a name (e.g., "AlenkaAssistant - Sheets Sync")

### 2. Copy the Script Code
1. Open `GoogleAppsScript.js` from this directory
2. Copy all the code
3. Paste it into the Google Apps Script editor (replacing any default code)
4. Update the `SPREADSHEET_ID` constant with your actual Google Sheet ID:
   ```javascript
   const SPREADSHEET_ID = "YOUR_ACTUAL_SPREADSHEET_ID";
   ```

### 3. Test the Script (Optional but Recommended)
1. In the Google Apps Script editor, select the `test` function from the dropdown (top center)
2. Click the Run button (▶)
3. Check the Execution log (Ctrl+Enter) to verify it works
4. If successful, you should see: "Test successful: {appendedRows: 1, ...}"

### 4. Deploy as a Web App
1. Click "Deploy" > "New Deployment"
2. Click the gear icon and select "Web app"
3. Fill in:
   - **Execute as**: Your Google account (the one that owns the sheet)
   - **Who has access**: "Anyone" (or specific users if needed)
4. Click "Deploy"
5. A dialog will appear asking for permissions - click "Authorize"
6. Review permissions and click "Allow"
7. Copy the deployment URL from the dialog (it will look like):
   ```
   https://script.google.com/macros/d/{DEPLOYMENT_ID}/userweb
   ```

### 5. Configure AlenkaAssistant
1. Open `GoogleSheetsConfig.json`
2. Update it with:
   ```json
   {
	 "deploymentUrl": "https://script.google.com/macros/d/{DEPLOYMENT_ID}/userweb",
	 "spreadsheetId": "YOUR_SPREADSHEET_ID",
	 "sheetName": "Sheet1",
	 "enabled": true
   }
   ```
3. Replace placeholders with actual values

## Troubleshooting

### "Sheet not found" Error
- Verify the sheet name matches exactly (case-sensitive)
- Ensure you're using the correct Google Sheet ID

### 403 Forbidden / CORS Errors
- This is normal in some browsers during initial setup
- The script should work when called from your .NET application
- If persisting, check that "Who has access" is set to "Anyone"

### Data Not Appearing
- Check that you're authorized with the correct Google account
- Verify the Google Sheet ID in the script matches your actual sheet
- Check the Apps Script execution logs for errors

## Updates and Versioning
After making changes to the Apps Script:
1. Update the code in the script editor
2. Click "Deploy" > "Manage deployments"
3. Create a new version (each deployment creates a new version)
4. Update the URL in `GoogleSheetsConfig.json` if a new deployment URL is generated

## Data Format
The service expects data in this column order:
1. Year
2. Month (Indonesian name: JANUARI, FEBRUARI, etc.)
3. Date
4. Total Cost
5. Cost Detail
6. RM# (User ID)
7. Treatment Description (Tindakan)
8. Treatment Type
9. Assistant Names
10. Doctor Name
