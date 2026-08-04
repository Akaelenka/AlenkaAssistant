# 🎯 Complete Visual Guide: From Google Sheet URL to Working Configuration

## The Big Picture

```
YOUR GOOGLE SHEET
	│
	├─ Extract Sheet ID from URL
	│
	↓

GOOGLE APPS SCRIPT (your deployment)
	│
	├─ Create project
	├─ Copy code
	├─ Test
	├─ Deploy
	│
	↓ Get Deployment URL

ALENKA ASSISTANT CONFIG
	│
	├─ GoogleSheetsConfig.json
	├─ Add Deployment URL
	├─ Add Sheet ID
	├─ Set enabled: true
	│
	↓

✅ DONE! Data saves to Google Sheet
```

---

## Step 1: Your Google Sheet URL

### What You See in Browser
```
┌─────────────────────────────────────────────────────────────────┐
│ https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiE  │
│ kVQZGzGkL8JzIgnKPZCH3c/edit#gid=0                              │
│                                                                 │
│  My Purchase Tracking Sheet                                    │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Year │ Month  │ Date │ Total  │ ... │ Doctor          │ │
│  ├──────────────────────────────────────────────────────────┤ │
│  │ 2024 │ JAN    │ 15   │ 500000 │ ... │ Dr. Smith       │ │
│  │ 2024 │ JAN    │ 20   │ 350000 │ ... │ Dr. Jones       │ │
│  │      │        │      │        │ ... │                 │ │
│  └──────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Extract the Sheet ID
```
URL: https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit

Breaking it down:
┌─────────────────────────────────────────────────────────┐
│ https://docs.google.com/spreadsheets/d/               │
│ 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c         │ ← COPY THIS
│ /edit                                                  │
└─────────────────────────────────────────────────────────┘

SHEET_ID = 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
```

**Action:** Copy the Sheet ID to your clipboard

---

## Step 2: Create Google Apps Script Project

### Visit Google Apps Script
```
┌─────────────────────────────────────────────────────────┐
│                                                         │
│         Go to: https://script.google.com               │
│                                                         │
│         You'll see:                                     │
│         ┌─────────────────────────────────────────────┐ │
│         │ Google Apps Script                          │ │
│         │ ┌─────────────────────────────────────────┐ │ │
│         │ │ [+ New project]   [Open]               │ │ │
│         │ └─────────────────────────────────────────┘ │ │
│         └─────────────────────────────────────────────┘ │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Action:** Click `[+ New project]`

---

## Step 3: Name Your Project

### After Clicking New Project
```
┌─────────────────────────────────────────────────────────┐
│ Untitled project                                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  (Default code template appears here)                  │
│                                                         │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Rename the Project
```
┌─────────────────────────────────────────────────────────┐
│ [Click Here] "Untitled project"                         │
│                                                         │
│ Edit name box appears ↓                                │
│                                                         │
│ ┌──────────────────────────────────────────────────┐   │
│ │ AlenkaAssistant - Sheets Sync                    │   │
│ └──────────────────────────────────────────────────┘   │
│                                                         │
│ Then press Enter                                        │
└─────────────────────────────────────────────────────────┘
```

**Action:** 
1. Click "Untitled project"
2. Type: `AlenkaAssistant - Sheets Sync`
3. Press Enter

---

## Step 4: Copy the Script Code

### From Your Project
```
AlenkaAssistant/Config/
│
└── GoogleAppsScript.js
```

### What to Copy
```javascript
/**
 * Google Apps Script for AlenkaAssistant
 * ...
 */
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";

function doPost(e) {
  // ... all the code
}
```

**Action:**
1. Open GoogleAppsScript.js from your project
2. Select all (Ctrl+A)
3. Copy (Ctrl+C)

---

## Step 5: Paste into Google Apps Script

### In Google Apps Script Editor
```
┌─────────────────────────────────────────────────────────┐
│ AlenkaAssistant - Sheets Sync              [Save]       │
├─────────────────────────────────────────────────────────┤
│ Code editor here (default code)                         │
│                                                         │
│ function myFunction() {                                 │
│   // Default code                                       │
│ }                                                       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### After Pasting
```
┌─────────────────────────────────────────────────────────┐
│ AlenkaAssistant - Sheets Sync              [Save]       │
├─────────────────────────────────────────────────────────┤
│ /**                                                     │
│  * Google Apps Script for AlenkaAssistant              │
│  */                                                     │
│ const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";          │
│ function doPost(e) { ... }                             │
│ function appendToSheet(...) { ... }                     │
│ function test() { ... }                                │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Action:**
1. Select all existing code (Ctrl+A)
2. Delete it
3. Paste your code (Ctrl+V)
4. Press Ctrl+S to save

---

## Step 6: Update the Sheet ID in Script

### Find This Line
```javascript
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";
```

### Replace It
```javascript
// BEFORE:
const SPREADSHEET_ID = "YOUR_SPREADSHEET_ID";

