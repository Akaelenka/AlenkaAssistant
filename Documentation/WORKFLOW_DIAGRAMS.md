# Patient Creation Feature - Visual Workflows

## Complete End-to-End Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                    ALENKAASSISTANT - ADD PATIENT                    │
└─────────────────────────────────────────────────────────────────────┘

STEP 1: USER CLICKS "TAMBAH PASIEN" BUTTON
┌──────────────────────────────┐
│  Purchase Request Form        │
│  ┌────────────────────────┐   │
│  │ No. RM: [          ]   │   │
│  │ [Tambah Pasien] <--- CLICK HERE
│  │ Pasien: [          ]   │   │
│  └────────────────────────┘   │
└──────────────────────────────┘
		 │
		 ▼
STEP 2: GET LAST RM FROM GOOGLE SHEETS
┌──────────────────────────────────────┐
│  C#: AddPatientService               │
│  └─ GetLastRmAsync()                 │
└──────────────────────────────────────┘
		 │
		 ▼
STEP 3: HTTP GET REQUEST TO GOOGLE APPS SCRIPT
┌─────────────────────────────────────────────┐
│  GET /exec?action=getLastRm&sheetName=NoRM  │
│  Deployment URL from GoogleSheetsConfig     │
└─────────────────────────────────────────────┘
		 │
		 ▼
STEP 4: GOOGLE APPS SCRIPT QUERIES NORMS SHEET
┌────────────────────────────────────────────────┐
│  Google Sheets (NoRM Sheet)                    │
│  ┌─────────────────────────────────────────┐  │
│  │ Row │ RM Column │ Patient Name Column   │  │
│  ├─────┼───────────┼───────────────────────┤  │
│  │ 1   │ A.0048    │ Patient A             │  │
│  │ 2   │ A.0049    │ Patient B             │  │
│  │ 3   │ A.0050    │ Patient C             │  │
│  │ ... │ ...       │ ...                   │  │
│  └─────┴───────────┴───────────────────────┘  │
│          ▲                                      │
│          │ Query: Find last non-empty RM      │
│          └─ Result: "A.0050"                   │
└────────────────────────────────────────────────┘
		 │
		 ▼
STEP 5: RESPONSE RETURNS TO C#
┌──────────────────────────────┐
│  { success: true,            │
│    lastRm: "A.0050",         │
│    lastRow: 51 }             │
└──────────────────────────────┘
		 │
		 ▼
STEP 6: DIALOG OPENS WITH SUGGESTION
┌─────────────────────────────────────┐
│          Add Patient Dialog         │
│  ┌─────────────────────────────┐    │
│  │ Nomor RM                    │    │
│  │ [A.0051]                    │    │ ← Auto-filled with next RM
│  │                             │    │
│  │ Nama Pasien                 │    │
│  │ [                      ]    │    │ ← User types here
│  │                             │    │
│  │ Catatan:                    │    │
│  │ Pasien akan disimpan saat   │    │
│  │ Anda mengirim atau print.   │    │
│  │                             │    │
│  │ [ Batal ]  [ Simpan ]       │    │
│  └─────────────────────────────┘    │
└─────────────────────────────────────┘
		 │
		 ▼
STEP 7: USER ENTERS PATIENT NAME
┌─────────────────────────────────────┐
│          Add Patient Dialog         │
│  ┌─────────────────────────────┐    │
│  │ Nomor RM                    │    │
│  │ [A.0051]                    │    │
│  │                             │    │
│  │ Nama Pasien                 │    │
│  │ [Nama Pasien Baru      ]    │    │ ← User enters name
│  │                             │    │
│  │ [ Batal ]  [ Simpan ]       │    │
│  └─────────────────────────────┘    │
└─────────────────────────────────────┘
		 │
		 ▼
STEP 8: USER CLICKS "SIMPAN"
		 │
		 ▼
STEP 9: DATA STORED IN LOCAL FORM
┌──────────────────────────────┐
│  Purchase Request Form        │
│  ┌────────────────────────┐   │
│  │ No. RM: [A.0051    ]   │   │ ← Auto-filled
│  │ Pasien: [Nama Pasien]  │   │ ← Auto-filled
│  │ ...                    │   │
│  │ [Kirim] [Print]        │   │
│  └────────────────────────┘   │
│                                │
│ ✓ Pasien baru: Nama Pasien    │
│   (A.0051) akan disimpan      │
│   saat mengirim.              │
└──────────────────────────────┘
		 │
		 ▼
