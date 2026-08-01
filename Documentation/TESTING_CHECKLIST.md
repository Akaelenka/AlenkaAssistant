# Testing Checklist - Patient Creation Feature

## Pre-Deployment Testing (in Google Apps Script)

Before deploying the updated script to your live Google Sheet, test these actions locally:

### Test 1: Get Last RM (Empty Sheet)
```javascript
// In Google Apps Script console, run:
const result = getLastRmFromSheet("NoRM", 0);
Logger.log(JSON.stringify(result));

// Expected result:
// { "success": true, "lastRm": null, "lastRow": 1, "message": "No data in sheet, start with A.0001" }
```

### Test 2: Get Last RM (With Data)
```javascript
// After adding some test data to NoRM sheet:
// Row 1: A.0001 | John Doe
// Row 2: A.0002 | Jane Smith
// Row 3: A.0003 | Bob Wilson

// Run:
const result = getLastRmFromSheet("NoRM", 0);
Logger.log(JSON.stringify(result));

// Expected result:
// { "success": true, "lastRm": "A.0003", "lastRow": 4 }
```

### Test 3: Add Patient (New Row)
```javascript
// Run:
const result = addPatientToSheet("NoRM", "A.0004", "Alice Brown", 0, 1);
Logger.log(JSON.stringify(result));

// Expected result:
// { "success": true, "message": "Patient added successfully", "newRow": 4, "rmNumber": "A.0004", "patientName": "Alice Brown" }

// Verify: Check the NoRM sheet - new row should be added
```

---

## Live Testing (after updating Google Apps Script)

### Test 4: Browser Test - Get Last RM via HTTP

1. Copy your deployment URL
2. Open this in your browser (replace `YOUR_DEPLOYMENT_URL`):
```
https://YOUR_DEPLOYMENT_URL/exec?action=getLastRm&sheetName=NoRM&searchColumn=0
```

3. You should see JSON response:
```json
{ "success": true, "lastRm": "A.0003", "lastRow": 4 }
```

**Expected Result**: ✅ JSON response with last RM value

---

### Test 5: AlenkaAssistant - Tambah Pasien Button

1. Run AlenkaAssistant.exe
2. Look for "Tambah Pasien" button next to "No. RM" field
3. Click the button
4. A dialog should open with:
   - "Nomor RM" label with a TextBox showing suggested RM (e.g., "A.0004")
   - "Nama Pasien" TextBox (empty)
   - Loading indicator while fetching
   - "Simpan" and "Batal" buttons

**Expected Result**: ✅ Dialog opens and loads last RM correctly

---

### Test 6: AlenkaAssistant - Enter New Patient

1. With dialog open, you should see suggested RM (e.g., "A.0004")
2. Type a patient name in "Nama Pasien" field (e.g., "Test Pasien Baru")
3. Click "Simpan" button
4. Dialog should close
5. The form should now show:
   - No. RM: "A.0004"
   - Patient Name: "Test Pasien Baru"
6. A status message should appear: "✓ Pasien baru: Test Pasien Baru (A.0004) akan disimpan saat mengirim."

**Expected Result**: ✅ Dialog closes with patient data populated

---

### Test 7: AlenkaAssistant - Save to Google Sheets

1. With the new patient data still on the form, fill in the rest of the form as needed
2. Click "Kirim ke Google Sheets" button
3. Wait for confirmation message
4. No error should appear

**Expected Result**: ✅ Form submits successfully

---

### Test 8: Verify Google Sheets Updated

1. Go to your Google Sheet
2. Check the NoRM sheet
3. Scroll to the bottom
4. **New row should be added** with:
   - Column A (RM): "A.0004"
   - Column B (Patient Name): "Test Pasien Baru"

**Expected Result**: ✅ New patient appears in NoRM sheet

---

### Test 9: AlenkaAssistant - Print with New Patient

1. Run AlenkaAssistant again
2. Click "Tambah Pasien"
3. Select a different RM (should be "A.0005")
4. Enter a different patient name
5. Fill in the rest of the form
6. Click "Print" button
7. Print preview should open

**Expected Result**: ✅ Print preview shows correctly

---

### Test 10: Verify Google Sheets Again

1. Go to your Google Sheet
2. Check the NoRM sheet
3. **Another new row should be added** with the new patient

**Expected Result**: ✅ Second patient appears in NoRM sheet

---

## Error Testing

### Test 11: Invalid Sheet Name
```
https://YOUR_DEPLOYMENT_URL/exec?action=getLastRm&sheetName=InvalidSheet&searchColumn=0
```

**Expected Result**: 
```json
{ "success": false, "error": "Sheet not found: InvalidSheet" }
```

### Test 12: Missing Patient Name
1. In dialog, leave "Nama Pasien" empty
2. Click "Simpan"
3. Button should be disabled or show error

**Expected Result**: ✅ Cannot save with empty patient name

### Test 13: Cancel Dialog
1. Click "Tambah Pasien"
2. Click "Batal" button
3. Dialog should close without changes

**Expected Result**: ✅ Dialog closes without populating form

---

## Performance Testing

### Test 14: Load Time
- Opening the "Tambah Pasien" dialog should load in < 2 seconds
- Google Sheets submission should complete in < 5 seconds

**Expected Result**: ✅ Responsive and fast

---

## Regression Testing

### Test 15: Old Functionality Still Works
1. Regular patient lookup by RM (type RM, patient name auto-fills)
2. Normal Google Sheets submission (without using Tambah Pasien)
3. Print functionality (without using Tambah Pasien)

**Expected Result**: ✅ All existing features work as before

---

## Troubleshooting Guide

| Issue | Solution |
|-------|----------|
| "getLastRm" returns error | Verify NoRM sheet exists and SPREADSHEET_ID is correct |
| Dialog won't open | Check browser console for errors; verify deployment URL in config |
| RM not incrementing correctly | Check NoRM sheet has proper RM format (e.g., "A.0001") |
| Patient not saved to sheet | Check Google Apps Script logs; verify sheet write permissions |
| No RM suggestion in dialog | Verify last RM query returned valid data |

---

## Sign-Off Checklist

- [ ] All 15 tests passed
- [ ] No errors in Google Apps Script logs
- [ ] No errors in AlenkaAssistant
- [ ] New patients appear correctly in NoRM sheet
- [ ] Old functionality still works
- [ ] Load times are acceptable
- [ ] Ready for production use

---

## Rollback Plan

If something goes wrong:

1. Go to https://script.google.com
2. Replace code with previous version from: `AlenkaAssistant/Config/GoogleAppsScript_Fixed.js`
3. Save and test with an existing patient lookup
4. Report the issue with error details from Google Apps Script logs

---

## Documentation References

- Full API details: `GOOGLE_APPS_SCRIPT_NEW_FEATURES.md`
- Quick deploy guide: `GOOGLE_APPS_SCRIPT_DEPLOY_QUICK.md`
- Update summary: `GOOGLE_APPS_SCRIPT_UPDATE_SUMMARY.md`
