using System;
using System.Collections.Generic;

namespace ARSafetySimulator.Assessment
{
    [Serializable]
    public class GasLeakEvent
    {
        public string sessionId;
        public string traineeId;
        public string scenarioId;
        public string timestampIso;
        public float elapsedSeconds;
        public string stepId;
        public string actionType;
        public string objectId;
        public bool isCorrect;
        public bool isCriticalHazard;
        public int penaltyPoints;
        public string hazardContext;
        public string metadata;
    }

    [Serializable]
    public class ActionScore
    {
        public string stepId;
        public int points;
    }

    [Serializable]
    public class ScoringRule
    {
        public string ScenarioId;
        public int MaxScore;
        public int PassingScore;
        public float MaxTimeSeconds;
        public List<ActionScore> CorrectActionPoints = new List<ActionScore>();
        public int TimeBonusPoints;
        public float TimeBonusThreshold;
    }

    [Serializable]
    public class AssessmentResult
    {
        public string session_id;
        public string trainee_id;
        public string scenario_id;
        public int score;
        public int duration_seconds;
        public int mistakes;
        public bool passed;
        public bool is_synced;
        public List<GasLeakEvent> events = new List<GasLeakEvent>();
    }
}
