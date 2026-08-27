from sqlalchemy.orm import Session
import datetime
import uuid
import models, schemas

def create_training_result(db: Session, result: schemas.AssessmentResult):
    # Lookup user by employee_id (mapped to trainee_id in the payload)
    user = db.query(models.User).filter(models.User.employee_id == result.trainee_id).first()
    
    # Auto-create user for testing purposes if they don't exist
    if not user:
        user = models.User(employee_id=result.trainee_id, name=f"Trainee {result.trainee_id}")
        db.add(user)
        db.commit()
        db.refresh(user)
        
    # Auto-create scenario if it doesn't exist
    scenario = db.query(models.Scenario).filter(models.Scenario.id == result.scenario_id).first()
    if not scenario:
        scenario = models.Scenario(id=result.scenario_id, name=f"Scenario {result.scenario_id}", category="General")
        db.add(scenario)
        db.commit()
        db.refresh(scenario)

    # Idempotent creation to prevent duplicates
    db_session = db.query(models.TrainingSession).filter(models.TrainingSession.id == result.session_id).first()
    if db_session:
        return db_session 

    db_session = models.TrainingSession(
        id=result.session_id,
        user_id=user.id,
        trainee_id=result.trainee_id,
        scenario_id=scenario.id,
        score=result.score,
        duration_seconds=result.duration_seconds,
        mistakes=result.mistakes,
        passed=result.passed,
        completed_at=datetime.datetime.utcnow()
    )
    db.add(db_session)

    for event in getattr(result, "events", []):
        db_event = models.TrainingEvent(
            session_id=result.session_id,
            timestamp_iso=event.timestampIso,
            elapsed_seconds=event.elapsedSeconds,
            step_id=event.stepId,
            action_type=event.actionType,
            object_id=event.objectId,
            is_correct=event.isCorrect,
            is_critical_hazard=event.isCriticalHazard,
            penalty_points=event.penaltyPoints,
            hazard_context=event.hazardContext,
            metadata_json=event.metadata
        )
        db.add(db_event)
    
    db.commit()
    db.refresh(db_session)
    return db_session

def create_legacy_training_result(db: Session, result: schemas.TrainingResultCreate):
    legacy_result = schemas.AssessmentResult(
        session_id=str(uuid.uuid4()),
        trainee_id=result.trainee_id,
        scenario_id=result.scenario_id,
        score=result.score,
        duration_seconds=result.duration_seconds,
        mistakes=result.mistakes,
        passed=result.passed,
        events=[]
    )
    return create_training_result(db, legacy_result)

def get_training_results(db: Session, skip: int = 0, limit: int = 100):
    return db.query(models.TrainingSession).offset(skip).limit(limit).all()

def get_results_by_trainee(db: Session, trainee_id: str):
    return db.query(models.TrainingSession).filter(models.TrainingSession.trainee_id == trainee_id).all()

def get_training_results_by_trainee(db: Session, trainee_id: str):
    # This falls back to matching employee_id
    return get_results_by_trainee(db, trainee_id)
