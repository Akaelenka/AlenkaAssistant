# Visual Step-by-Step Guide: From Sheet URL to Working Configuration

## Your Google Sheet URL

When you open your Google Sheet in a browser, the URL looks like this:

```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit#gid=0
```

### Extract the Spreadsheet ID

The **SPREADSHEET_ID** is the long string between `/d/` and `/edit`:

```
URL: https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
											↑                                                ↑
									   START HERE                                    END HERE

SPREADSHEET_ID = 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
```

**Action:** Copy this ID to your clipboard.

---

## Your Google Sheet Tabs

At the bottom of your Google Sheet, you'll see sheet tabs:

```
┌─────────────────────────────────────────────────────────────────┐
│ My Google Sheet                                             [+]  │
├─────────────────────────────────────────────────────────────────┤
│ [...Sheet contents...]                                          │
│                                                                 │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│  📄 Sheet1  │ 📄 Data  │ 📄 Archive  │  ...                     │
└─────────────────────────────────────────────────────────────────┘
```

The **SHEET_NAME** is the name of the active tab (usually "Sheet1").

**Action:** Note the exact name (case-sensitive).

---

## Creating the Google Apps Script Project

### 1. Go to Google Apps Script

Visit: https://script.google.com

You should see:

```
┌─────────────────────────────────────────────────────────┐
│  Google Apps Script                                     │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [+ New project]  [Open]  [etc...]                     │
│                                                         │
│  Recent projects:                                       │
│  - My Project 1                                         │
│  - My Project 2                                         │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Action:** Click `[+ New project]`

---

### 2. Name Your Project

After clicking "New project", you'll see:

```
┌─────────────────────────────────────────────────────────┐
│  📋 Untitled project                                    │
│                                                         │
│  (shows default code template)                          │
│                                                         │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Action:** 
1. Click "Untitled project" at the top
2. Rename it to: `AlenkaAssistant - Sheets Sync`
3. Press Enter

---

### 3. Replace the Script Code

In the editor, you'll see default code. 

**Action:**
1. Select all (Ctrl+A)
2. Delete it
3. Copy code from: `GoogleAppsScript.js` (in your project Config folder)
4. Paste it into the editor
5. Press Ctrl+S to save

Result should look like:

