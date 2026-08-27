from pydantic import BaseModel
from typing import List, Optional

class GasLeakEvent(BaseModel):
    sessionId: str
    traineeId: str
    scenarioId: str
    timestampIso: str
    elapsedSeconds: float
    stepId: str
    actionType: str
    objectId: str
    isCorrect: bool
    isCriticalHazard: bool
    penaltyPoints: int
    hazardContext: str
    metadata: str

class AssessmentResult(BaseModel):
    session_id: str
    trainee_id: str
    scenario_id: str
    score: int
    duration_seconds: int
    mistakes: int
    passed: bool
    events: List[GasLeakEvent] = []

class SyncPayload(BaseModel):
    results: List[AssessmentResult]
