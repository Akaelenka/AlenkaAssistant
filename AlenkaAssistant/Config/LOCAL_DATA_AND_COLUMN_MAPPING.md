# Local Data Persistence & Configurable Column Mapping

## Overview
Two new features have been added to the AlenkaAssistant application:

1. **Local Data Persistence** – Save form data locally without sending to Google Sheets
2. **Configurable Column Mapping** – Adjust where data is saved in Google Sheets via configuration

---

## 1. Local Data Persistence

### What It Does
- **"Simpan Lokal" button** saves form data to a local JSON file on your computer
- **Auto-load on startup** – When you open the app, the last saved data automatically loads into the form
- **No internet required** – Works completely offline
- Data is stored at: `C:\Users\{YourUsername}\AppData\Local\AlenkaAssistant\Data\local_purchases.json`

### How to Use
1. Fill out the purchase request form
2. Click **"Simpan Lokal"** (Save Locally) to save to your computer
3. Close and reopen the app – your data will be pre-filled
4. Edit as needed and save again, or send to Google Sheets using **"Simpan ke Google Sheet"**

### Button Order (Bottom Right)
1. **Simpan Lokal** (Green) – Save to local computer
2. **Simpan ke Google Sheet** (Blue) – Send to Google Sheets
3. **Batal** (Red) – Cancel/Clear form

---

## 2. Configurable Column Mapping

### What It Does
The Google Sheets integration now uses a **configurable column mapping** system. Instead of hardcoding which columns store which data, you can adjust the mapping in `GoogleSheetsConfig.json`.

### How It Works
When you update your Google Sheet structure, simply edit the column numbers in the config file:

```json
{
  "deploymentUrl": "https://script.google.com/macros/s/YOUR_URL/exec",
  "spreadsheetId": "YOUR_SPREADSHEET_ID",
  "sheetName": "Sheet1",
  "enabled": true,
  "columnMapping": {
	"year": 1,
	"month": 2,
	"date": 3,
	"totalCost": 4,
	"costDetail": 5,
	"userId": 6,
	"treatmentDescription": 7,
	"treatmentType": 8,
	"assistantNames": 9,
	"doctorName": 10
  }
}
```

### Mapping Fields
- **year** – Tahun (Year)
- **month** – Bulan (Month in Indonesian)
- **date** – Tanggal (Date)
- **totalCost** – Total Biaya (Total Cost)
- **costDetail** – Detail Biaya (Cost Detail)
- **userId** – No. RM (Patient ID)
- **treatmentDescription** – Tindakan (Treatment Description)
- **treatmentType** – Tipe Tindakan (Treatment Type)
- **assistantNames** – Nama Asisten (Assistant Names)
- **doctorName** – Nama Dokter (Doctor Name)

### Adjusting Column Numbers
If your Google Sheet columns have moved, simply update the column numbers:

**Example: If you moved "Doctor Name" from column 10 to column 12:**
```json
"columnMapping": {
  // ... other fields ...
  "doctorName": 12  // Changed from 10 to 12
}
```

Save the config file, and the app will automatically use the new column positions next time you send data to Google Sheets.

---

## File Structure

### New Files Created
- `AlenkaAssistant/Services/LocalDataService.cs` – Handles local data save/load
- `AlenkaAssistant/Config/LOCAL_DATA_AND_COLUMN_MAPPING.md` – This documentation

### Modified Files
- `AlenkaAssistant/Config/GoogleSheetsConfig.json` – Added `columnMapping` section
- `AlenkaAssistant/Services/GoogleSheetsService.cs` – Now supports dynamic column mapping
- `AlenkaAssistant/ViewModels/PurchaseRequestViewModel.cs` – Added `SaveLocalCommand`
- `AlenkaAssistant/Views/PurchaseRequestView.xaml` – Updated button layout
- `AlenkaAssistant/Views/PurchaseRequestView.xaml.cs` – Added auto-load logic

---

## Technical Details

### LocalDataService Methods
- `SavePurchaseRequestAsync(PurchaseRequestModel)` – Save a purchase request
- `LoadAllPurchaseRequestsAsync()` – Load all saved records
- `LoadLastPurchaseRequestAsync()` – Load the most recent record (used on app startup)
- `LoadPurchaseRequestByIdAsync(int id)` – Load a specific record
- `DeletePurchaseRequestAsync(int id)` – Delete a record

### JSON Storage Format
Local data is stored in a JSON array:
```json
[
  {
	"id": 1234567890,
	"userId": "RM001",
	"createdAt": "2024-01-15T14:30:00",
	"generalTreatmentDesc": "Scaling",
	"treatmentType": 1,
	"totalCost": 500000,
	"assistantName": 0,
	"altAssistantName": ["Asisten A", "Asisten B"],
	"doctorName": 2,
	"altDoctorName": null,
	"costDetails": [
	  {"treatmentDesc": "Scaling", "cost": 500000}
	]
  }
]
```

---

## FAQ

**Q: Can I use both local save and Google Sheets save?**
A: Yes! You can save locally first, then later send the same data to Google Sheets. Both features work independently.

**Q: What if I change column positions in Google Sheets?**
A: Edit `GoogleSheetsConfig.json` and update the `columnMapping` values. No code changes needed!

**Q: Does local data sync across computers?**
A: No, local data is stored only on your computer. To share data, use Google Sheets save instead.

**Q: Can I delete old local saves?**
A: Currently, all saves are kept. You can manually delete the JSON file at:
`C:\Users\{YourUsername}\AppData\Local\AlenkaAssistant\Data\local_purchases.json`

**Q: What happens if I don't fill in all required fields?**
A: The "Simpan Lokal" button will be disabled (grayed out) until you fill in RM# and a date.

---

## Testing Checklist

- [ ] Fill form with test data
- [ ] Click "Simpan Lokal" – should see "✓ Successfully saved locally!"
- [ ] Close and reopen app – form should be pre-filled
- [ ] Edit data and save again – should update successfully
- [ ] Click "Simpan ke Google Sheet" – data should appear in Google Sheets
- [ ] Verify data is in correct columns (check `columnMapping` in config)
- [ ] Move a column in Google Sheets, update mapping in config, and test again