```
┌─────────────────────────────────────────────────────────┐
│  📋 AlenkaAssistant - Sheets Sync              [Save]   │
│                                                         │
│  const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";         │
│  function doPost(e) { ... }                            │
│  function appendToSheet(sheetName, values) { ... }     │
│  function test() { ... }                               │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

### 4. Update the SPREADSHEET_ID

Find this line in the script:

```javascript
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";
```

Replace it with your actual ID:

```javascript
const SPREADSHEET_ID = "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c";
```

**Action:**
1. Click on "YOUR_SPREADSHEET_ID" line
2. Highlight the text in quotes
3. Type or paste your Spreadsheet ID
4. Press Ctrl+S

---

### 5. Test the Script (Optional)

At the top of the editor, you'll see:

```
┌──────────────────────────────────────────────────┐
│  [Select function ▼]  [▶ Run]  [Debug ▼]        │
└──────────────────────────────────────────────────┘
```

**Action:**
1. Click "Select function" dropdown
2. Choose `test`
3. Click `Run ▶`
4. Click "Authorize" if prompted
5. Grant permissions

Check the execution log (Ctrl+Enter):

```
2024-01-15 10:30:45    Test successful: {appendedRows: 1, startRow: 2, sheetName: "Sheet1"}
```

✅ If you see this, the script works!

---

### 6. Deploy as Web App

Click **"Deploy"** button (top right):

```
┌──────────────────────────────────────┐
│  [Deploy ▼]                          │
└──────────────────────────────────────┘
```

A dropdown appears:

```
┌──────────────────────────────────────┐
│  Deploy ▼                            │
│  ├─ New deployment                   │
│  ├─ Manage deployments               │
│  └─ ● Version history                │
└──────────────────────────────────────┘
```

**Action:** Click `New deployment`

---

### 7. Select "Web app"

You'll see:

```
┌──────────────────────────────────────────────────────┐
│  Create new deployment                              │
│                                                     │
│  [⚙️ Select type]                                  │
│                                                     │
│  (Dropdown opens...)                               │
│  ├─ Web app                                         │
│  ├─ Head                                            │
│  └─ API Executable                                  │
└──────────────────────────────────────────────────────┘
```

**Action:** Click `Web app`

---

### 8. Configure Deployment

A form appears:

```
┌──────────────────────────────────────────────────────┐
│  Configuration                                      │
│                                                     │
│  Description: ________________________              │
│  (optional)                                         │
│                                                     │
│  Execute as:        [Your Email ▼]                 │
│                                                     │
│  Who has access:    [Anyone ▼]                     │
│                     (Important! Must be "Anyone")   │
│                                                     │
│  [Cancel]  [Deploy]                                │
└──────────────────────────────────────────────────────┘
```

**Action:**
1. Leave "Description" empty (optional)
2. For "Execute as": Select your Google account email
3. For "Who has access": Select "Anyone"
4. Click `Deploy`

---

### 9. Authorize

A popup asks for permissions:

```
┌──────────────────────────────────────────────────────┐
│  Authorization required                            │
│                                                     │
│  "AlenkaAssistant - Sheets Sync"                   │
│  wants to:                                          │
│                                                     │
│  ✓ View and manage spreadsheets                    │
│                                                     │
│  [⬅ Go back]  [Allow]                              │
└──────────────────────────────────────────────────────┘
```

**Action:** Click `Allow`

---

### 10. Copy Your Deployment URL

After authorization, a success dialog shows:

```
┌──────────────────────────────────────────────────────┐
│  Deployment successful!                             │
│                                                     │
│  Deployment ID: 1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o    │
│                                                     │
│  Web app URL:                                       │
│  https://script.google.com/macros/d/               │
│  1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb         │
│                                                     │
│  [Copy]                                             │
│                                                     │
│  [Authorize other users]  [Done]                    │
└──────────────────────────────────────────────────────┘
```

**Action:**
1. Click `Copy` to copy the full URL
2. Click `Done`

You now have your **DEPLOYMENT_URL**:
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
```

---

## Configuring AlenkaAssistant

Now you have all three pieces of information:

1. ✅ **SPREADSHEET_ID**: `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c`
2. ✅ **DEPLOYMENT_URL**: `https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb`
3. ✅ **SHEET_NAME**: `Sheet1`

### Step 1: Open GoogleSheetsConfig.json

Navigate to: `AlenkaAssistant/Config/GoogleSheetsConfig.json`

You'll see:

```json
{
  "deploymentUrl": "https://script.google.com/macros/d/YOUR_DEPLOYMENT_ID/userweb",
  "spreadsheetId": "YOUR_SPREADSHEET_ID",
  "sheetName": "Sheet1",
  "enabled": false
}
```

---

### Step 2: Update with Your Values

Replace all placeholders:

**Before:**
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/YOUR_DEPLOYMENT_ID/userweb",
  "spreadsheetId": "YOUR_SPREADSHEET_ID",
  "sheetName": "Sheet1",
  "enabled": false
}
```

**After (your actual values):**
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb",
  "spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**Important Changes:**
- Copy-paste the full `deploymentUrl` from Step 10
- Copy-paste the `spreadsheetId` from Step 1
- Keep or change `sheetName` if your sheet has a different name
- Change `enabled` from `false` to `true`

---

### Step 3: Save the File

Press Ctrl+S or File → Save

Visual Studio shows:

```
GoogleSheetsConfig.json (file saved ✓)
```

---

## Testing the Integration

### Step 1: Build the Project

In Visual Studio:
1. Click **Build** → **Build Solution**
2. Wait for build to complete
3. Check Output window for any errors

You should see:
```
========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
```

---

### Step 2: Run the Application

Press **F5** or Click **Start Debugging**

Your application starts.

---

### Step 3: Create a Purchase Request

In the application:
1. Fill in all required fields
2. Click **"Save to Google Sheets"** button

The app shows:
```
⏳ Saving to Google Sheets...
✅ Purchase request saved to Google Sheets successfully!
```

---

### Step 4: Verify in Google Sheets

Switch to your Google Sheet browser tab:

```
┌─────────────────────────────────────────────────────────┐
│ My Google Sheet                                         │
├─────────────────────────────────────────────────────────┤
│  Year │ Month    │ Date │ Total Cost │ ... │ Doctor   │
├─────────────────────────────────────────────────────────┤
│ 2024 │ JANUARI  │  15  │  1000000   │ ... │ Dr. Smith│
│ 2024 │ JANUARI  │  20  │  500000    │ ... │ Dr. Jones│
│      │          │      │            │ ... │          │
└─────────────────────────────────────────────────────────┘
```

✅ **Success!** Your data is now in Google Sheets!

---

## Troubleshooting Visual Guide

### Issue: "Google Sheets is not enabled in config"

```
❌ Error message appears
```

**Solution:**

1. Open `GoogleSheetsConfig.json`
2. Find the line: `"enabled": false`
3. Change it to: `"enabled": true`
4. Save the file (Ctrl+S)
5. Rebuild the project

---

### Issue: "DeploymentUrl is not configured"

```
❌ Error: DeploymentUrl is not configured in GoogleSheetsConfig.json
```

**Solution:**

1. Go back to Google Apps Script
2. Click "Deploy" → "Manage deployments"
3. Find the active deployment
4. Copy the full URL (not just the ID)
5. Paste into `GoogleSheetsConfig.json`

**Check format:**
```
✅ Correct:  https://script.google.com/macros/d/ABC123.../userweb
❌ Wrong:    https://script.google.com/macros/s/ABC123.../userweb
❌ Wrong:    just the ID without https://
```

---

### Issue: "Sheet not found"

```
❌ Error: Sheet "Sheet1" not found
```

**Solution:**

1. Open your Google Sheet
2. Look at the sheet tabs at bottom
3. Check the exact name (case matters!)
4. Update in `GoogleSheetsConfig.json`

**Examples:**
```json
"sheetName": "Sheet1"      ✅ Default
"sheetName": "Data"        ✅ Custom name
"sheetName": "sheet1"      ❌ Wrong case
"sheetName": "Sheet 1"     ❌ Extra space
```

---

### Issue: 403 Forbidden Error

```
❌ Error: HTTP 403 Forbidden
```

**Solution:**

In Google Apps Script:
1. Go to "Deploy" → "Manage deployments"
2. Click the pencil/edit icon on your deployment
3. Check "Who has access": must be **"Anyone"**
4. If it's not, click "Update" to change it
5. Try again

---

### Issue: Data Not Appearing

```
❌ Click "Save to Google Sheets" but data doesn't appear
```

**Solution:**

1. **Verify correct account**: Are you logged into the correct Google account in both:
   - Google Apps Script deployment
   - Google Sheet viewing

2. **Check Sheet ID**: In Google Apps Script:
   - Open your project
   - Line 3 should have: `const SPREADSHEET_ID = "1BxiMVs0..."`
   - Make sure it's your ACTUAL Sheet ID

3. **Check Execution Logs**: In Google Apps Script:
   - Click View → Logs
   - Look for error messages
   - Example error: "Sheet "Data" not found"

4. **Manual test**: In Google Apps Script:
   - Select `test` function
   - Click Run
   - Check logs for success

---

## Summary Table

| What | Where | Value |
|-----|-------|-------|
| **Sheet ID** | Google Sheet URL | `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c` |
| **Deployment URL** | Google Apps Script Deploy | `https://script.google.com/macros/d/1a2b3c4d.../userweb` |
| **Sheet Name** | Google Sheet tab name | `Sheet1` |
| **Config File** | Visual Studio | `AlenkaAssistant/Config/GoogleSheetsConfig.json` |
| **Script File** | Google Apps Script | `GoogleAppsScript.js` |
| **Enable Flag** | Config file | `"enabled": true` |

---

## Next: You're Ready! 🚀

You can now:
- ✅ Create purchase requests
- ✅ Save them to Google Sheets
- ✅ View data in real-time
- ✅ Share with team members (they can see the sheet)
- ✅ Generate reports from the data

Congratulations! 🎉
