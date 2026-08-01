# Configuration Examples and Common Setups

## Example 1: Basic Setup (Default)

**GoogleSheetsConfig.json:**
```json
{
  "deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb",
  "spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c",
  "sheetName": "Sheet1",
  "enabled": true
}
```

**Google Sheet URL:**
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
```

---

## How to Find Your Values

### Find Deployment URL
After deploying as Web App:
1. Google Apps Script → Your Project
2. Click "Deploy" → "Manage deployments"
3. Look for the URL in the "Active" section
4. It looks like: `https://script.google.com/macros/d/{ID}/userweb`

### Find Spreadsheet ID
From your Google Sheet:
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
										└─────────────────────────────────────┬──────────────────┘
														  SPREADSHEET_ID goes here
```

### Find Sheet Name
1. Look at the sheet tabs at the bottom of your Google Sheet
2. Default is "Sheet1"
3. If you renamed it, use the exact name (case-sensitive)
4. Example sheet names: "Data", "Purchases", "2024 Requests"

---

## Data Flow Diagram

```
┌─────────────────────────────────────────┐
│   AlenkaAssistant (Your .NET App)       │
│                                         │
│  PurchaseRequestViewModel               │
│  ├─ Creates PurchaseRequestModel        │
│  └─ Calls GoogleSheetsService           │
└────────────────────┬────────────────────┘
					 │
					 │ HTTP POST (JSON)
					 │ {"sheetName": "Sheet1", "values": [...]}
					 ▼
┌─────────────────────────────────────────┐
│   Google Apps Script (Your Deployment)  │
│                                         │
│  deploymentUrl (from config)            │
│  ├─ Receives POST request               │
│  ├─ Validates JSON payload              │
│  └─ Calls appendToSheet() function      │
└────────────────────┬────────────────────┘
					 │
					 │ Direct API Access
					 │ (using SPREADSHEET_ID)
					 ▼
┌─────────────────────────────────────────┐
│   Google Sheets (Your Spreadsheet)      │
│                                         │
│  SPREADSHEET_ID (from config)           │
│  ├─ Sheet1                              │
│  │  ├─ Column A: Year                   │
│  │  ├─ Column B: Month                  │
│  │  ├─ Column C: Date                   │
│  │  └─ ... (10 columns total)           │
│  └─ New data appended!                  │
└─────────────────────────────────────────┘
```

---

## Complete Configuration Reference

### GoogleSheetsConfig.json Fields

| Field | Type | Required | Description | Example |
|-------|------|----------|-------------|---------|
| `deploymentUrl` | string | Yes | Google Apps Script web app URL | `https://script.google.com/macros/d/ABC123/userweb` |
| `spreadsheetId` | string | Yes | Google Sheet ID (from URL) | `1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c` |
| `sheetName` | string | Yes | Sheet name (case-sensitive) | `Sheet1` or `Data` |
| `enabled` | boolean | Yes | Enable/disable this feature | `true` or `false` |

---

## Common Issues and Solutions

### Issue 1: Deployment URL Format Wrong

❌ **Wrong:**
```
https://docs.google.com/spreadsheets/d/ABC123/edit
https://script.google.com/macros/s/ABC123/userweb
```

✅ **Correct:**
```
https://script.google.com/macros/d/ABC123/userweb
```

**Note:** The key difference is `/d/` not `/s/`, and `/userweb` at the end.

---

### Issue 2: Spreadsheet ID Not Found

❌ **Wrong:**
```json
"spreadsheetId": "https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit"
```

✅ **Correct:**
```json
"spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c"
```

**Note:** Only the ID portion, not the full URL.

---

### Issue 3: Sheet Name Case Sensitivity

❌ **Wrong (if actual name is "Sheet1"):**
```json
"sheetName": "sheet1"
```

✅ **Correct:**
```json
"sheetName": "Sheet1"
```

---

## Testing Your Configuration

### Manual Test Steps

1. **Open GoogleSheetsConfig.json**
   ```json
   {
	 "deploymentUrl": "YOUR_DEPLOYMENT_URL",
	 "spreadsheetId": "YOUR_SHEET_ID",
	 "sheetName": "Sheet1",
	 "enabled": true
   }
   ```

2. **Verify each field:**
   - Does `deploymentUrl` start with `https://script.google.com/macros/d/`?
   - Does `spreadsheetId` look like a long alphanumeric string?
   - Does `sheetName` match a sheet in your Google Sheet?
   - Is `enabled` set to `true`?

3. **Build the application**
   ```
   Visual Studio → Build → Build Solution
   ```

4. **Run and test**
   - Create a purchase request
   - Click "Save to Google Sheets"
   - Check Google Sheet for new row

---

## Multi-Sheet Setup

If you have multiple Google Sheets to track:

### Option A: Multiple Configurations (Advanced)
Create separate config files:
- `GoogleSheetsConfig_Patients.json`
- `GoogleSheetsConfig_Purchases.json`
- `GoogleSheetsConfig_Invoices.json`

### Option B: One Config, Multiple Sheet Names
Keep one config but change `sheetName` based on context:

```csharp
// In your C# code
config.sheetName = "Purchases"; // or "Patients", "Invoices"
```

---

## URL Breakdown Example

Your Google Sheet URL:
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
```

Breaking it down:
```
https://docs.google.com/spreadsheets/d/
  ↑ Google Sheets domain

1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c
  ↑ This is your SPREADSHEET_ID
  Use this value!

/edit
  ↑ Just the view mode, ignore this part
```

For your config:
```json
"spreadsheetId": "1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c"
```

---

## Your Deployment URL Breakdown

After deploying as Web App, Google gives you:
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
```

Breaking it down:
```
https://script.google.com/macros/d/
  ↑ Google Apps Script domain, MUST be "/d/" not "/s/"

1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p
  ↑ Your DEPLOYMENT_ID
  Google generates this when you deploy

/userweb
  ↑ The endpoint type, must be exactly "/userweb"
```

For your config:
```json
"deploymentUrl": "https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb"
```

---

## Quick Reference Card

Print or bookmark this:

```
WHAT TO FIND                   WHERE TO FIND IT              WHERE IT GOES
───────────────────────────────────────────────────────────────────────────
Spreadsheet ID                 Google Sheet URL              "spreadsheetId"
							   (long string after /d/)

Sheet Name                     Sheet tabs (bottom)           "sheetName"
							   Usually "Sheet1"

Deployment URL                 Google Apps Script            "deploymentUrl"
							   Manage Deployments section

Enable Flag                    Always true for use           "enabled": true
```

---

## Validation Checklist

Before saving GoogleSheetsConfig.json:

```
□ deploymentUrl exists and is not empty
□ deploymentUrl starts with: https://script.google.com/macros/d/
□ deploymentUrl ends with: /userweb
□ spreadsheetId is not empty (no spaces, no slashes)
□ spreadsheetId looks like: ABC123...XYZ (long alphanumeric)
□ sheetName matches exactly (check case)
□ enabled is set to true
□ File is valid JSON (no syntax errors)
```

---

## Next Steps

1. ✅ Gather all three values (Deployment URL, Sheet ID, Sheet Name)
2. ✅ Create or update GoogleSheetsConfig.json
3. ✅ Build the AlenkaAssistant project
4. ✅ Run and test
5. ✅ Create a purchase request and submit
6. ✅ Verify data in Google Sheet

**You're all set! 🎉**