STEP 10: USER FILLS REST OF FORM AND CLICKS "KIRIM" OR "PRINT"
		 │
		 ├─────────────────┬──────────────────┐
		 ▼                 ▼                  ▼
   [SEND PATH]       [PRINT PATH]    [FORM SUBMITTED]
		 │                 │                  │
		 ▼                 ▼                  │
  SaveToGoogleSheets  Print() Method         ▼
		 │                 │
		 ├─────────────────┴──────┐
		 │                        │
		 ▼                        ▼
   AddPatientAsync()        AddPatientAsync()
		 │                        │
		 ├────────────┬───────────┤
		 │            │           │
		 ▼            ▼           ▼
   POST to       POST to      Data not
   Apps Script   Apps Script  resaved
		 │            │
		 └────────────┴─────────────┐
				  │                  │
				  ▼                  ▼
		HTTP POST: action=addPatient
		{ rmNumber: "A.0051",
		  patientName: "Nama Pasien",
		  ... }
				  │
				  ▼
		┌──────────────────────────────┐
		│ Google Apps Script           │
		│ addPatientToSheet()          │
		│                              │
		│ Adds row to NoRM sheet:      │
		│ Row 52: A.0051 | Nama Pasien│
		│                              │
		│ Returns: { success: true }   │
		└──────────────────────────────┘
				  │
				  ▼
		┌──────────────────────────────┐
		│ NoRM Sheet Updated           │
		│ ┌──────────────────────────┐ │
		│ │ RM Column │ Patient Col  │ │
		│ ├───────────┼─────────────┤ │
		│ │ A.0048    │ Patient A   │ │
		│ │ A.0049    │ Patient B   │ │
		│ │ A.0050    │ Patient C   │ │
		│ │ A.0051    │ Nama Pasien │ │ ← NEW!
		│ └──────────────────────────┘ │
		└──────────────────────────────┘
				  │
				  ▼
		Continue with normal flow:
		- Send to Google Sheets, or
		- Show Print Preview
```

---

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                      AlenkaAssistant App                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │         PurchaseRequestView (XAML)                       │  │
│  │  ┌─────────────────────────────────────────────────────┐ │  │
│  │  │ No. RM: [TextBox]        [Tambah Pasien Button]    │ │  │
│  │  │ Pasien: [TextBox]                                  │ │  │
│  │  │ ... rest of form ...                               │ │  │
│  │  └─────────────────────────────────────────────────────┘ │  │
│  └────────────────────┬──────────────────────────────────────┘  │
│                       │ (Binds to)                               │
│  ┌────────────────────▼──────────────────────────────────────┐  │
│  │      PurchaseRequestViewModel                            │  │
│  │  ┌──────────────────────────────────────────────────┐    │  │
│  │  │ AddPatientCommand                                │    │  │
│  │  │  └─ ShowAddPatientDialog()                      │    │  │
│  │  │      └─ Creates AddPatientDialog                │    │  │
│  │  │      └─ Creates AddPatientDialogViewModel       │    │  │
│  │  │      └─ Sets DataContext                        │    │  │
│  │  │      └─ Shows Dialog                            │    │  │
│  │  │                                                  │    │  │
│  │  │ SaveToGoogleSheetsAsync()                       │    │  │
│  │  │  └─ Calls AddPatientService.AddPatientAsync()  │    │  │
│  │  │  └─ Calls GoogleSheetsService.AppendAsync()    │    │  │
│  │  │                                                  │    │  │
│  │  │ Print()                                          │    │  │
│  │  │  └─ Calls AddPatientService.AddPatientAsync()  │    │  │
│  │  │  └─ Calls PrintService.GenerateFlowDocument()  │    │  │
│  │  └──────────────────────────────────────────────────┘    │  │
│  └─┬───────────────────────┬──────────────────────┬──────────┘  │
│    │                       │                      │              │
│    ▼                       ▼                      ▼              │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐    │
│  │ AddPatient      │  │ AddPatient      │  │ Google       │    │
│  │ Dialog          │  │ Service         │  │ Sheets       │    │
│  │ (XAML)          │  │                 │  │ Service      │    │
│  │                 │  │ GetLastRmAsync()│  │              │    │
│  │ [RM]            │  │ AddPatientAsync │  │ Append Rows  │    │
│  │ [PatientName]   │  │ Increment       │  │              │    │
│  │ [Simpan][Batal] │  │                 │  │              │    │
│  └────────┬────────┘  └────────┬────────┘  └──────────────┘    │
│           │                    │                                 │
│  ┌────────▼──────────┐         │                                 │
│  │ AddPatient        │         │                                 │
│  │ DialogViewModel   │         │                                 │
│  │                   │         │                                 │
│  │ LoadLastRmAsync() │─────────┘                                 │
│  │ Confirm()         │                                           │
│  │ Cancel()          │                                           │
│  └───────────────────┘                                           │
│                                                                  │
└──────────────────────────────┬───────────────────────────────────┘
							   │
					(HTTP Requests via HttpClient)
							   │
				┌──────────────┴──────────────┐
				│                             │
				▼                             ▼
		 ┌─────────────────┐          ┌─────────────────┐
		 │   Google Apps   │          │   Google Apps   │
		 │   Script        │          │   Script        │
		 │                 │          │                 │
		 │ GET: getLastRm()│          │ POST: addPatient│
		 │                 │          │                 │
		 │ Queries:        │          │ Adds rows to:   │
		 │ NoRM Sheet      │          │ NoRM Sheet      │
		 │                 │          │                 │
		 │ Returns: RM     │          │ Returns: Result │
		 └────────┬────────┘          └────────┬────────┘
				  │                           │
				  └───────────────┬───────────┘
								  │
								  ▼
					   ┌──────────────────────┐
					   │   Google Sheets      │
					   │                      │
					   │ [NoRM Sheet]         │
					   │ ┌──────────────────┐ │
					   │ │ RM │ Patient Name │ │
					   │ ├────┼─────────────┤ │
					   │ │... │ ...         │ │
					   │ │ NEW│ NEW PATIENT │ │
					   │ └──────────────────┘ │
					   └──────────────────────┘
```

