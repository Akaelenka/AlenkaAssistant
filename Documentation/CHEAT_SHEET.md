# Quick Reference Cheat Sheet

## The Three Values You Need

### Value 1: Spreadsheet ID
**Where to find it:**
```
Your Google Sheet URL: 
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
										↑ Copy this part ↑
```

**Copy this:** `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c`

**Paste in config as:**
```json
"spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c"
```

---

### Value 2: Deployment URL
**Where to find it:**
1. Google Apps Script → Your Project
2. Click "Deploy" → "Manage deployments"
3. Copy the "Web app URL"

**Example:** `https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb`

**Key format:**
```
✅ Correct:  https://script.google.com/macros/d/ABC123.../userweb
❌ Wrong:    https://script.google.com/macros/s/ABC123.../userweb
❌ Wrong:    Just the ID without https://
```

**Paste in config as:**
```json
"deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb"
```

---

### Value 3: Sheet Name
**Where to find it:**
- Look at sheet tabs at bottom of your Google Sheet
- Usually: `Sheet1`
- But could be: `Data`, `Purchases`, etc.

**Important:** Case-sensitive!
```
✅ "Sheet1"      ← Correct (default)
❌ "sheet1"      ← Wrong (wrong case)
✅ "MyData"      ← Correct (if you renamed it)
❌ "My Data"     ← Wrong (extra space)
```

**Paste in config as:**
```json
"sheetName": "Sheet1"
```

---

## The Configuration File

**Location:** `AlenkaAssistant/Config/GoogleSheetsConfig.json`

**Template:**
```json
{
  "deploymentUrl": "PASTE_DEPLOYMENT_URL_HERE",
  "spreadsheetId": "PASTE_SHEET_ID_HERE",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**Filled Example:**
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb",
  "spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c",
  "sheetName": "Sheet1",
  "enabled": true
}
```

---

## Setup Steps (TLDR)

### Step 1: Google Apps Script Setup
```
1. Go to https://script.google.com
2. New project → Name it "AlenkaAssistant - Sheets Sync"
3. Copy code from GoogleAppsScript.js
4. Paste into editor
5. Update: const SPREADSHEET_ID = "YOUR_SHEET_ID"
6. Run "test" function to verify
7. Deploy as Web App (Anyone access)
8. Copy deployment URL
```

### Step 2: Configure AlenkaAssistant
```
1. Open GoogleSheetsConfig.json
2. Paste deployment URL
3. Paste sheet ID
4. Check sheet name
5. Set enabled: true
6. Save file
```

### Step 3: Test
```
1. Build project
2. Run app
3. Create purchase request
4. Click "Save to Google Sheets"
5. Check Google Sheet for new row
```

---

## Common Errors & Fixes

### Error: "Google Sheets is not enabled in config"
```
Fix: Change "enabled": false  →  "enabled": true
	 Save and rebuild
```

### Error: "DeploymentUrl is not configured"
```
Fix: Make sure you have the full URL, not just the ID
	 Should start with: https://script.google.com/macros/d/
```

### Error: "Sheet not found"
```
Fix: Check exact sheet name (case-sensitive!)
	 Look at sheet tabs at bottom of Google Sheet
	 Update "sheetName" in config to match exactly
```

### Error: "HTTP 403 Forbidden"
```
Fix: In Google Apps Script deployment settings:
	 Change "Who has access" to "Anyone"
	 Click Update
```

### Error: Data not appearing
```
Fix: 1. Verify you're using correct Google account
	 2. Check SPREADSHEET_ID in the script matches your sheet
	 3. Look at Google Apps Script execution logs for errors
	 4. Run the "test" function manually
```

---

## Verification Checklist

Before marking as complete:

- [ ] GoogleSheetsConfig.json has `deploymentUrl`
- [ ] GoogleSheetsConfig.json has `spreadsheetId`
- [ ] GoogleSheetsConfig.json has `sheetName`
- [ ] `enabled` is set to `true`
- [ ] No syntax errors in JSON file
- [ ] Google Apps Script deployed (not just code saved)
- [ ] App builds successfully
- [ ] Status message shows success (no errors)
- [ ] New row appears in Google Sheet
- [ ] Data is in the correct columns

---

## File Locations

