# 🚀 Deployment Guide - Patient Creation Feature

## Quick Summary

This guide walks you through deploying the new **Tambah Pasien** (Add Patient) feature to your Google Sheets integration.

**Total Time**: 20-30 minutes  
**Difficulty**: Easy  
**Prerequisites**: Access to Google Apps Script project, GitHub repository

---

## What You're Deploying

### C# Application ✅
- Already updated and compiled
- 4 new files created
- 3 existing files enhanced
- Build status: SUCCESSFUL
- **No action needed** - ready to use

### Google Apps Script 🔧
- Updated with 2 new actions
- Ready to copy-paste
- Full backward compatibility
- **Action needed** - see steps below

### Database/Sheets 📊
- No changes to Google Sheets structure
- Uses existing NoRM sheet
- Automatic updates only
- **No action needed**

---

## Step-by-Step Deployment

### Step 1: Prepare (2 minutes)

```
1. Go to: https://script.google.com
2. Sign in with your Google account
3. Open your AlenkaAssistant deployment project
4. You're ready for Step 2
```

If you don't know which project, check your GoogleSheetsConfig.json for the deployment URL and open that project.

---

### Optional: Separate NoRM Spreadsheet (Advanced)

If you want to keep patient data (NoRM sheet) in a separate Google Sheet from your main purchase requests:

1. **Create a new Google Sheet** for NoRM data (or use an existing one)
2. **Add one sheet named "NoRM"** with columns: RM Number (column A), Patient Name (column B)
3. **Get the Spreadsheet ID** from the URL
4. **Update GoogleSheetsConfig.json:**
   ```json
   {
     "deploymentUrl": "YOUR_DEPLOYMENT_URL",
     "spreadsheetId": "YOUR_MAIN_SPREADSHEET_ID",
     "noRmSpreadsheetId": "YOUR_NORM_SPREADSHEET_ID",
     ...
   }
   ```
5. **Save and restart** the application

If `noRmSpreadsheetId` is not set or is empty, the app will use the main `spreadsheetId` for both purposes (keeping everything in one sheet).

---

### Step 2: Copy the Google Apps Script Code (2 minutes)

1. In this Documentation folder, find: `GOOGLE_APPS_SCRIPT_SETUP.md`
2. OR directly use the code from: `AlenkaAssistant/Config/GoogleAppsScript.js`
3. Copy **ALL** the code (Ctrl+A to select all in the file)

---

### Step 3: Update Your Google Apps Script (3 minutes)

In your Google Apps Script project editor:

1. **Select all existing code**
   ```
   Press: Ctrl+A
   ```

2. **Delete it**
   ```
   Press: Delete
   ```

3. **Paste new code**
   ```
   Paste the code you copied
   Press: Ctrl+Shift+V (for paste without formatting if needed)
   ```

4. **Save**
   ```
   Press: Ctrl+S
   Wait for "Saved" confirmation
   ```

✅ **Your Google Apps Script is now updated!**

---

### Step 4: Verify Deployment (2 minutes)

Test that the new actions are working:

**Test 1: Get Last RM**

1. Open your browser
2. Go to this URL (replace YOUR_DEPLOYMENT_URL):
   ```
   https://YOUR_DEPLOYMENT_URL/exec?action=getLastRm&sheetName=NoRM&searchColumn=0
   ```

3. You should see JSON like:
   ```json
   { "success": true, "lastRm": "A.0050", "lastRow": 51 }
   ```

✅ If you see this, deployment worked!

**Having trouble?** See Step 5 below.

---

### Step 5: Test in AlenkaAssistant (5-10 minutes)

Now test the actual feature:

1. **Run AlenkaAssistant**
   ```
   AlenkaAssistant.exe
   ```

2. **Look for "Tambah Pasien" button**
   - Should appear in orange next to "No. RM" field
   - If not visible, rebuild the solution

3. **Click the button**
   - Dialog should open
   - It should load the last RM from Google Sheets
   - Should suggest next sequential RM (e.g., "A.0051")

4. **Test the workflow**
   - Type a patient name
   - Click "Simpan"
   - Form should fill with RM and name
   - See confirmation message