---

## Data Flow - Get Last RM

```
┌─────────────────────────────────────────────────────────────┐
│                     GET LAST RM FLOW                        │
└─────────────────────────────────────────────────────────────┘

User clicks "Tambah Pasien"
		 │
		 ▼
ShowAddPatientDialog()
		 │
		 ▼
new AddPatientDialog()
		 │
		 ▼
new AddPatientDialogViewModel(dialog, _addPatientService)
		 │
		 ▼
AddPatientDialogViewModel.__init__()
		 │
		 ▼
LoadLastRmAsync()
		 │
		 ▼
_addPatientService.GetLastRmAsync()
		 │
		 ▼
HttpClient.GetAsync(
  $"{_deploymentUrl}?action=getLastRm&" +
  $"sheetName={_patientLookupSheetName}&" +
  $"searchColumn={_rmColumn}"
)
		 │
		 ▼
Google Apps Script: doGet()
		 │
		 ├─ Check action parameter = "getLastRm"
		 │
		 ▼
getLastRmFromSheet(sheetName, rmColumn)
		 │
		 ├─ Open spreadsheet by SPREADSHEET_ID
		 ├─ Get sheet by name (NoRM)
		 ├─ Read all values from RM column
		 ├─ Scan from bottom to find last non-empty RM
		 │
		 ▼
Return JSON response
{
  success: true,
  lastRm: "A.0050",
  lastRow: 51
}
		 │
		 ▼
Back to C#: ParseJson()
		 │
		 ▼
UpdateCanConfirm()
		 │
		 ▼
RmSuggestion = "A.0051" (next sequential)
		 │
		 ▼
Dialog displays with suggestion
```

---

## Data Flow - Add Patient

