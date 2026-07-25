# AlenkaAssistant Google Sheets Integration - Complete Setup Summary

## Quick Answer: How to Use Google Sheets URL

### The 3-Step Summary

You need three pieces of information to configure your AlenkaAssistant:

#### 1️⃣ Your Google Sheet URL
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
```

#### 2️⃣ Extract the Sheet ID (from the URL)
```
1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c  ← This part
												  (goes in config)
```

#### 3️⃣ Get the Deployment URL (from Google Apps Script)
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
																   (also goes in config)
```

---

## Complete Workflow

### Phase 1: Setup Google Apps Script (5 minutes)

```
STEP 1: Visit https://script.google.com
		   ↓
STEP 2: Create new project named "AlenkaAssistant - Sheets Sync"
		   ↓
STEP 3: Copy code from GoogleAppsScript.js file
		   ↓
STEP 4: Paste into Google Apps Script editor
		   ↓
STEP 5: Update SPREADSHEET_ID with your Sheet ID
		   ↓
STEP 6: Test by running the "test" function
		   ↓
STEP 7: Deploy as Web App ("Anyone" access)
		   ↓
STEP 8: Copy deployment URL
		   ↓
Result: You now have the DEPLOYMENT_URL
```

### Phase 2: Configure AlenkaAssistant (2 minutes)

```
STEP 1: Open GoogleSheetsConfig.json
		   ↓
STEP 2: Paste deployment URL into "deploymentUrl" field
		   ↓
STEP 3: Paste sheet ID into "spreadsheetId" field
		   ↓
STEP 4: Verify "sheetName" matches your sheet tab name
		   ↓
STEP 5: Change "enabled" from false to true
		   ↓
STEP 6: Save the file
		   ↓
Result: Configuration complete!
```

### Phase 3: Test Integration (1 minute)

```
STEP 1: Build the project (Visual Studio)
		   ↓
STEP 2: Run the application
		   ↓
STEP 3: Create a purchase request
		   ↓
STEP 4: Click "Save to Google Sheets"
		   ↓
STEP 5: Check Google Sheet for new row
		   ↓
Result: Data should appear in Google Sheet!
```

---

## The Configuration File Explained

### GoogleSheetsConfig.json

**Location:** `AlenkaAssistant/Config/GoogleSheetsConfig.json`

**Format:**
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/YOUR_DEPLOYMENT_ID/userweb",
  "spreadsheetId": "YOUR_SHEET_ID_HERE",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**What Each Field Means:**

| Field | Meaning | Example | Where to Get |
|-------|---------|---------|--------------|
| `deploymentUrl` | URL of your Google Apps Script web app | `https://script.google.com/macros/d/1a2b3c4.../userweb` | Google Apps Script → Deploy → Manage Deployments |
| `spreadsheetId` | ID of your Google Sheet | `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c` | Your Google Sheet URL (the long string) |
| `sheetName` | Name of the sheet tab to write to | `Sheet1` or `Data` | Sheet tabs at bottom of Google Sheet |
| `enabled` | Turn this feature on/off | `true` (on) or `false` (off) | Your choice |

---

## How Data Flows

```
┌──────────────────────────────────┐
│  Your AlenkaAssistant App        │
│  (runs on your computer)         │
│                                  │
│  Creates purchase request        │
│  Clicks "Save to Google Sheets"  │
└────────────┬─────────────────────┘
			 │
			 │ Sends HTTP POST request
			 │ (using deploymentUrl)
			 ↓
┌────────────────────────────────────────┐
│  Google Apps Script (your deployment)  │
│  (runs on Google's servers)            │
│                                        │
│  Receives the data                     │
│  Connects to your Google Sheet         │
│  (using spreadsheetId)                 │
└────────────┬─────────────────────────────┘
			 │
			 │ Appends data
			 ↓
┌──────────────────────────────────┐
│  Your Google Sheet               │
│  (on Google Drive)               │
│                                  │
│  New row appears!                │
│  Year │ Month │ Date │ Cost ...  │
│ 2024 │ JAN   │  15  │ 100000...  │
└──────────────────────────────────┘
```

---

## Files Provided

### 📄 Configuration Files

1. **GoogleSheetsConfig.json**
   - Your main configuration file
   - Edit this with your deployment URL and sheet ID
   - Location: `AlenkaAssistant/Config/GoogleSheetsConfig.json`

2. **GoogleAppsScript.js**
   - The script code to deploy on Google
   - Copy this entire file into Google Apps Script
   - Location: `AlenkaAssistant/Config/GoogleAppsScript.js`

### 📚 Documentation Files

1. **QUICKSTART_GUIDE.md** ← START HERE!
   - Step-by-step setup with screenshots descriptions
   - Easiest way to get started

2. **VISUAL_WALKTHROUGH.md**
   - Detailed visual guide with diagrams
   - Shows exactly where to find values

3. **CONFIGURATION_REFERENCE.md**
   - Complete configuration reference
   - Common issues and fixes
   - URL format examples

4. **GOOGLE_APPS_SCRIPT_SETUP.md**
   - Technical details
   - Troubleshooting guide
   - Advanced options

---

## Common Questions & Answers

### Q1: Where do I find my Google Sheet ID?

**A:** Open your Google Sheet in a browser. Look at the URL:

```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
										└────────────────────────────────────┬──────────────┘
														  SHEET ID goes here
```

Just the ID: `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c`

---

### Q2: Where do I get the Deployment URL?

