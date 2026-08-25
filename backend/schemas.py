from pydantic import BaseModel, Field
from typing import Optional
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
    user_id: str
    scenario_id: str
    score: int
    duration_seconds: int
    mistakes: int
    passed: bool
    completed_at: datetime

    class Config:
        from_attributes = True
