import uuid
import datetime
from sqlalchemy import Column, String, Integer, Boolean, Float, ForeignKey, DateTime
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from database import Base

class User(Base):
    __tablename__ = "users"

    id = Column(String, primary_key=True, default=lambda: str(uuid.uuid4()))
    name = Column(String, index=True)
    employee_id = Column(String, unique=True, index=True)
    role = Column(String, default="trainee")
    language = Column(String, default="en")
    created_at = Column(DateTime(timezone=True), server_default=func.now())

    sessions = relationship("TrainingSession", back_populates="user")

class Scenario(Base):
    __tablename__ = "scenarios"

    id = Column(String, primary_key=True, index=True)
    name = Column(String)
    category = Column(String)
    version = Column(String)
    active = Column(Boolean, default=True)

    sessions = relationship("TrainingSession", back_populates="scenario")

class TrainingSession(Base):
    __tablename__ = "training_sessions"

    id = Column(String, primary_key=True, index=True, default=lambda: str(uuid.uuid4()))
    user_id = Column(String, ForeignKey("users.id"))
    trainee_id = Column(String, index=True) 
    scenario_id = Column(String, ForeignKey("scenarios.id"))
    score = Column(Integer)
    duration_seconds = Column(Integer)
    mistakes = Column(Integer)
    passed = Column(Boolean)
    completed_at = Column(DateTime(timezone=True), default=datetime.datetime.utcnow, server_default=func.now())

    user = relationship("User", back_populates="sessions")
    scenario = relationship("Scenario", back_populates="sessions")
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