**A:** 
1. Go to [script.google.com](https://script.google.com)
2. Open your "AlenkaAssistant - Sheets Sync" project
3. Click "Deploy" → "Manage deployments"
4. Find the active deployment
5. Copy the URL (it shows as "Web app URL")

Example: `https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb`

---

### Q3: What's the difference between Sheet ID and Deployment URL?

**A:**
- **Sheet ID**: Identifies YOUR GOOGLE SHEET (where data is stored)
  - Format: Long alphanumeric string like `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c`
  - Source: Your Google Sheet URL

- **Deployment URL**: Identifies YOUR GOOGLE APPS SCRIPT (the program that saves data)
  - Format: Full URL like `https://script.google.com/macros/d/1a2b3c4.../userweb`
  - Source: Google Apps Script deployment

Both are needed!

---

### Q4: How do I know if it's working?

**A:** You'll see these signs:
- ✅ App shows: "Purchase request saved to Google Sheets successfully!"
- ✅ New row appears in your Google Sheet with the data
- ✅ No error messages in the application

---

### Q5: Can I use multiple Google Sheets?

**A:** Yes! You can:
- Create separate config files for each sheet
- Or create separate deployments (one per sheet)
- Or update the sheetName field dynamically in code

---

### Q6: What if I get a "Sheet not found" error?

**A:** 
1. Open your Google Sheet
2. Look at the sheet tabs at the bottom
3. Check the exact name (it's case-sensitive!)
4. Update `sheetName` in GoogleSheetsConfig.json to match exactly

Examples:
- `"sheetName": "Sheet1"` (default)
- `"sheetName": "Data"` (if you renamed it)
- `"sheetName": "Purchases 2024"` (with spaces)

---

### Q7: What if I get a "403 Forbidden" error?

**A:** 
1. Go to Google Apps Script
2. Click "Deploy" → "Manage deployments"
3. Click the pencil/edit icon
4. Change "Who has access" to "Anyone"
5. Click "Update"

---

### Q8: Do I need to code anything?

**A:** No! The system is pre-built. You just need to:
1. Deploy the Google Apps Script (copy-paste + click deploy)
2. Fill in 3 values in the config file
3. Done!

---

## Step-by-Step Checklist

Print this and check off as you go:

```
SETUP PHASE
☐ I have my Google Sheet URL
☐ I extracted the Sheet ID from the URL
☐ I went to script.google.com
☐ I created a new project
☐ I copied the GoogleAppsScript.js code
☐ I pasted it into the script editor
☐ I updated the SPREADSHEET_ID with my Sheet ID
☐ I tested the script (ran the "test" function)
☐ I deployed it as a Web App
☐ I copied the deployment URL
☐ I verified "Who has access" is set to "Anyone"

CONFIGURATION PHASE
☐ I opened GoogleSheetsConfig.json
☐ I pasted my deployment URL into "deploymentUrl"
☐ I pasted my sheet ID into "spreadsheetId"
☐ I verified "sheetName" matches my sheet tab name
☐ I changed "enabled" to true
☐ I saved the file

TESTING PHASE
☐ I built the project
☐ I ran the application
☐ I created a purchase request
☐ I clicked "Save to Google Sheets"
☐ I checked my Google Sheet for new data
☐ Data appeared successfully!

VERIFICATION
☐ The application shows success message
☐ New rows appear in Google Sheet
☐ Data is in the correct columns
☐ No error messages appear
☐ Everything is working!
```

Once all items are checked, you're done! 🎉

---

## Quick Reference Card

Save this for future reference:

```
MY SETUP DETAILS
════════════════════════════════════════════════════════

Google Sheet ID:
1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c

Google Sheet URL:
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit

Deployment URL:
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb

Sheet Name:
Sheet1

Config File Location:
AlenkaAssistant/Config/GoogleSheetsConfig.json

Google Apps Script Project Name:
AlenkaAssistant - Sheets Sync
════════════════════════════════════════════════════════
```

---

## Getting Help

If something doesn't work:

1. **Check the Troubleshooting section** in one of the guide files
2. **Review the Google Apps Script execution logs:**
   - Google Apps Script editor
   - View → Logs
   - Look for error messages
3. **Verify all three values** (Deployment URL, Sheet ID, Sheet Name)
4. **Test the script manually:**
   - Google Apps Script editor
   - Select "test" function
   - Click Run
   - Check logs

---

## Files You Need to Edit

### Only 1 File to Edit:
📝 **GoogleSheetsConfig.json** (in `AlenkaAssistant/Config/`)

Just fill in these 4 fields:
1. `deploymentUrl` - Your Google Apps Script URL
2. `spreadsheetId` - Your Google Sheet ID
3. `sheetName` - Name of your sheet tab
4. `enabled` - Set to `true`

That's it!

---

## Documentation Structure

```
AlenkaAssistant/Config/
├── GoogleSheetsConfig.json           ← Edit this!
├── GoogleAppsScript.js               ← Copy to Google
├── QUICKSTART_GUIDE.md               ← Read this first! ⭐
├── VISUAL_WALKTHROUGH.md             ← Detailed steps with diagrams
├── CONFIGURATION_REFERENCE.md        ← Technical details & examples
├── GOOGLE_APPS_SCRIPT_SETUP.md       ← Setup & troubleshooting
└── SETUP_SUMMARY.md                  ← This file (overview)
```

---

## You're Ready! 🚀

The hardest part is done (the code setup). Now it's just:
1. Deploy the script (10 minutes)
2. Fill in the config (2 minutes)
3. Test (1 minute)

**Total time: ~15 minutes**

Then you can start saving all your purchase requests to Google Sheets automatically!

---

**Questions?** Check the documentation files - they have detailed answers!

**Ready to start?** Begin with **QUICKSTART_GUIDE.md** ← Start here!
