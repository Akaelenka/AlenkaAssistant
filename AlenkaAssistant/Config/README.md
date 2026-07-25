# Google Sheets Integration Documentation

Welcome! This directory contains everything you need to set up Google Sheets integration for AlenkaAssistant.

## 📖 Quick Navigation

### 🚀 Getting Started (START HERE!)

1. **[QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md)** ⭐ **START HERE**
   - Step-by-step setup guide
   - Clear instructions for each step
   - Takes about 15 minutes

2. **[CHEAT_SHEET.md](CHEAT_SHEET.md)**
   - Quick reference card
   - Common errors and fixes
   - Copy-paste templates

3. **[SETUP_SUMMARY.md](SETUP_SUMMARY.md)**
   - High-level overview
   - What you need to know
   - File checklist

### 📚 Detailed Guides

4. **[VISUAL_WALKTHROUGH.md](VISUAL_WALKTHROUGH.md)**
   - Detailed step-by-step with visual descriptions
   - Shows where to find each value
   - Best for visual learners

5. **[CONFIGURATION_REFERENCE.md](CONFIGURATION_REFERENCE.md)**
   - Complete configuration reference
   - Common issues and solutions
   - URL format examples
   - Multi-sheet setup

6. **[GOOGLE_APPS_SCRIPT_SETUP.md](GOOGLE_APPS_SCRIPT_SETUP.md)**
   - Technical setup details
   - Troubleshooting guide
   - Advanced options

### ⚙️ Configuration Files

7. **[GoogleSheetsConfig.json](GoogleSheetsConfig.json)** ✏️ **EDIT THIS FILE**
   - Your configuration file
   - Fill in 3 values:
	 - deploymentUrl (from Google Apps Script)
	 - spreadsheetId (from your Google Sheet URL)
	 - sheetName (sheet tab name)
	 - enabled (true to enable)

8. **[GoogleAppsScript.js](GoogleAppsScript.js)**
   - Google Apps Script code
   - Copy this entire file into Google Apps Script editor
   - Deploy as Web App
   - DO NOT EDIT (use as-is)

---

## ⏱️ Time Required

| Task | Time |
|------|------|
| Deploy Google Apps Script | 5-10 min |
| Configure AlenkaAssistant | 2-3 min |
| Test Integration | 2-3 min |
| **TOTAL** | **~15 minutes** |

---

## 🎯 The Mission

You want to automatically save purchase requests from AlenkaAssistant to a Google Sheet.

## ✅ The Solution

1. Deploy a Google Apps Script (acts as a bridge)
2. Configure AlenkaAssistant with 2 URLs/IDs
3. Done! Data saves automatically

---

## 📋 The Three Values You Need

### 1. Spreadsheet ID
**Where to get it:** Your Google Sheet URL
```
https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMXT3PJiEkVQZGzGkL8JzIgnKPZCH3c/edit
										└─────────────────────────────────┬─────────────────┘
										COPY THIS (it's your Spreadsheet ID)
```

### 2. Deployment URL  
**Where to get it:** Google Apps Script deployment
```
https://script.google.com/macros/d/1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p/userweb
(Full URL from "Manage deployments")
```

### 3. Sheet Name
**Where to get it:** Sheet tabs in your Google Sheet
```
Look at the tabs at the bottom: Sheet1, Data, Purchases, etc.
(Usually "Sheet1" by default)
```

---

## 🚦 Setup Process

### Phase 1: Deploy Google Apps Script
```
script.google.com → New Project → Copy Code → Paste → Configure → Deploy
```

### Phase 2: Configure AlenkaAssistant
```
Edit GoogleSheetsConfig.json → Fill in 3 values → Save → Build → Test
```

### Phase 3: Verify
```
Run App → Create Request → Save → Check Google Sheet → Success!
```

---

## 📚 Documentation Map

```
AlenkaAssistant/Config/
│
├── README.md (you are here) ← Overview
│
├── 🚀 GETTING STARTED
│   ├── QUICKSTART_GUIDE.md ⭐ START HERE
│   ├── CHEAT_SHEET.md (quick reference)
│   └── SETUP_SUMMARY.md (overview)
│
├── 📚 DETAILED GUIDES
│   ├── VISUAL_WALKTHROUGH.md (step-by-step with visuals)
│   ├── CONFIGURATION_REFERENCE.md (technical details)
│   └── GOOGLE_APPS_SCRIPT_SETUP.md (setup & troubleshooting)
│
├── ⚙️ IMPLEMENTATION FILES
│   ├── GoogleSheetsConfig.json ← EDIT THIS
│   └── GoogleAppsScript.js ← COPY TO GOOGLE
│
└── 📄 THIS FILE (README.md)
```

---

## ❓ FAQ

**Q: Which file should I read first?**
A: Start with **[QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md)** - it's the most straightforward.

**Q: What do I actually need to do?**
A: 
1. Deploy the Google Apps Script (follow QUICKSTART_GUIDE.md)
2. Fill in GoogleSheetsConfig.json with 3 values
3. Done!

**Q: Will I have to code anything?**
A: No! Everything is pre-built. Just configure and deploy.

**Q: How long does this take?**
A: About 15 minutes total.