// AFTER (with your actual Sheet ID):
const SPREADSHEET_ID = "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c";
```

**Action:**
1. Double-click "YOUR_SPREADSHEET_ID" to select it
2. Type your actual Sheet ID (paste from clipboard)
3. Press Ctrl+S to save

---

## Step 7: Test the Script

### Select Test Function
```
┌─────────────────────────────────────────────────────────┐
│ [Select function ▼]  [▶ Run]  [Debug ▼]                │
│  ↑ Click here        ↑ Click here                       │
│                                                         │
│ Dropdown shows:                                         │
│ ├─ test                     ← Select this               │
│ ├─ doPost                                               │
│ └─ appendToSheet                                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Run the Test
```
┌─────────────────────────────────────────────────────────┐
│ [test ▼]  [▶ Run]  [Debug ▼]                           │
│           ↑ Click the Run button                        │
│                                                         │
│ You'll see:                                             │
│ "Authorization required"                                │
│ Click "Authorize" and grant permissions                │
│                                                         │
│ Then check Execution Logs (Ctrl+Enter):                │
│ ┌─────────────────────────────────────────────────────┐│
│ │ 2024-01-15 10:30:45  Test successful: {...}        ││
│ └─────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────┘
```

**Action:**
1. Click function dropdown
2. Select `test`
3. Click `Run ▶`
4. Authorize if prompted
5. Check logs (Ctrl+Enter)

**Expected Result:**
```
Test successful: {appendedRows: 1, startRow: 2, sheetName: "Sheet1"}
```

✅ If you see this message, the script works!

---

## Step 8: Deploy as Web App

### Click Deploy
```
┌─────────────────────────────────────────────────────────┐
│ [Deploy ▼]  ← Click here                               │
│                                                         │
│ Dropdown appears:                                       │
│ ├─ New deployment        ← Click this                  │
│ ├─ Manage deployments                                   │
│ └─ ● Version history                                    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Action:** Click `New deployment`

---

## Step 9: Configure Deployment

### Select Type
```
┌──────────────────────────────────────────────────────┐
│ Create new deployment                                │
│ ┌────────────────────────────────────────────────┐   │
│ │ [⚙️ Select type ▼]                            │   │
│ │                                                │   │
│ │ ├─ Web app      ← Click this one               │   │
│ │ ├─ Head                                        │   │
│ │ └─ API Executable                              │   │
│ └────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────┘
```

**Action:** Click `Web app`

---

## Step 10: Set Permissions

### The Configuration Form
```
┌──────────────────────────────────────────────────────┐
│ New deployment                                       │
│                                                      │
│ Description: ______________________________         │
│ (optional)                                           │
│                                                      │
│ Execute as:                                          │
│ ┌─ Your Email ▼                                     │
│                                                      │
│ Who has access:                                      │
│ ┌─ Anyone ▼      ← IMPORTANT: Must be "Anyone"     │
│                                                      │
│ [Cancel]  [Deploy]                                  │
└──────────────────────────────────────────────────────┘
```

**Action:**
1. Leave "Description" empty (optional)
2. For "Execute as": Keep selected (your email)
3. For "Who has access": **Select "Anyone"** ← IMPORTANT!
4. Click `Deploy`

---

## Step 11: Authorize

### Permission Dialog
```
┌──────────────────────────────────────────────────────┐
│ Authorization required                               │
│                                                      │
│ "AlenkaAssistant - Sheets Sync"                     │
│ wants to access your Google Account                  │
│                                                      │
│ Permissions requested:                               │
│ ✓ View and manage your spreadsheets in Google Sheets│
│                                                      │
│ [⬅ Go back]  [Allow]                                │
│              ↑ Click here                            │
└──────────────────────────────────────────────────────┘
```

**Action:** Click `Allow`

---

## Step 12: Copy Deployment URL

### Success Dialog
```
┌──────────────────────────────────────────────────────┐
│ ✅ Deployment successful!                            │
│                                                      │
│ Deployment ID:                                       │
│ 1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o                     │
│                                                      │
│ Web app URL:                                         │
│ ┌────────────────────────────────────────────────┐  │
│ │ https://script.google.com/macros/d/            │  │
│ │ 1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb      │  │
│ │                                                 │  │
│ │ [Copy]  ← Click to copy the URL                │  │
│ └────────────────────────────────────────────────┘  │
│                                                      │
│ [Authorize other users]  [Done]                      │
└──────────────────────────────────────────────────────┘
```

**Action:**
1. Click `[Copy]` button
2. Your deployment URL is now in clipboard
3. Click `Done`

**Your Deployment URL:**
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
```

