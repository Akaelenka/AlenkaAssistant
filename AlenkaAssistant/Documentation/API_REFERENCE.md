# 📡 API Reference - Google Apps Script Endpoints

Complete API documentation for the Patient Creation Feature.

---

## Overview

Two new endpoints support the patient creation workflow:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `getLastRm` | GET | Retrieve last RM from NoRM sheet |
| `addPatient` | POST | Add new patient to NoRM sheet |

---

## GET: `getLastRm`

### Purpose
Fetch the last patient record number (RM) from the NoRM sheet to suggest the next sequential RM.

### Request Format
```
GET /exec?action=getLastRm&sheetName=NoRM&searchColumn=0
```

### Parameters
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `action` | string | - | Must be: `getLastRm` |
| `sheetName` | string | "NoRM" | Sheet name to query |
| `searchColumn` | number | 0 | Column index containing RMs (0-based) |

### Example Requests

**Basic (using defaults)**
```
https://your-deployment-url/exec?action=getLastRm
```

**With custom sheet**
```
https://your-deployment-url/exec?action=getLastRm&sheetName=PatientRegistry&searchColumn=0
```

### Success Response
```json
{
  "success": true,
  "lastRm": "A.0050",
  "lastRow": 51
}
```

**Fields**:
- `success` (boolean): Always true on success
- `lastRm` (string): The last RM found (e.g., "A.0050")
- `lastRow` (number): Row number where last RM is located

### Empty Sheet Response
```json
{
  "success": true,
  "lastRm": null,
  "lastRow": 1,
  "message": "No data in sheet, start with A.0001"
}
```

### Error Response
```json
{
  "success": false,
  "error": "Sheet not found: NoRM"
}
```

**Common Errors**:
- `"Sheet not found: {name}"` - Sheet doesn't exist
- `"RM column is empty"` - No RM values in column

### Usage in C#
```csharp
// Service makes the request
var response = await httpClient.GetAsync(
  $"{deploymentUrl}?action=getLastRm&sheetName=NoRM&searchColumn=0"
);
var json = await response.Content.ReadAsStringAsync();
var result = JsonSerializer.Deserialize<GetLastRmResponse>(json);

if (result.success && result.lastRm != null)
{
  var nextRm = IncrementRmNumber(result.lastRm);
  // Use nextRm in dialog
}
```

---

## POST: `addPatient`

### Purpose
Add a new patient record to the NoRM sheet with RM number and patient name.

### Request Format
```
POST /exec
Content-Type: application/json

{
  "action": "addPatient",
  "sheetName": "NoRM",
  "rmNumber": "A.0051",
  "patientName": "Patient Name",
  "rmColumn": 0,
  "patientNameColumn": 1
}
```

### Request Body Parameters
| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `action` | string | - | Yes | Must be: `addPatient` |
| `sheetName` | string | "NoRM" | No | Sheet to save to |
| `rmNumber` | string | - | **Yes** | RM number to save |
| `patientName` | string | - | **Yes** | Patient name to save |
| `rmColumn` | number | 0 | No | Column index for RM (0-based) |
| `patientNameColumn` | number | 1 | No | Column index for name (0-based) |

### Example Requests

**Basic**
```json
{
  "action": "addPatient",
  "rmNumber": "A.0051",
  "patientName": "Nama Pasien Baru"
}
```

**With custom columns**
```json
{
  "action": "addPatient",
  "sheetName": "PatientRegistry",
  "rmNumber": "P001",
  "patientName": "John Doe",
  "rmColumn": 1,
  "patientNameColumn": 2
}
```

### Success Response
```json
{
  "success": true,
  "message": "Patient added successfully",
  "newRow": 52,
  "rmNumber": "A.0051",
  "patientName": "Nama Pasien Baru"
}
```

**Fields**:
- `success` (boolean): Always true on success
- `message` (string): Confirmation message
- `newRow` (number): Row number where patient was added
- `rmNumber` (string): The RM that was saved
- `patientName` (string): The patient name that was saved

### Error Response
```json
{
  "success": false,
  "error": "Sheet not found: NoRM"
}
```

**Common Errors**:
- `"Sheet not found: {name}"` - Sheet doesn't exist
- `"RM number and patient name are required"` - Missing data
- `"Error in addPatientToSheet: ..."` - Other Google Sheets API error

