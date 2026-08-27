from fastapi.testclient import TestClient
from main import app
import schemas

client = TestClient(app)
headers = {"X-API-Key": "default-dev-key"}

def test_get_scenarios():
    response = client.get("/api/scenarios", headers=headers)
    assert response.status_code == 200
    assert len(response.json()) == 2
    assert response.json()[0]["id"] == "gas_leak"

def test_submit_result_and_retrieve():
    payload = {
        "session_id": "test-uuid-1234",
        "trainee_id": "T001",
        "scenario_id": "gas_leak",
        "score": 90,
        "duration_seconds": 45,
        "mistakes": 1,
        "passed": True,
        "events": [
            {
                "sessionId": "test-uuid-1234",
                "traineeId": "T001",
                "scenarioId": "gas_leak",
                "timestampIso": "2026-08-27T10:04:30.000Z",
                "elapsedSeconds": 5.2,
                "stepId": "STEP_01_DETECTION",
                "actionType": "Interact",
                "objectId": "GasMonitor",
                "isCorrect": True,
                "isCriticalHazard": False,
                "penaltyPoints": 0,
                "hazardContext": "Checked air quality",
                "metadata": "{}"
            }
        ]
    }

    # Submit result
    response = client.post("/api/training/result", json=payload, headers=headers)
    assert response.status_code == 200
    assert response.json()["message"] == "Result saved successfully"

    # Retrieve all results
    response = client.get("/api/training/results", headers=headers)
    assert response.status_code == 200
    results = response.json()
    assert len(results) >= 1
    assert any(r["id"] == "test-uuid-1234" for r in results)

    # Retrieve by trainee
    response = client.get("/api/training/results/T001", headers=headers)
    assert response.status_code == 200
    results = response.json()
    assert len(results) >= 1
    assert results[0]["trainee_id"] == "T001"

def test_duplicate_submission():
    payload = {
        "session_id": "test-uuid-9999",
        "trainee_id": "T002",
        "scenario_id": "gas_leak",
        "score": 100,
        "duration_seconds": 30,
        "mistakes": 0,
        "passed": True,
        "events": []
    }

    # First submission
    response = client.post("/api/training/result", json=payload, headers=headers)
    assert response.status_code == 200

    # Second submission (should be idempotent and not error, or prevent duplicate)
    response = client.post("/api/training/result", json=payload, headers=headers)
    assert response.status_code == 200

    # Verify only one is stored (by checking the DB or retrieving)
    response = client.get("/api/training/results/T002", headers=headers)
    assert response.status_code == 200
    assert len(response.json()) == 1

def test_sync_results():
    payload = {
        "results": [
            {
                "session_id": "sync-uuid-1",
                "trainee_id": "T003",
                "scenario_id": "gas_leak",
                "score": 85,
                "duration_seconds": 50,
                "mistakes": 2,
                "passed": True,
                "events": []
            }
        ]
    }

    response = client.post("/api/sync", json=payload, headers=headers)
    assert response.status_code == 200
    assert response.json()["synced"] == 1
    assert response.json()["failed"] == 0

    # Verify sync works and is idempotent
    response = client.post("/api/sync", json=payload, headers=headers)
    assert response.status_code == 200
    assert response.json()["synced"] == 0 # Already synced
