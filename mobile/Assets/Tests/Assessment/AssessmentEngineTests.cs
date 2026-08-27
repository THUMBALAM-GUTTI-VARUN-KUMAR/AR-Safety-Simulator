using NUnit.Framework;
using ARSafetySimulator.Assessment;
using System.Collections.Generic;

namespace ARSafetySimulator.Tests
{
    public class AssessmentEngineTests
    {
        private ScoringRule gasLeakRule;

        [SetUp]
        public void Setup()
        {
            gasLeakRule = new ScoringRule
            {
                ScenarioId = "gas_leak",
                MaxScore = 100,
                PassingScore = 80,
                MaxTimeSeconds = 120f,
                CorrectActionPoints = new List<ActionScore>
                {
                    new ActionScore { stepId = "STEP_01_DETECTION", points = 20 },
                    new ActionScore { stepId = "STEP_03_PPE_SELECTION", points = 30 },
                    new ActionScore { stepId = "STEP_02_HAZARD_ID", points = 30 }, // Closing valve
                    new ActionScore { stepId = "STEP_06_EVACUATION_MOVEMENT", points = 20 }
                },
                TimeBonusPoints = 10,
                TimeBonusThreshold = 60f
            };
        }

        [Test]
        public void Test_CorrectAction_And_Completion()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_01_DETECTION", isCorrect = true, elapsedSeconds = 5f });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_03_PPE_SELECTION", isCorrect = true, elapsedSeconds = 15f });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_02_HAZARD_ID", isCorrect = true, elapsedSeconds = 30f });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_06_EVACUATION_MOVEMENT", isCorrect = true, elapsedSeconds = 45f });
            
            var result = engine.EndScenario(50f);
            
            // Total score: 20 + 30 + 30 + 20 = 100. Time bonus = +10. Total 110. Max score = 100.
            Assert.AreEqual(100, result.score);
            Assert.AreEqual(0, result.mistakes);
            Assert.AreEqual(50, result.duration_seconds);
            Assert.IsTrue(result.passed);
        }

        [Test]
        public void Test_WrongAction_Penalty()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_01_DETECTION", isCorrect = true });
            
            // Wrong action penalty
            engine.RecordEvent(new GasLeakEvent { stepId = "WRONG_VALVE", isCorrect = false, penaltyPoints = 10 });
            
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_03_PPE_SELECTION", isCorrect = true });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_02_HAZARD_ID", isCorrect = true });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_06_EVACUATION_MOVEMENT", isCorrect = true });
            
            var result = engine.EndScenario(70f); // Duration > 60, no bonus
            
            // Base 100 - 10 = 90
            Assert.AreEqual(90, result.score);
            Assert.AreEqual(1, result.mistakes);
            Assert.IsTrue(result.passed);
        }

        [Test]
        public void Test_CriticalHazard_AutoFails()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_01_DETECTION", isCorrect = true });
            
            // Critical failure
            engine.RecordEvent(new GasLeakEvent { stepId = "SPARK_IGNITION", isCorrect = false, isCriticalHazard = true, penaltyPoints = 50 });
            
            var result = engine.EndScenario(30f);
            
            Assert.AreEqual(0, result.score); // 20 - 50 = -30, capped at 0
            Assert.IsFalse(result.passed);
        }
        
        [Test]
        public void Test_Timeout_Fails()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_01_DETECTION", isCorrect = true });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_03_PPE_SELECTION", isCorrect = true });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_02_HAZARD_ID", isCorrect = true });
            engine.RecordEvent(new GasLeakEvent { stepId = "STEP_06_EVACUATION_MOVEMENT", isCorrect = true });
            
            var result = engine.EndScenario(150f); // Max time is 120
            
            Assert.IsFalse(result.passed); // Fails due to timeout
        }
    }
}
