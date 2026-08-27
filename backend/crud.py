from sqlalchemy.orm import Session
import models, schemas
import datetime

def create_training_result(db: Session, result: schemas.AssessmentResult):
    # Idempotent creation to prevent duplicates
    db_session = db.query(models.TrainingSession).filter(models.TrainingSession.id == result.session_id).first()
    if db_session:
        return db_session 

    db_session = models.TrainingSession(
        id=result.session_id,
        trainee_id=result.trainee_id,
        scenario_id=result.scenario_id,
        score=result.score,
        duration_seconds=result.duration_seconds,
        mistakes=result.mistakes,
        passed=result.passed,
        completed_at=datetime.datetime.utcnow()
    )
    db.add(db_session)

    for event in result.events:
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

def get_training_results(db: Session, skip: int = 0, limit: int = 100):
    return db.query(models.TrainingSession).offset(skip).limit(limit).all()

def get_training_results_by_trainee(db: Session, trainee_id: str):
    return db.query(models.TrainingSession).filter(models.TrainingSession.trainee_id == trainee_id).all()
