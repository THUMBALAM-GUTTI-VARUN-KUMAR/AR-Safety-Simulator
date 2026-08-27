from sqlalchemy import Column, String, Integer, Boolean, Float, ForeignKey, DateTime
from sqlalchemy.orm import relationship
import datetime
from database import Base

class TrainingSession(Base):
    __tablename__ = "training_sessions"

    id = Column(String, primary_key=True, index=True) # session_id
    trainee_id = Column("user_id", String, index=True)
    scenario_id = Column(String, index=True)
    score = Column(Integer)
    duration_seconds = Column(Integer)
    mistakes = Column(Integer)
    passed = Column(Boolean)
    completed_at = Column(DateTime, default=datetime.datetime.utcnow)

    events = relationship("TrainingEvent", back_populates="session", cascade="all, delete-orphan")

class TrainingEvent(Base):
    __tablename__ = "training_events"

    id = Column(Integer, primary_key=True, index=True)
    session_id = Column(String, ForeignKey("training_sessions.id"))
    timestamp_iso = Column(String)
    elapsed_seconds = Column(Float)
    step_id = Column(String)
    action_type = Column(String)
    object_id = Column(String)
    is_correct = Column(Boolean)
    is_critical_hazard = Column(Boolean)
    penalty_points = Column(Integer)
    hazard_context = Column(String)
    metadata_json = Column(String)

    session = relationship("TrainingSession", back_populates="events")
