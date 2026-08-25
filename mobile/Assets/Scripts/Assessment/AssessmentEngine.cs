using System;
using System.Collections.Generic;

namespace ARSafetySimulator.Assessment
{
    public enum ActionType
    {
        Correct,
        Incorrect,
        CriticalFailure
    }

    public class ActionRecord
    {
        public string ActionId { get; set; }
        public ActionType Type { get; set; }
        public int Points { get; set; }
        public float Timestamp { get; set; }
    }

    public class ScoringRule
    {
        public string ScenarioId { get; set; }
        public int MaxScore { get; set; }
        public int PassingScore { get; set; }
        public float MaxTimeSeconds { get; set; }
    }

    public class AssessmentResult
    {
        // Must match API_CONTRACT.md keys when serialized to JSON
        public string trainee_id;
        public string scenario_id;
        public int score;
        public int duration_seconds;
        public int mistakes;
        public bool passed;
    }

    /// <summary>
    /// Deterministic Scoring Engine. 
    /// Does NOT rely on AI or external APIs for core safety-critical evaluations.
    /// </summary>
    public class AssessmentEngine
    {
        private string traineeId;
        private ScoringRule rule;
        private List<ActionRecord> actions;
        private float startTime;
        private float endTime;
        private bool hasCriticalFailure;

        public AssessmentEngine(string traineeId, ScoringRule rule)
        {
            this.traineeId = traineeId;
            this.rule = rule;
            this.actions = new List<ActionRecord>();
            this.hasCriticalFailure = false;
        }

        public void StartScenario(float currentTimestampSeconds)
        {
            startTime = currentTimestampSeconds;
        }

        public void RecordAction(string actionId, ActionType type, int points, float currentTimestampSeconds)
        {
            actions.Add(new ActionRecord
            {
                ActionId = actionId,
                Type = type,
                Points = points,
                Timestamp = currentTimestampSeconds
            });

            if (type == ActionType.CriticalFailure)
            {
                hasCriticalFailure = true;
            }
        }

        public AssessmentResult EndScenario(float currentTimestampSeconds)
        {
            endTime = currentTimestampSeconds;
            int totalScore = 0;
            int mistakes = 0;

            foreach (var action in actions)
            {
                totalScore += action.Points;
                if (action.Type == ActionType.Incorrect || action.Type == ActionType.CriticalFailure)
                {
                    mistakes++;
                }
            }

            // Ensure score does not drop below 0
            totalScore = Math.Max(0, totalScore);
            
            // Ensure score does not exceed maximum
            totalScore = Math.Min(totalScore, rule.MaxScore);

            int duration = (int)Math.Round(endTime - startTime);
            
            // Trainee passes if there are no critical failures AND they met the point threshold
            bool passed = !hasCriticalFailure && (totalScore >= rule.PassingScore);

            return new AssessmentResult
            {
                trainee_id = traineeId,
                scenario_id = rule.ScenarioId,
                score = totalScore,
                duration_seconds = duration,
                mistakes = mistakes,
                passed = passed
            };
        }
    }
}