### Usage in C#
```csharp
var payload = new {
	action = "addPatient",
	sheetName = "NoRM",
	rmNumber = "A.0051",
	patientName = "Patient Name",
	rmColumn = 0,
	patientNameColumn = 1
};

var json = JsonSerializer.Serialize(payload);
var content = new StringContent(json, Encoding.UTF8, "application/json");
var response = await httpClient.PostAsync(deploymentUrl, content);
var responseText = await response.Content.ReadAsStringAsync();
var result = JsonSerializer.Deserialize<AddPatientResponse>(responseText);

if (result.success)
{
	Console.WriteLine($"Patient added at row {result.newRow}");
}
```

---

## Configuration

Both endpoints use configuration from `GoogleSheetsConfig.json`:

```json
{
  "deploymentUrl": "https://script.google.com/macros/s/...",
  "patientLookup": {
	"sheetName": "NoRM",
	"rmColumn": 0,
	"patientNameColumn": 1
  }
}
```

**Default Values**:
- `sheetName`: "NoRM"
- `rmColumn`: 0 (Column A)
- `patientNameColumn`: 1 (Column B)

---

## Error Handling

### Network Errors
```
Exception: HttpRequestException
Message: "No connection to server"
Action: Retry with exponential backoff, show user message
```

### Validation Errors
```json
{ "success": false, "error": "RM number and patient name are required" }
```
**Action**: Show error to user, prompt for missing data

### Sheet Errors
```json
{ "success": false, "error": "Sheet not found: NoRM" }
```
**Action**: Log error, notify administrator

### Permission Errors
```json
{ "success": false, "error": "Insufficient permissions" }
```
**Action**: Check Google Sheets sharing settings

---

## Rate Limiting

No built-in rate limiting. Each request is independent.

**Recommendations**:
- Cache last RM result for 5 minutes
- Batch add patients if possible
- Implement client-side debouncing

---

## Testing Endpoints

### Using Browser (getLastRm only)
```
https://your-deployment-url/exec?action=getLastRm&sheetName=NoRM
```

### Using Postman (or similar)

**GET Request**:
- Method: GET
- URL: `https://your-deployment-url/exec?action=getLastRm`
- Headers: (none required)
- Body: (none)

**POST Request**:
- Method: POST
- URL: `https://your-deployment-url/exec`
- Headers: `Content-Type: application/json`
- Body:
```json
{
  "action": "addPatient",
  "rmNumber": "A.0051",
  "patientName": "Test Patient"
}
```

### Using cURL
```bash
# GET
curl "https://your-deployment-url/exec?action=getLastRm"

# POST
curl -X POST \
  -H "Content-Type: application/json" \
  -d '{"action":"addPatient","rmNumber":"A.0051","patientName":"Test"}' \
  https://your-deployment-url/exec
```

---

## Data Types

### RM Number Format
Flexible format supported:
- `A.0001` ✅
- `P001` ✅
- `001` ✅
- `RM-2024-001` ✅

The system doesn't impose strict format validation.

### Patient Name
- Any string up to 255 characters
- Supports special characters
- Leading/trailing spaces will be trimmed

---

## Logging & Debugging

All requests are logged in Google Apps Script execution logs.

To view logs:
1. Go to https://script.google.com
2. Open your project
3. Click "Executions" (upper left menu)
4. Find entries with timestamps matching your requests
5. Click to expand and see detailed logs

**Log entries include**:
- Start/end of each action
- Parameter values
- Sheet operations
- Any errors encountered

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 2.0 | 2024 | Added getLastRm and addPatient actions |
| 1.0 | Earlier | Original lookup and append functionality |

**Backward Compatibility**: ✅ All v1.0 endpoints still work

---

## Integration Checklist

- [ ] Deployment URL configured in GoogleSheetsConfig.json
- [ ] NoRM sheet exists in Google Sheet
- [ ] Sheet name and column indices verified
- [ ] getLastRm tested in browser
- [ ] addPatient tested via Postman/cURL
- [ ] C# application updated with AddPatientService
- [ ] Both endpoints working in production

---

## Support & Reference

**For more information**:
- Feature overview: `README_PATIENT_CREATION.md`
- Deployment: `DEPLOYMENT_GUIDE.md`
- Testing: `TESTING_CHECKLIST.md`
- Architecture: `WORKFLOW_DIAGRAMS.md`
