from pydantic import BaseModel, Field
from typing import List, Optional
from datetime import datetime

class TrainingResultCreate(BaseModel):
    trainee_id: str
    scenario_id: str
    score: int = Field(ge=0)
    duration_seconds: int = Field(ge=0)
    mistakes: int = Field(ge=0)
    passed: bool

class TrainingResultResponse(BaseModel):
    id: str
    trainee_id: str
    scenario_id: str
    score: int
    duration_seconds: int
    mistakes: int
    passed: bool
    completed_at: datetime

    class Config:
        from_attributes = True

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