---

## Step 13: Configure AlenkaAssistant

### Open GoogleSheetsConfig.json
```
Visual Studio
├─ Solution Explorer
│  ├─ AlenkaAssistant
│  │  ├─ Config
│  │  │  └─ GoogleSheetsConfig.json  ← Double-click to open
```

### Before Configuration
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/YOUR_DEPLOYMENT_ID/userweb",
  "spreadsheetId": "YOUR_SPREADSHEET_ID",
  "sheetName": "Sheet1",
  "enabled": false
}
```

### After Configuration
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb",
  "spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**Changes:**
```
"deploymentUrl":  YOUR_DEPLOYMENT_ID → 1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p
"spreadsheetId":  YOUR_SPREADSHEET_ID → 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
"sheetName":      Sheet1 → Sheet1 (no change if default)
"enabled":        false → true ✅ IMPORTANT!
```

**Action:**
1. Edit the 4 fields with your values
2. Save file (Ctrl+S)

---

## Step 14: Build the Project

### In Visual Studio
```
┌─────────────────────────────────────────────────────┐
│ Visual Studio                                       │
│                                                     │
│ [Build] menu → [Build Solution]                    │
│  (or press Ctrl+Shift+B)                           │
│                                                     │
│ Output window shows:                                │
│ ┌─────────────────────────────────────────────────┐│
│ │ ========== Build: 1 succeeded,                 ││
│ │ 0 failed, 0 up-to-date, 0 skipped ==========   ││
│ └─────────────────────────────────────────────────┘│
│                                                     │
│ ✅ Build successful!                               │
└─────────────────────────────────────────────────────┘
```

**Action:**
1. Press Ctrl+Shift+B to build
2. Wait for build to complete
3. Check output for success message

---

## Step 15: Run and Test

### Start the Application
```
Visual Studio
├─ Press F5 (Start Debugging)
│  or
├─ Debug → Start Debugging
│
└─ Application window opens
```

### Create a Purchase Request
```
┌────────────────────────────────────┐
│ AlenkaAssistant                    │
│                                    │
│ [Form fields...]                   │
│ └─ [Save to Google Sheets] button  │
│                                    │
└────────────────────────────────────┘
```

**Action:**
1. Fill in the form fields
2. Click `[Save to Google Sheets]`

---

## Step 16: Verify Success

### In AlenkaAssistant
```
┌────────────────────────────────────┐
│ Status: Saving to Google Sheets... │
│         (loading spinner)          │
│                                    │
│ After completion:                  │
│ ✅ Purchase request saved to       │
│    Google Sheets successfully!     │
│                                    │
└────────────────────────────────────┘
```

### In Your Google Sheet
```
Your Google Sheet (browser tab)
┌──────────────────────────────────────┐
│ Year │ Month │ Date │ ... │ Doctor  │
├──────────────────────────────────────┤
│ 2024 │ JAN   │ 15   │ ... │ Dr.Smith│ ← NEW ROW!
│ 2024 │ JAN   │ 20   │ ... │ Dr.Jones│
│      │       │      │ ... │         │
└──────────────────────────────────────┘
```

✅ **Success!** Your data is now in Google Sheets!

---

## Summary of What Happened

```
Your Google Sheet URL
		↓ (Extract ID)
SHEET_ID = 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
		↓
Google Apps Script Project (Created)
		↓ (Copy code)
Script Code (Pasted)
		↓ (Update Sheet ID)
Script Ready (Tested)
		↓ (Deploy)
Deployment URL = https://script.google.com/macros/d/1a2b3c4.../userweb
		↓
GoogleSheetsConfig.json (Updated)
  - deploymentUrl: https://script.google.com/macros/d/1a2b3c4.../userweb
  - spreadsheetId: 1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
  - sheetName: Sheet1
  - enabled: true
		↓
AlenkaAssistant (Built & Running)
		↓ (Create & Save)
Data Saved!
		↓
Google Sheet (Data Appears!)
```

---

## Congratulations! 🎉

You've successfully:
1. ✅ Created a Google Apps Script project
2. ✅ Deployed it as a Web App
3. ✅ Configured AlenkaAssistant
4. ✅ Saved data to Google Sheets

**You're all set!**

---

## What's Next?

- Create more purchase requests
- Watch data accumulate in Google Sheet
- Share the sheet with team members
- Generate reports from the data
- Celebrate your success! 🚀

---

**Total Time Spent:** ~15 minutes  
**Difficulty:** Easy  
**Result:** Working integration! ✅