```
Your Project Root:
D:\Applications\Alenka\AlenkaAssistant\

Config Files Location:
D:\Applications\Alenka\AlenkaAssistant\AlenkaAssistant\Config\

Key Files:
├── GoogleSheetsConfig.json         ← Edit this! ✏️
├── GoogleAppsScript.js             ← Deploy this 📤
├── QUICKSTART_GUIDE.md             ← Read this first! 📖
├── SETUP_SUMMARY.md
└── CHEAT_SHEET.md                  ← You are here
```

---

## URL Breakdown Reference

### Your Google Sheet URL
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit#gid=0
└────────┬──────────────────┘  ┌────────────────────────────────────┬──────────────────┐
		 │                     │                                    │
	  Domain          SPREADSHEET_ID (copy this!)             Other stuff (ignore)
```

**What to copy:** `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c`

### Your Deployment URL
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
└────────┬──────────────────┘  ┌─────────┬──────────────────────────────┬───┐
		 │                     │         │                              │
	  Domain              Key Mark    DEPLOYMENT_ID            Endpoint (must be /userweb)
						  (must be /d/  not /s/)
```

**What to copy:** entire URL `https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb`

---

## Commands You'll Use

### In Visual Studio (Build & Run)
```
Ctrl+Shift+B     Build Solution
F5               Start Debugging
Ctrl+S           Save File
```

### In Google Apps Script
```
Ctrl+S           Save Script
Ctrl+Enter       View Execution Logs
Ctrl+Enter       Run Current Function
```

---

## Data Format (Your Sheet Columns)

When data is saved to Google Sheets, it appears in this order:

| Col | Field | Example |
|-----|-------|---------|
| A | Year | 2024 |
| B | Month | JANUARI |
| C | Date | 15 |
| D | Total Cost | 1000000 |
| E | Cost Detail | 500000 |
| F | RM# | RM001 |
| G | Tindakan | Perawatan Gigi |
| H | Type | Rawat Jalan |
| I | Assistants | Dr. A / Dr. B |
| J | Doctor | Dr. Smith |

---

## How It Works (Simple Version)

```
You create a purchase request in AlenkaAssistant
					↓
You click "Save to Google Sheets"
					↓
App reads GoogleSheetsConfig.json
					↓
App sends HTTP POST to deployment URL with the data
					↓
Google Apps Script receives it
					↓
Script connects to Google Sheet using spreadsheetId
					↓
Script appends a new row with your data
					↓
New row appears in Google Sheet instantly!
```

---

## Need the Full Guides?

- **Quick Setup:** Read `QUICKSTART_GUIDE.md`
- **Visual Steps:** Read `VISUAL_WALKTHROUGH.md`
- **Configuration Help:** Read `CONFIGURATION_REFERENCE.md`
- **Troubleshooting:** Read `GOOGLE_APPS_SCRIPT_SETUP.md`
- **Overview:** Read `SETUP_SUMMARY.md`

---

## Copy & Paste Template

Use this template when setting up:

```
MY SPREADSHEET ID:
[Copy from Google Sheet URL]
_______________________________________________________

MY DEPLOYMENT URL:
[Copy from Google Apps Script]
_______________________________________________________

MY SHEET NAME:
[Check sheet tab at bottom]
_______________________________________________________

MY COMPLETED CONFIG:
{
  "deploymentUrl": "[Paste deployment URL here]",
  "spreadsheetId": "[Paste sheet ID here]",
  "sheetName": "[Paste sheet name here]",
  "enabled": true
}
```

---

## Success Indicators

You'll know it's working when:

1. ✅ Google Sheet opens with your data
2. ✅ New rows appear after clicking save
3. ✅ No error messages in the app
4. ✅ Status shows "successfully saved"
5. ✅ Multiple saves = multiple rows

---

## Emergency Troubleshooting

Not working? Follow this checklist in order:

1. **Is the app enabled?**
   - Check: `"enabled": true` in config
   - If false, change to true, save, rebuild

2. **Is the deployment URL correct?**
   - Check it starts with: `https://script.google.com/macros/d/`
   - Check it ends with: `/userweb`

3. **Is the sheet ID correct?**
   - Copy fresh from Google Sheet URL
   - Make sure it's just the ID (no slashes)

4. **Do the values match?**
   - Sheet ID in script = Sheet ID in config
   - Sheet name = actual sheet tab name

5. **Did you deploy the script?**
   - Just saving code = NOT deployed
   - Must click Deploy → New Deployment → Web App

6. **Is deployment set to "Anyone"?**
   - Google Apps Script → Deploy → Manage
   - Check "Who has access" = "Anyone"

---

**Got it? You're ready! Let's go! 🚀**
