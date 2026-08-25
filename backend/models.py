import uuid
from sqlalchemy import Boolean, Column, ForeignKey, Integer, String, DateTime
from sqlalchemy.sql import func
from sqlalchemy.orm import relationship
from database import Base

class User(Base):
    __tablename__ = "users"

    # Using String for UUID to keep SQLite compatibility for quick testing
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

    id = Column(String, primary_key=True, default=lambda: str(uuid.uuid4()))
    user_id = Column(String, ForeignKey("users.id"))
    scenario_id = Column(String, ForeignKey("scenarios.id"))
    score = Column(Integer)
    duration_seconds = Column(Integer)
    mistakes = Column(Integer)
    passed = Column(Boolean)
    completed_at = Column(DateTime(timezone=True), server_default=func.now())

    user = relationship("User", back_populates="sessions")
    scenario = relationship("Scenario", back_populates="sessions")