5. **Complete the entry**
   - Fill out the rest of the form
   - Click "Kirim ke Google Sheets"
   - Patient should be saved

6. **Verify in Google Sheets**
   - Go to your Google Sheet
   - Open the NoRM sheet
   - Scroll to the bottom
   - **New patient should appear!**

✅ If you see the patient in the sheet, it worked!

---

## Troubleshooting

### Issue: "Unknown action" error in browser

**Solution**:
1. Make sure you completely replaced the old code
2. Make sure SPREADSHEET_ID is set correctly
3. Try the URL again - sometimes there's a caching issue

---

### Issue: Dialog opens but doesn't load RM

**Possible causes**:
- NoRM sheet doesn't exist in your Google Sheet
- Sheet name is spelled differently
- Column indices are wrong

**Solution**:
1. Check GoogleSheetsConfig.json for patientLookup settings:
   ```json
   "patientLookup": {
	 "sheetName": "NoRM",
	 "rmColumn": 0,
	 "patientNameColumn": 1
   }
   ```
2. Verify NoRM sheet exists in your Google Sheet
3. Verify the sheet name matches exactly (case-sensitive)

---

### Issue: "Tambah Pasien" button not visible

**Possible causes**:
- AlenkaAssistant wasn't rebuilt after code changes

**Solution**:
1. Rebuild the solution (Build → Rebuild Solution)
2. Run AlenkaAssistant again

---

### Issue: Patient not saved to NoRM sheet

**Possible causes**:
- Google Apps Script deployment didn't update
- Permissions issue with Google Sheet
- Wrong sheet name in config

**Solution**:
1. Check that you saved the Google Apps Script (Ctrl+S)
2. Verify the NoRM sheet has write permissions
3. Check Google Apps Script execution logs:
   - Go to script.google.com > Executions
   - Look for error messages
4. Try redeploying the script

---

### Issue: Getting "Sheet not found" error

**Solution**:
1. Check that your NoRM sheet exists
2. Check the exact spelling of the sheet name
3. Make sure GoogleSheetsConfig.json has the correct sheet name:
   ```json
   "patientLookup": {
	 "sheetName": "NoRM"  ← Verify this matches your sheet
   }
   ```

---

## Testing Complete Workflow

Once deployment is verified, run through this complete test:

1. ✅ Click "Tambah Pasien"
2. ✅ Dialog loads with suggested RM
3. ✅ Enter patient name
4. ✅ Click "Simpan"
5. ✅ Form fills with data
6. ✅ Fill rest of form
7. ✅ Click "Kirim ke Google Sheets"
8. ✅ Check NoRM sheet - patient appears
9. ✅ Check main sheet - transaction appears

If all 9 steps work, deployment is **SUCCESSFUL**! ✅

---

## Rollback (If Needed)

If something goes wrong and you need to revert:

1. Go to: https://script.google.com
2. Open your project
3. Replace with old code from: `AlenkaAssistant/Config/GoogleAppsScript_Fixed.js`
4. Save and test

**Time to rollback**: 2-3 minutes

---

## Next Steps

1. ✅ Complete this deployment
2. ✅ Run through all 9 workflow tests above
3. ✅ Monitor for issues
4. ✅ Get user feedback

For detailed testing procedures, see: `TESTING_CHECKLIST.md`

---

## Support & Reference

**Need more details?**
- Full API documentation: `API_REFERENCE.md`
- Visual architecture: `WORKFLOW_DIAGRAMS.md`
- Testing guide: `TESTING_CHECKLIST.md`
- Quick reference: `CHEAT_SHEET.md`

**Questions?**
- See `README_PATIENT_CREATION.md` for complete feature overview
- Check `TROUBLESHOOTING.md` for common issues

---

## Checklist

- [ ] Google Apps Script project opened
- [ ] Old code selected and deleted
- [ ] New code pasted
- [ ] Changes saved (Ctrl+S)
- [ ] Verified in browser with getLastRm URL
- [ ] Tested "Tambah Pasien" button
- [ ] Patient data entered and saved
- [ ] Patient appears in NoRM sheet
- [ ] All 9 workflow tests passed
- [ ] Ready for production use

**✅ Deployment Complete!**

Enjoy your new feature! 🎉
