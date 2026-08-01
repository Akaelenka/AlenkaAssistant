# 🔧 Troubleshooting Guide

Common issues and their solutions for the Patient Creation Feature.

---

## Table of Contents

1. [Deployment Issues](#deployment-issues)
2. [Feature Issues](#feature-issues)
3. [Google Sheets Issues](#google-sheets-issues)
4. [Google Apps Script Issues](#google-apps-script-issues)
5. [Windows/Build Issues](#windowsbuild-issues)
6. [Performance Issues](#performance-issues)

---

## Deployment Issues

### "Unknown action" Error in Browser

**Symptom**:
```
URL returns: {"success": false, "error": "Unknown action"}
```

**Causes**:
- Google Apps Script wasn't updated with new code
- Old code is still cached
- SPREADSHEET_ID not set correctly

**Solutions**:

1. **Verify code was updated**:
   - Go to https://script.google.com
   - Open your project
   - Check that the file contains both `getLastRm` and `addPatient` functions
   - If not, copy-paste the new code again

2. **Clear browser cache**:
   - Press Ctrl+Shift+Delete
   - Clear cache for the past hour
   - Try the URL again

3. **Verify SPREADSHEET_ID**:
   - In Google Apps Script, check line: `const SPREADSHEET_ID = "YOUR_ID"`
   - Copy your Google Sheet URL
   - The ID is between `/spreadsheets/d/` and `/edit`
   - Make sure it matches

4. **Wait for deployment**:
   - Sometimes changes take 30-60 seconds to propagate
   - Try the URL again after waiting

---

### "Sheet not found" Error

**Symptom**:
```json
{"success": false, "error": "Sheet not found: NoRM"}
```

**Causes**:
- NoRM sheet doesn't exist
- Sheet name is misspelled
- Sheet name is case-sensitive

**Solutions**:

1. **Verify sheet exists**:
   - Open your Google Sheet
   - Look for a sheet tab named "NoRM"
   - Create it if it doesn't exist

2. **Check spelling**:
   - Exact name required (case-sensitive)
   - "NoRM" ≠ "nORM" ≠ "nOrm"
   - Check GoogleSheetsConfig.json for exact name

3. **Check configuration**:
   ```json
   "patientLookup": {
	 "sheetName": "NoRM"   ← Must match sheet name exactly
   }
   ```

4. **Recreate sheet if needed**:
   - Right-click sheet tab
   - Select "Delete"
   - Create new sheet
   - Name it exactly "NoRM"
   - Add headers if desired

---

## Feature Issues

### "Tambah Pasien" Button Not Visible

**Symptom**:
- No orange button appears next to "No. RM" field

**Causes**:
- AlenkaAssistant not rebuilt after code changes
- XAML changes not loaded
- Wrong build configuration

**Solutions**:

1. **Rebuild solution**:
   ```
   Build → Rebuild Solution (Ctrl+Shift+B)
   ```

2. **Clean and rebuild**:
   ```
   Build → Clean Solution
   Then: Build → Rebuild Solution
   ```

3. **Verify files were added**:
   - Check project includes: AddPatientDialog.xaml
   - Check project includes: AddPatientService.cs
   - Check project includes: AddPatientDialogViewModel.cs

4. **Run app again**:
   - Close AlenkaAssistant completely
   - Run AlenkaAssistant.exe
   - Button should now be visible

---

### Dialog Opens But Doesn't Load RM

**Symptom**:
- "Tambah Pasien" button works
- Dialog opens
- But RM suggestion never loads
- Loading indicator keeps spinning

**Causes**:
- Network connectivity issue
- Google Apps Script deployment URL wrong
- Script returning error

**Solutions**:

1. **Check deployment URL**:
   - Open GoogleSheetsConfig.json
   - Find: `"deploymentUrl": "https://script.google.com/..."`
   - Make sure it's not empty or "YOUR_URL"

2. **Test in browser**:
   - Copy deployment URL into browser
   - Add: `?action=getLastRm`
   - Should return JSON with RM data

3. **Check network**:
   - Verify internet connection
   - Check corporate firewall (if applicable)
   - Try from different network

4. **Check Google Apps Script logs**:
   - Go to https://script.google.com
   - Open your project
   - Click "Executions"
   - Look for recent runs
   - Check for error messages

---

### Patient Data Not Saving

**Symptom**:
- Dialog closes correctly
- Form fills with RM and name
- Click "Kirim" or "Print"
- But patient doesn't appear in NoRM sheet

**Causes**:
- Google Apps Script `addPatient` action failed
- Sheet permissions issue
- NoRM sheet is read-only

**Solutions**:

1. **Check Google Apps Script logs**:
   - Go to script.google.com > Executions
   - Look for failed executions (red X)
   - Click to see error details

2. **Check sheet permissions**:
   - Open your Google Sheet
   - Click "Share"
   - Verify your account has "Editor" access
   - Verify Google Apps Script account has access

3. **Try manual test**:
   - Go to sheet.google.com
   - Manually add a row to NoRM sheet
   - Verify you can add data
   - If not, permissions are the issue

4. **Check column indices**:
   - Verify GoogleSheetsConfig.json:
	 ```json
	 "rmColumn": 0,
	 "patientNameColumn": 1
	 ```
   - Column 0 = A, Column 1 = B
   - Make sure this matches your sheet layout

---

### Dialog Validation Error

**Symptom**:
- Can't click "Simpan" button
- Button is disabled or grayed out

**Causes**:
- Patient name field is empty
- RM number field is empty
- Both required before saving

**Solutions**:

1. **Enter patient name**:
   - Click "Nama Pasien" field
   - Type the patient name
   - Button should become enabled

2. **Enter RM number** (if needed):
   - Some dialogs require RM to be set
   - If field is empty, click and enter RM

3. **Check for error message**:
   - Dialog should show error below fields
   - Follow the error message instructions

---

## Google Sheets Issues

### NoRM Sheet Data Structure

**Symptom**:
- Unsure if NoRM sheet is set up correctly

**Expected Structure**:
```
Row 1 (Headers - optional):
Column A: RM
Column B: Patient Name

Row 2+:
A2: A.0001
B2: Patient Name 1

A3: A.0002
B3: Patient Name 2
```

**Verify your sheet**:
1. Open Google Sheet
2. Go to NoRM sheet tab
3. Check that:
   - Column A contains RM numbers
   - Column B contains patient names
   - Data starts at Row 2 (Row 1 can be headers)

**Fix if wrong**:
- Rearrange columns to match above
- Delete extra columns if any
- Update GoogleSheetsConfig.json if columns differ

---

### Duplicate Patients in NoRM Sheet

**Symptom**:
- Same patient appears multiple times
- Last RM not incrementing correctly

**Causes**:
- Feature called multiple times
- Manual duplicates added

**Prevention**:
1. Ensure you're using the Tambah Pasien button
2. Only click once per patient
3. Wait for confirmation message

**Cleanup**:
1. Open NoRM sheet
2. Find duplicate rows
3. Delete manually
4. Verify last RM is correct

---

## Google Apps Script Issues

### Logs Show Errors

**Symptom**:
- script.google.com > Executions shows red X (failed)

**How to read logs**:
1. Open https://script.google.com
2. Go to Executions (left menu)
3. Click on failed execution (red X)
4. Scroll through to find error message

**Common errors**:
- `"Sheet not found"` → Check sheet name
- `"Cannot read property"` → Check column indices
- `"Permission denied"` → Check sharing settings

---

### Apps Script Runs But No Data Appears

**Symptom**:
- Logs show "success: true"
- But data doesn't appear in sheet

**Causes**:
- Data written to wrong sheet
- Column indices wrong
- Cache not refreshed

**Solutions**:

1. **Refresh Google Sheet**:
   - Close tab
   - Reopen
   - Data should appear

2. **Check row was added**:
   - Go to sheet
   - Scroll to very bottom
   - New row should be there

3. **Verify column indices**:
   - Check GoogleSheetsConfig.json again
   - Count columns from 0
   - Column A = 0, B = 1, etc.

---

## Windows/Build Issues

### "AlenkaAssistant.dll" File Blocked Error

**Symptom**:
```
Application Control Error 0x800711C7
Failed to load DLL
```

**Cause**:
- Windows security marked DLL as "from internet"

**Solution**:

1. **Quick fix**:
   - Run PowerShell as Administrator
   - Navigate to: `D:\Applications\Alenka\AlenkaAssistant`
   - Run: `.\QuickFix-0x800711C7.ps1`

2. **Manual fix**:
   - Right-click DLL file
   - Select "Properties"
   - Click "Unblock" button
   - Click "OK"

3. **Comprehensive fix**:
   - Go to folder: `bin\Release\net10.0-windows`
   - Select all DLLs
   - Right-click > Properties > Unblock > OK

---

### Build Fails or Won't Compile

**Symptom**:
- Build → Rebuild Solution shows errors

**Solutions**:

1. **Check for file conflicts**:
   - Close AlenkaAssistant
   - Clean solution (Build > Clean Solution)
   - Wait 10 seconds
   - Rebuild (Build > Rebuild Solution)

2. **Check file paths**:
   - Verify AddPatientService.cs exists
   - Verify AddPatientDialog.xaml exists
   - Both should be in project

3. **Reset build cache**:
   - Delete bin and obj folders
   - Clean solution
   - Rebuild solution

---

## Performance Issues

### Dialog Loads Very Slowly

**Symptom**:
- Click "Tambah Pasien"
- 5+ second delay before RM appears

**Causes**:
- Network latency
- Google Sheets API slow
- Large NoRM sheet

**Solutions**:

1. **Check network**:
   - Test internet speed
   - Switch to faster network if possible
   - Check VPN (disable if using)

2. **Check sheet size**:
   - NoRM sheet with 100,000+ rows will be slow
   - Consider archiving old data
   - Split into multiple sheets if needed

3. **Wait for caching**:
   - App caches RM for short time
   - Subsequent dialogs will be faster

---

### App Freezes When Adding Patient

**Symptom**:
- Click "Simpan"
- App becomes unresponsive
- "Not Responding" message

**Causes**:
- Network timeout
- Google Sheets API issue
- Large data transfer

**Solutions**:

1. **Wait longer** (5-10 seconds):
   - App may just be processing
   - Don't force-close

2. **Check network connection**:
   - Verify still connected to internet
   - Try again

3. **Restart app**:
   - If truly frozen, close with Task Manager
   - Try again

4. **Reduce form data**:
   - Large detail entries can slow things down
   - Keep descriptions reasonable length

---

## Still Having Issues?

### Debug Information to Gather

Before contacting support, gather:
1. Error message (exact text)
2. Google Apps Script execution log entry
3. Browser console error (F12 > Console)
4. Time when issue occurred
5. Steps to reproduce

### Where to Look

**Google Apps Script Logs**:
- https://script.google.com > Executions
- Most recent entries at top

**Application Logs**:
- Windows Event Viewer
- Applications section
- Filter for "AlenkaAssistant"

**Browser Console**:
- F12 key
- Console tab
- Any red error messages

---

## Quick Reference: Most Common Issues

| Issue | Solution | Time |
|-------|----------|------|
| Button not showing | Rebuild solution | 2 min |
| RM won't load | Check sheet name matches | 1 min |
| Patient won't save | Check Google Sheets permissions | 2 min |
| "Unknown action" | Re-upload Google Apps Script | 5 min |
| DLL blocked | Run unblock script | 1 min |

---

**Still stuck?** Check the main documentation:
- `README_PATIENT_CREATION.md` - Feature overview
- `API_REFERENCE.md` - API details
- `DEPLOYMENT_GUIDE.md` - Deployment steps
