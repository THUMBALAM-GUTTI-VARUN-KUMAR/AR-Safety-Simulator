// Mock data and API adapter
export interface AssessmentResult {
  session_id: string;
  trainee_id: string;
  scenario_id: string;
  score: number;
  duration_seconds: number;
  mistakes: number;
  passed: boolean;
  events: any[];
}

// Set this to true to use real backend later
const USE_REAL_BACKEND = false;
const API_BASE_URL = "http://localhost:8000/api";
const HEADERS = {
  "Content-Type": "application/json",
  "X-API-Key": "default-dev-key"
};

const mockResults: AssessmentResult[] = [
  {
    session_id: "uuid-1",
    trainee_id: "T001",
    scenario_id: "gas_leak",
    score: 90,
    duration_seconds: 45,
    mistakes: 1,
    passed: true,
    events: []
  },
  {
    session_id: "uuid-2",
    trainee_id: "T002",
    scenario_id: "gas_leak",
    score: 75,
    duration_seconds: 120,
    mistakes: 3,
    passed: false,
    events: []
  },
  {
    session_id: "uuid-3",
    trainee_id: "T001",
    scenario_id: "fire_explosion",
    score: 100,
    duration_seconds: 30,
    mistakes: 0,
    passed: true,
    events: []
  }
];

export async function fetchTrainingResults(): Promise<AssessmentResult[]> {
  if (USE_REAL_BACKEND) {
    const res = await fetch(`${API_BASE_URL}/training/results`, { headers: HEADERS });
    if (!res.ok) throw new Error("Failed to fetch");
    return res.json();
  }
  return new Promise(resolve => setTimeout(() => resolve(mockResults), 500));
}

export async function fetchTraineeResults(traineeId: string): Promise<AssessmentResult[]> {
  if (USE_REAL_BACKEND) {
    const res = await fetch(`${API_BASE_URL}/training/results/${traineeId}`, { headers: HEADERS });
    if (!res.ok) throw new Error("Failed to fetch");
    return res.json();
  }
  return new Promise(resolve => setTimeout(() => resolve(mockResults.filter(r => r.trainee_id === traineeId)), 500));
}