```
┌─────────────────────────────────────────────────────────────┐
│                    ADD PATIENT FLOW                         │
└─────────────────────────────────────────────────────────────┘

User clicks "Kirim ke Google Sheets" or "Print"
		 │
		 ▼
SaveToGoogleSheetsAsync() OR Print()
		 │
		 ▼
BuildPurchaseRequestModel()
		 │
		 ├─ Read form values:
		 │  - Uid (RM number)
		 │  - PatientName
		 │
		 ▼
_addPatientService.AddPatientAsync(
  Uid,      // e.g., "A.0051"
  PatientName
)
		 │
		 ▼
Create JSON payload:
{
  action: "addPatient",
  sheetName: _patientLookupSheetName,  // "NoRM"
  rmNumber: rmNumber,                   // "A.0051"
  patientName: patientName,             // "Nama Pasien"
  rmColumn: _rmColumn,                  // 0
  patientNameColumn: _patientNameColumn // 1
}
		 │
		 ▼
HttpClient.PostAsync(
  deploymentUrl,
  StringContent(json, Encoding.UTF8, "application/json")
)
		 │
		 ▼
Google Apps Script: doPost()
		 │
		 ├─ Parse payload
		 ├─ Check action = "addPatient"
		 │
		 ▼
addPatientToSheet(
  sheetName,
  rmNumber,
  patientName,
  rmColumn,
  patientNameColumn
)
		 │
		 ├─ Get sheet from spreadsheet
		 ├─ Validate data
		 ├─ Find last row
		 ├─ Create new row with values:
		 │  Column 0: rmNumber
		 │  Column 1: patientName
		 │
		 ▼
sheet.getRange(newRow, 1, 1, maxColumns).setValues([newValues])
		 │
		 ▼
Return JSON response:
{
  success: true,
  message: "Patient added successfully",
  newRow: 52,
  rmNumber: "A.0051",
  patientName: "Nama Pasien"
}
		 │
		 ▼
Back to C#: ParseJson()
		 │
		 ├─ Check if success
		 ├─ Continue with:
		 │  - GoogleSheetsService.AppendAsync() if Send was clicked
		 │  - PrintService.GenerateFlowDocument() if Print was clicked
		 │
		 ▼
Transaction completed
Patient is now in NoRM sheet
```

---

## Error Handling Flow

```
Any operation can fail at several points:

1. Network Error
   User clicks Tambah Pasien
   ├─ No internet connection
   └─ Error: "Failed to load last RM"
	  Dialog shows error message

2. Google Apps Script Error
   getLastRm() called
   ├─ Sheet not found
   ├─ SPREADSHEET_ID incorrect
   └─ Error: "Sheet not found: NoRM"
	  Dialog shows error message

3. Data Validation Error
   User tries to save
   ├─ Patient name is empty
   └─ Error: "Patient name required"
	  Simpan button is disabled

4. Add Patient Error
   addPatient() called
   ├─ Sheet permissions issue
   ├─ Invalid sheet name
   ├─ Missing data
   └─ Error: "Failed to add patient"
	  User sees error in status bar

All errors are logged for debugging.
Check Google Apps Script Execution logs:
  script.google.com > Executions
```

---

## State Machine - Dialog

```
					┌─────────┐
					│  START  │
					└────┬────┘
						 │
						 ▼
			  ┌──────────────────────┐
			  │  Load Last RM        │
			  │  (IsLoading = true)  │
			  └──────────┬───────────┘
						 │
		 ┌───────────────┼───────────────┐
		 │               │               │
		 ▼               ▼               ▼
	[SUCCESS]      [ERROR]         [TIMEOUT]
		 │               │               │
		 ▼               ▼               ▼
   RM loaded      Error shown     Default RM
		 │               │               │
		 └───────────────┼───────────────┘
						 │
						 ▼
			  ┌──────────────────────┐
			  │  Dialog Ready        │
			  │  CanConfirm = false  │ ◄─ Wait for user input
			  └────┬─────────────────┘
				   │
	 ┌─────────────┼─────────────┐
	 │             │             │
	 ▼             ▼             ▼
  [USER       [USER        [USER CLOSES]
   ENTERS     LEAVES
   DATA]      EMPTY]
	 │             │             │
	 ▼             ▼             ▼
  Validate   CanConfirm   Dialog Closed
  Data       = false      (No Change)
	 │             │             │
	 └─────────────┼─────────────┘
				   │
		 ┌─────────▼─────────┐
		 │                   │
		 ▼                   ▼
	[VALID]            [INVALID]
		 │                   │
		 ▼                   ▼
 CanConfirm =         CanConfirm =
 true                 false
 Simpan Enable        Simpan Disable
		 │                   │
		 └─────────────┬─────┘
					   │
					   ▼
		 ┌─────────────────────────┐
		 │  User Clicks Simpan     │
		 │  OR Batal               │
		 └────┬────────────────────┘
			  │
			  ├─────────────────┐
			  │                 │
			  ▼                 ▼
		[CONFIRM]          [CANCEL]
			  │                 │
			  ▼                 ▼
	  Write values         Don't write
	  Set Confirmed = true Set Confirmed = false
	  Close Dialog         Close Dialog
			  │                 │
			  └────────┬────────┘
					   │
					   ▼
				  ┌──────────┐
				  │   END    │
				  └──────────┘
```

---

These diagrams illustrate the complete architecture and flow of the patient creation feature. For implementation details, refer to the documentation files.
