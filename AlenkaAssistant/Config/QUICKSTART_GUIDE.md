# AlenkaAssistant - Google Apps Script Quick Start Guide

## What You Need

Before starting, gather:
- ✅ Your Google Sheet URL: `https://docs.google.com/spreadsheets/d/YOUR_SHEET_ID_HERE/edit`
- ✅ Your Google Sheet ID (the long string between `/d/` and `/edit`)
- ✅ A Google account with access to the sheet

---

## Step 1: Extract Your Sheet ID

From your Google Sheet URL: `https://docs.google.com/spreadsheets/d/1a2b3c4d5e6f7g8h9i0j/edit`

**The Sheet ID is:** `1a2b3c4d5e6f7g8h9i0j`

Copy this - you'll need it soon!

---

## Step 2: Create Google Apps Script Project

1. Open **[script.google.com](https://script.google.com)**
2. Click **"New project"** button (top left)
3. Give your project a name:
   - Example: `AlenkaAssistant - Sheets Sync`
4. Click **Create**

---

## Step 3: Copy the Script Code

1. Locate file: `AlenkaAssistant/Config/GoogleAppsScript.js` in your project
2. Open it and copy **all the code**
3. Go back to Google Apps Script editor
4. Delete all existing code (clear the editor)
5. Paste the entire `GoogleAppsScript.js` code

---

## Step 4: Configure the Sheet ID

In the Google Apps Script editor, find this line near the top:

```javascript
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";
```

Replace `YOUR_SPREADSHEET_ID` with your actual sheet ID from Step 1:

```javascript
const SPREADSHEET_ID = "1a2b3c4d5e6f7g8h9i0j";
```

**Click "Save"** (Ctrl+S)

---

## Step 5: Test the Script (Recommended)

1. In the script editor, find the **function dropdown** (top area, shows "Select function")
2. Click it and select **`test`**
3. Click the **Run ▶️ button** (or press Ctrl+Enter)
4. First time: Click **Authorize** and grant permissions
5. Check the **Execution log** (View → Logs or Ctrl+Enter)

**Expected success message:**
```
Test successful: {appendedRows: 1, startRow: 2, sheetName: "Sheet1"}
```

---

## Step 6: Deploy as Web App

### 6.1 Create Deployment
1. Click **"Deploy"** button (top right)
2. Click **"New deployment"**
3. Click the **gear icon ⚙️** next to "New Deployment"
4. Select **"Web app"** from the dropdown

### 6.2 Configure Permissions
Fill in the deployment settings:
- **Execute as:** Your Google Account (the email that owns the sheet)
- **Who has access:** "Anyone" (important!)
- Click **Deploy**

### 6.3 Authorize Access
1. A Google authorization dialog appears
2. Click **Authorize**
3. Select your Google account
4. Click **Allow** (grant permissions)

### 6.4 Copy Deployment URL
1. A dialog shows your deployment URL
2. **Copy this URL** - you need it for configuration:
   ```
   https://script.google.com/macros/d/ABC123DEF456GHI789JKL/userweb
   ```

---

## Step 7: Configure AlenkaAssistant

Now that you have the deployment URL, configure your .NET app:

### 7.1 Find the Config File
Navigate to: `AlenkaAssistant/Config/GoogleSheetsConfig.json`

### 7.2 Update Configuration

Replace the entire contents with your actual values:

```json
{
  "deploymentUrl": "https://script.google.com/macros/d/ABC123DEF456GHI789JKL/userweb",
  "spreadsheetId": "1a2b3c4d5e6f7g8h9i0j",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**Replace these placeholders:**
- `ABC123DEF456GHI789JKL` → Your deployment URL (from Step 6.4)
- `1a2b3c4d5e6f7g8h9i0j` → Your sheet ID (from Step 1)
- `Sheet1` → The exact sheet name (case-sensitive)

### 7.3 Save the File
Save `GoogleSheetsConfig.json` with your updates

---

## Step 8: Verify Your Google Sheet Structure

Your Google Sheet should have these columns (in order):

| A | B | C | D | E | F | G | H | I | J |
|---|---|---|---|---|---|---|---|---|---|
| Year | Month | Date | Total Cost | Cost Detail | RM# | Tindakan | Type | Assistants | Doctor |

**Column Headers (Optional but recommended):**
- A: Year
- B: Month
- C: Date
- D: Total Cost
- E: Cost Detail
- F: RM#
- G: Tindakan
- H: Treatment Type
- I: Assistant Names
- J: Doctor Name

---

## Step 9: Test the Integration

1. **Build your AlenkaAssistant project** (Visual Studio → Build → Build Solution)
2. **Run the application**
3. **Create a purchase request** and submit
4. **Check your Google Sheet** for the new data
5. **Verify data appears** in the correct columns

---

## ✅ Success Signs

You should see:
- ✅ No error messages in AlenkaAssistant
- ✅ Status shows "Saved to Google Sheets successfully"
- ✅ New rows appear in your Google Sheet
- ✅ Data is in the correct columns

---

## ❌ Troubleshooting

### Problem: "Google Sheets is not enabled in config"
**Solution:**
- Make sure `"enabled": true` in `GoogleSheetsConfig.json`
- Verify the file was saved

### Problem: "DeploymentUrl is not configured"
**Solution:**
- Your deployment URL is empty or missing
- Go back to Step 6.4 and copy the correct URL
- Paste it in `GoogleSheetsConfig.json`

### Problem: "Sheet not found" error
**Solution:**
- Check that `sheetName` matches exactly (case-sensitive)
- If your sheet is named "Data", use `"sheetName": "Data"`
- Default is usually "Sheet1"

### Problem: 403 Forbidden error
**Solution:**
- Verify "Who has access" is set to "Anyone" in the deployment
- Re-deploy the script with correct permissions
- Clear browser cache and try again

### Problem: Data not appearing in Google Sheet
**Solution:**
1. Check Google Apps Script execution logs:
   - Go to script.google.com
   - Open your project
   - Click View → Execution log
   - Look for errors
2. Verify the deployment URL is correct in config
3. Make sure you're logged into the correct Google account
4. Check that the sheet ID is correct

### Problem: "Unknown error" or blank response
**Solution:**
1. Test the script directly:
   - In Google Apps Script editor, select `test` function
   - Click Run
   - Check the log for errors
2. Verify SPREADSHEET_ID in the script matches your sheet ID
3. Make sure the sheet you're referencing exists

---

## Advanced: Multiple Sheets

If you want to append to different sheets, you can:

1. **Update the JavaScript function** to accept sheet name in the payload
2. Or create **multiple deployments** - one per sheet
3. Or update `GoogleSheetsConfig.json` `sheetName` field dynamically

---

## Advanced: Security

For production deployments, consider:

1. Change **"Who has access"** to specific users/domains instead of "Anyone"
2. Implement API key validation in the script
3. Add request logging and monitoring
4. Use Google's standard OAuth 2.0 flow for advanced auth

---

## Video Tutorial Links (External)

- [Google Apps Script Intro](https://www.youtube.com/results?search_query=google+apps+script+tutorial)
- [Google Sheets API via Apps Script](https://www.youtube.com/results?search_query=google+sheets+apps+script)
- [Web Apps Deployment](https://www.youtube.com/results?search_query=google+apps+script+web+app+deployment)

---

## Need Help?

If you're stuck:

1. **Check the Troubleshooting section above**
2. **Review the Google Apps Script Execution Log** for error messages
3. **Verify all credentials match** (Sheet ID, Deployment URL)
4. **Test the script function** using the `test()` function
5. **Check file permissions** - ensure config file is readable by the app

---

## Summary Checklist

- [ ] Found your Google Sheet ID
- [ ] Created Google Apps Script project
- [ ] Copied and pasted the script code
- [ ] Updated SPREADSHEET_ID with your actual ID
- [ ] Tested the script (test function)
- [ ] Deployed as Web App
- [ ] Copied deployment URL
- [ ] Updated GoogleSheetsConfig.json with all values
- [ ] Verified Google Sheet structure
- [ ] Tested integration with AlenkaAssistant
- [ ] Data appears in Google Sheet

**Once all items are checked, you're ready to go! 🚀**
