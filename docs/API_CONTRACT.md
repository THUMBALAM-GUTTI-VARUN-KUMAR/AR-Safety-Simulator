# API Contract

This document outlines the standard REST APIs for the system. 
Do not invent unnecessary APIs.

## Important Data Contract
When a trainee completes a scenario, the mobile application MUST send data using this basic structure:

```json
{
  "trainee_id": "T001",
  "scenario_id": "gas_leak",
  "score": 86,
  "duration_seconds": 142,
  "mistakes": 2,
  "passed": true
}
```
*Note: You may extend this structure if necessary, but do NOT change the core field names without documenting the change.*
The Unity team and backend team MUST follow this same contract.

## REST APIs

### POST /api/training/result
- **Purpose**: Submit a single training result.
- **HTTP Method**: POST
- **Request JSON**: See data contract above.
- **Response JSON**: `{ "message": "Result saved successfully" }`
- **Required fields**: `trainee_id`, `scenario_id`, `score`, `duration_seconds`, `passed`
- **Error response**: `400 Bad Request` if required fields are missing or invalid.
- **Authentication requirement**: API Key or Bearer Token.

### GET /api/training/results
- **Purpose**: Fetch all training results for the dashboard.
- **HTTP Method**: GET
- **Request JSON**: N/A
- **Response JSON**: Array of result objects.
- **Required fields**: N/A
- **Error response**: `401 Unauthorized` if admin token is invalid.
- **Authentication requirement**: Admin Bearer Token.

### GET /api/training/results/{trainee_id}
- **Purpose**: Fetch all training results for a specific trainee.
- **HTTP Method**: GET
- **Request JSON**: N/A
- **Response JSON**: Array of result objects for the trainee.
- **Required fields**: `trainee_id` in path.
- **Error response**: `404 Not Found` if trainee ID does not exist.
- **Authentication requirement**: Admin or Trainee token.

### GET /api/trainees/{trainee_id}
- **Purpose**: Get trainee profile details.
- **HTTP Method**: GET
- **Request JSON**: N/A
- **Response JSON**: Profile information for trainee.
- **Required fields**: `trainee_id` in path.
- **Error response**: `404 Not Found` if missing.
- **Authentication requirement**: Admin or Trainee token.

### POST /api/sync
- **Purpose**: Batch synchronization for the offline sync queue.
- **HTTP Method**: POST
- **Request JSON**: `{ "results": [ { ...Data Contract... } ] }`
- **Response JSON**: `{ "synced": 5, "failed": 0 }`
- **Required fields**: `results` array containing valid session payloads.
- **Error response**: `400 Bad Request` if array is malformed.
- **Authentication requirement**: API Key.

### POST /api/certificates
- **Purpose**: Generate a certificate after passing.
- **HTTP Method**: POST
- **Request JSON**: `{ "session_id": "123" }`
- **Response JSON**: `{ "certificate_id": "CERT123", "qr_code_url": "..." }`
- **Required fields**: `session_id`
- **Error response**: `400 Bad Request` if session did not pass or was not found.
- **Authentication requirement**: System / API Key.

### GET /api/certificates/{certificate_id}/verify
- **Purpose**: Verify certificate via QR scan.
- **HTTP Method**: GET
- **Request JSON**: N/A
- **Response JSON**: `{ "valid": true, "details": { ... } }`
- **Required fields**: `certificate_id` in path.
- **Error response**: `404 Not Found` if invalid.
- **Authentication requirement**: None (Public).

### GET /api/scenarios
- **Purpose**: List active scenarios to load in AR menu.
- **HTTP Method**: GET
- **Request JSON**: N/A
- **Response JSON**: Array of scenario definitions.
- **Required fields**: N/A
- **Error response**: `500 Internal Server Error`.
- **Authentication requirement**: API Key.
