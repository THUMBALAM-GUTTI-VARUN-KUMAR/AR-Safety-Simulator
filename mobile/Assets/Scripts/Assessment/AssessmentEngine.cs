using System;
using System.Collections.Generic;

namespace ARSafetySimulator.Assessment
{
    /// <summary>
    /// Deterministic Scoring Engine. 
    /// Does NOT rely on AI or external APIs for core safety-critical evaluations.
    /// </summary>
    public class AssessmentEngine
    {
        private string traineeId;
        private ScoringRule rule;
        private List<GasLeakEvent> events;
        private float startTime;
        private float endTime;
        private bool hasCriticalFailure;
        private string sessionId;

        public AssessmentEngine(string traineeId, ScoringRule rule, string sessionId = null)
        {
            this.traineeId = traineeId;
            this.rule = rule;
            this.events = new List<GasLeakEvent>();
            this.hasCriticalFailure = false;
            this.sessionId = string.IsNullOrEmpty(sessionId) ? Guid.NewGuid().ToString() : sessionId;
        }

        public void StartScenario(float currentTimestampSeconds)
        {
            startTime = currentTimestampSeconds;
        }

        public void RecordEvent(GasLeakEvent telemetryEvent)
        {
            telemetryEvent.sessionId = this.sessionId;
            telemetryEvent.traineeId = this.traineeId;
            telemetryEvent.scenarioId = rule.ScenarioId;
            telemetryEvent.timestampIso = DateTime.UtcNow.ToString("o");
            
            events.Add(telemetryEvent);

            if (telemetryEvent.isCriticalHazard)
            {
                hasCriticalFailure = true;
            }
        }

        public AssessmentResult EndScenario(float currentTimestampSeconds)
        {
            endTime = currentTimestampSeconds;
            int totalScore = 0;
            int mistakes = 0;

            foreach (var ev in events)
            {
                if (ev.isCorrect)
                {
                    // Add points based on stepId
                    var actionScore = rule.CorrectActionPoints.Find(a => a.stepId == ev.stepId);
                    if (actionScore != null)
                    {
                        totalScore += actionScore.points;
                    }
                }
                else
                {
                    totalScore -= ev.penaltyPoints;
                    mistakes++;
                }
            }

            int duration = (int)Math.Round(endTime - startTime);

            // Time bonus
            if (rule.TimeBonusPoints > 0 && duration <= rule.TimeBonusThreshold && !hasCriticalFailure)
            {
                totalScore += rule.TimeBonusPoints;
            }

            // Ensure score does not drop below 0
            totalScore = Math.Max(0, totalScore);
            
            // Ensure score does not exceed maximum
            totalScore = Math.Min(totalScore, rule.MaxScore);
            
            // Timeout failure condition
            if (duration > rule.MaxTimeSeconds)
            {
                hasCriticalFailure = true;
            }

            // Trainee passes if there are no critical failures AND they met the point threshold
            bool passed = !hasCriticalFailure && (totalScore >= rule.PassingScore);

            return new AssessmentResult
            {
                session_id = this.sessionId,
                trainee_id = this.traineeId,
                scenario_id = rule.ScenarioId,
                score = totalScore,
                duration_seconds = duration,
                mistakes = mistakes,
                passed = passed,
                is_synced = false,
                events = this.events
            };
        }
    }
}