**Q: What if something goes wrong?**
A: Check the **Troubleshooting** section in any of the guide files, especially:
- [CHEAT_SHEET.md](CHEAT_SHEET.md) for quick fixes
- [GOOGLE_APPS_SCRIPT_SETUP.md](GOOGLE_APPS_SCRIPT_SETUP.md) for detailed troubleshooting

**Q: Can I use multiple Google Sheets?**
A: Yes! Create separate GoogleSheetsConfig files or deployments.

**Q: Is this secure?**
A: Yes, you control everything. For production, you can restrict access.

---

## 🔄 Data Flow

```
AlenkaAssistant
	  ↓ (HTTP POST)
Google Apps Script (your deployment)
	  ↓ (uses your IDs)
Your Google Sheet
	  ↓ (new data appears!)
New Row in Google Sheet
```

---

## 📝 What Gets Saved

When you create a purchase request, these columns are saved:

| Column | Content |
|--------|---------|
| A | Year |
| B | Month |
| C | Date |
| D | Total Cost |
| E | Cost Detail |
| F | RM# |
| G | Treatment Description |
| H | Treatment Type |
| I | Assistant Names |
| J | Doctor Name |

---

## ✨ Features

✅ Automatic data saving  
✅ No credentials file needed  
✅ Simple configuration  
✅ Real-time updates  
✅ Easy to troubleshoot  
✅ Can scale to multiple sheets  

---

## 🛠️ Troubleshooting Quick Links

- **Config not working?** → [CHEAT_SHEET.md#common-errors--fixes](CHEAT_SHEET.md)
- **Deployment failed?** → [GOOGLE_APPS_SCRIPT_SETUP.md#troubleshooting](GOOGLE_APPS_SCRIPT_SETUP.md)
- **Data not appearing?** → [CONFIGURATION_REFERENCE.md#common-issues](CONFIGURATION_REFERENCE.md)
- **URL format wrong?** → [CONFIGURATION_REFERENCE.md#url-breakdown-example](CONFIGURATION_REFERENCE.md)

---

## 📋 Pre-Setup Checklist

Before you start, make sure you have:

- [ ] A Google account
- [ ] A Google Sheet where you want to store data
- [ ] Your Google Sheet URL (from the browser address bar)
- [ ] Access to AlenkaAssistant source code
- [ ] Visual Studio installed

---

## 🎓 Learning Path

**Beginner? Start here:**
1. Read [QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md) (10 min)
2. Follow the steps
3. Done!

**Need more details?**
1. [VISUAL_WALKTHROUGH.md](VISUAL_WALKTHROUGH.md) - See exactly where to click
2. [CONFIGURATION_REFERENCE.md](CONFIGURATION_REFERENCE.md) - Understand the config

**Experienced? Quick reference:**
1. [CHEAT_SHEET.md](CHEAT_SHEET.md) - Copy-paste templates
2. [GoogleAppsScript.js](GoogleAppsScript.js) - Deploy code

**Troubleshooting?**
1. [GOOGLE_APPS_SCRIPT_SETUP.md](GOOGLE_APPS_SCRIPT_SETUP.md) - Full troubleshooting guide

---

## 🚀 Next Steps

1. **Open [QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md)**
2. **Follow steps 1-9**
3. **Come back here if you have questions**

---

## 📞 Quick Help

### "I don't know where to start"
→ Read [QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md)

### "I'm stuck on a step"
→ Check [VISUAL_WALKTHROUGH.md](VISUAL_WALKTHROUGH.md)

### "Something's not working"
→ Check [GOOGLE_APPS_SCRIPT_SETUP.md](GOOGLE_APPS_SCRIPT_SETUP.md#troubleshooting)

### "I need the reference"
→ Use [CHEAT_SHEET.md](CHEAT_SHEET.md)

### "I need technical details"
→ Read [CONFIGURATION_REFERENCE.md](CONFIGURATION_REFERENCE.md)

---

## 📚 All Files in This Directory

| File | Purpose | Read When |
|------|---------|-----------|
| **QUICKSTART_GUIDE.md** | Main setup guide | Starting setup |
| **CHEAT_SHEET.md** | Quick reference | Need quick answer |
| **SETUP_SUMMARY.md** | High-level overview | Want overview first |
| **VISUAL_WALKTHROUGH.md** | Step-by-step visual | Prefer step-by-step |
| **CONFIGURATION_REFERENCE.md** | Technical reference | Need details |
| **GOOGLE_APPS_SCRIPT_SETUP.md** | Setup & troubleshooting | Setup problems |
| **GoogleSheetsConfig.json** | Configuration file | After setup |
| **GoogleAppsScript.js** | Script code | Deploying |
| **README.md** | This file | Navigation |

---

## ✅ Success Criteria

You'll know you're done when:
1. ✅ GoogleSheetsConfig.json is filled with your values
2. ✅ Google Apps Script is deployed
3. ✅ AlenkaAssistant app runs without errors
4. ✅ New data appears in Google Sheet after saving
5. ✅ Multiple purchases = multiple rows in sheet

---

**Ready to get started? → Open [QUICKSTART_GUIDE.md](QUICKSTART_GUIDE.md)** 🚀

**Questions? → Check this README or the other guides!** 📚
