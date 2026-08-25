import requests

BASE_URL = "http://127.0.0.1:8001"

def test_api():
    print("1. Submitting a new training result...")
    payload = {
        "trainee_id": "T001",
        "scenario_id": "gas_leak",
        "score": 86,
        "duration_seconds": 142,
        "mistakes": 2,
        "passed": True
    }
    response = requests.post(f"{BASE_URL}/api/training/result", json=payload)
    print(f"Status: {response.status_code}")
    print(f"Response: {response.json()}\n")

    print("2. Fetching all training results...")
    response = requests.get(f"{BASE_URL}/api/training/results")
    print(f"Status: {response.status_code}")
    print(f"Count: {len(response.json())}")
    if response.status_code == 200 and len(response.json()) > 0:
        print(f"Sample: {response.json()[0]}\n")

    print("3. Fetching results for trainee T001...")
    response = requests.get(f"{BASE_URL}/api/training/results/T001")
    print(f"Status: {response.status_code}")
    print(f"Count: {len(response.json())}\n")

if __name__ == "__main__":
    try:
        requests.get(f"{BASE_URL}/ping")
        test_api()
        print("All tests completed.")
    except requests.exceptions.ConnectionError:
        print("Error: Could not connect to the server. Is FastAPI running on port 8001?")
