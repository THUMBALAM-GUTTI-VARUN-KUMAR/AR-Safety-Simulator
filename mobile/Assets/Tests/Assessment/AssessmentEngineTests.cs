using NUnit.Framework;
using ARSafetySimulator.Assessment;

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
                MaxTimeSeconds = 300f
            };
        }

        [Test]
        public void Test_PerfectRun_Passes()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            // Correct actions add positive points
            engine.RecordAction("equip_mask", ActionType.Correct, 50, 10f);
            engine.RecordAction("close_valve", ActionType.Correct, 50, 25f);
            
            var result = engine.EndScenario(30f);
            
            Assert.AreEqual(100, result.score);
            Assert.AreEqual(0, result.mistakes);
            Assert.AreEqual(30, result.duration_seconds);
            Assert.IsTrue(result.passed);
            Assert.AreEqual("T001", result.trainee_id);
            Assert.AreEqual("gas_leak", result.scenario_id);
        }

        [Test]
        public void Test_MinorMistakes_ReducesScoreButPasses()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordAction("equip_mask", ActionType.Correct, 50, 10f);
            // Incorrect action causes negative points and adds to mistakes
            engine.RecordAction("wrong_valve", ActionType.Incorrect, -10, 20f);
            engine.RecordAction("close_valve", ActionType.Correct, 50, 35f);
            
            var result = engine.EndScenario(40f);
            
            Assert.AreEqual(90, result.score);
            Assert.AreEqual(1, result.mistakes);
            Assert.IsTrue(result.passed);
        }

        [Test]
        public void Test_TooManyMistakes_FailsThreshold()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordAction("equip_mask", ActionType.Correct, 50, 10f);
            engine.RecordAction("wrong_valve_1", ActionType.Incorrect, -15, 20f);
            engine.RecordAction("wrong_valve_2", ActionType.Incorrect, -15, 25f);
            engine.RecordAction("close_valve", ActionType.Correct, 50, 35f);
            
            var result = engine.EndScenario(40f);
            
            Assert.AreEqual(70, result.score); // 50 - 15 - 15 + 50 = 70
            Assert.AreEqual(2, result.mistakes);
            Assert.IsFalse(result.passed); // 70 < 80 passing score
        }

        [Test]
        public void Test_CriticalFailure_AutoFails()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            // Critical failure gives a heavy penalty and marks the run as a failure
            engine.RecordAction("cause_spark", ActionType.CriticalFailure, -50, 10f);
            engine.RecordAction("equip_mask", ActionType.Correct, 50, 20f);
            engine.RecordAction("close_valve", ActionType.Correct, 50, 35f);
            
            var result = engine.EndScenario(40f);
            
            Assert.AreEqual(50, result.score); // Starts at 0, -50 capped at 0, then +50 +50? Wait, 50-50+50 = 50. 
            Assert.AreEqual(1, result.mistakes);
            Assert.IsFalse(result.passed); // Auto fails despite any other correct actions
        }

        [Test]
        public void Test_ScoreNeverDropsBelowZero()
        {
            var engine = new AssessmentEngine("T001", gasLeakRule);
            engine.StartScenario(0f);
            
            engine.RecordAction("horrible_mistake", ActionType.Incorrect, -100, 10f);
            
            var result = engine.EndScenario(20f);
            
            Assert.AreEqual(0, result.score);
            Assert.AreEqual(1, result.mistakes);
        }
    }
}
