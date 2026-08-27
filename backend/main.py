import os
from fastapi import FastAPI, Depends, HTTPException, Header, status
from sqlalchemy.orm import Session
from typing import List

import models, schemas, crud
from database import engine, get_db
from fastapi.middleware.cors import CORSMiddleware

models.Base.metadata.create_all(bind=engine)

app = FastAPI(title="AR Safety Simulator API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

API_KEY = os.getenv("API_KEY", "default-dev-key")

def verify_api_key(x_api_key: str = Header(..., alias="X-API-Key")):
    if x_api_key != API_KEY:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid API Key")
    return x_api_key

@app.get("/")
def read_root():
    return {"message": "AR Safety Simulator API is running! Access the dashboard at http://localhost:5173"}

@app.get("/ping")
def ping():
    return {"status": "ok"}

# Assessment-Offline functionality (Legacy)
@app.post("/api/training/result-legacy", status_code=201)
def submit_training_result(result: schemas.TrainingResultCreate, db: Session = Depends(get_db)):
    try:
        db_session = crud.create_legacy_training_result(db=db, result=result)
        return {"message": "Result saved successfully", "session_id": str(db_session.id)}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

# Main functionality
@app.post("/api/training/result", dependencies=[Depends(verify_api_key)])
def submit_result(result: schemas.AssessmentResult, db: Session = Depends(get_db)):
    try:
        crud.create_training_result(db, result)
        return {"message": "Result saved successfully"}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))

@app.get("/api/training/results", dependencies=[Depends(verify_api_key)])
def get_results(db: Session = Depends(get_db)):
    return crud.get_training_results(db)

@app.get("/api/training/results/{trainee_id}", dependencies=[Depends(verify_api_key)])
def get_trainee_results(trainee_id: str, db: Session = Depends(get_db)):
    results = crud.get_training_results_by_trainee(db, trainee_id)
    if not results:
        pass # Returning empty list to avoid breaking dashboards
    return results

@app.post("/api/sync", dependencies=[Depends(verify_api_key)])
def sync_results(payload: schemas.SyncPayload, db: Session = Depends(get_db)):
    synced = 0
    for result in payload.results:
        existing = db.query(models.TrainingSession).filter(models.TrainingSession.id == result.session_id).first()
        if not existing:
            crud.create_training_result(db, result)
            synced += 1
    return {"synced": synced, "failed": 0}

@app.get("/api/scenarios", dependencies=[Depends(verify_api_key)])
def get_scenarios():
    return [
        {"id": "gas_leak", "name": "Gas Leak / Confined Space", "active": True},
        {"id": "fire_explosion", "name": "Fire / Explosion", "active": True}
    ]
