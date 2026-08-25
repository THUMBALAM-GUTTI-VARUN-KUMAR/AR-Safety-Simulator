from sqlalchemy.orm import Session
import models, schemas

def create_training_result(db: Session, result: schemas.TrainingResultCreate):
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

    db_session = models.TrainingSession(
        user_id=user.id,
        scenario_id=result.scenario_id,
        score=result.score,
        duration_seconds=result.duration_seconds,
        mistakes=result.mistakes,
        passed=result.passed
    )
    db.add(db_session)
    db.commit()
    db.refresh(db_session)
    return db_session

def get_training_results(db: Session, skip: int = 0, limit: int = 100):
    return db.query(models.TrainingSession).offset(skip).limit(limit).all()

def get_results_by_trainee(db: Session, trainee_id: str):
    user = db.query(models.User).filter(models.User.employee_id == trainee_id).first()
    if not user:
        return []
    return db.query(models.TrainingSession).filter(models.TrainingSession.user_id == user.id).all()
