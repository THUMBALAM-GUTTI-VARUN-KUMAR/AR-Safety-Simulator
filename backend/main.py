from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import List

import models, schemas, crud
from database import engine, get_db

# Create database tables
models.Base.metadata.create_all(bind=engine)

app = FastAPI(title="AR Safety Simulator API")

@app.post("/api/training/result", status_code=201)
def submit_training_result(result: schemas.TrainingResultCreate, db: Session = Depends(get_db)):
    try:
        db_session = crud.create_training_result(db=db, result=result)
        return {"message": "Result saved successfully", "session_id": str(db_session.id)}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/training/results", response_model=List[schemas.TrainingResultResponse])
def get_all_training_results(db: Session = Depends(get_db)):
    results = crud.get_training_results(db)
    return results

@app.get("/api/training/results/{trainee_id}", response_model=List[schemas.TrainingResultResponse])
def get_trainee_results(trainee_id: str, db: Session = Depends(get_db)):
    results = crud.get_results_by_trainee(db, trainee_id=trainee_id)
    if not results:
        # Returning 404 if no results found, as requested by general REST practices, 
        # though empty list is also acceptable. Let's stick to empty list to avoid breaking dashboards.
        pass
    return results

# Health check
@app.get("/ping")
def ping():
    return {"status": "ok"}
