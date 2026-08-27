using NUnit.Framework;
using ARSafetySimulator.Assessment;
using System.IO;
using System.Collections.Generic;

namespace ARSafetySimulator.Tests
{
    public class OfflineStorageTests
    {
        private string tempPath;
        private OfflineStorageManager storageManager;

        [SetUp]
        public void Setup()
        {
            tempPath = Path.Combine(Path.GetTempPath(), "test_offline_results.json");
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            storageManager = new OfflineStorageManager(tempPath);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }

        [Test]
        public void Test_OfflineSave_And_PendingResults()
        {
            var result = new AssessmentResult
            {
                session_id = "S001",
                trainee_id = "T001",
                score = 80,
                passed = true
            };

            storageManager.SaveResult(result);

            List<AssessmentResult> pending = storageManager.GetPendingResults();
            Assert.AreEqual(1, pending.Count);
            Assert.AreEqual("S001", pending[0].session_id);
            Assert.IsFalse(pending[0].is_synced);
        }

        [Test]
        public void Test_DuplicateSession_Prevented()
        {
            var result1 = new AssessmentResult { session_id = "S001", score = 80 };
            var result2 = new AssessmentResult { session_id = "S001", score = 90 };

            storageManager.SaveResult(result1);
            storageManager.SaveResult(result2);

            List<AssessmentResult> pending = storageManager.GetPendingResults();
            Assert.AreEqual(1, pending.Count); // Should only have one
            Assert.AreEqual(80, pending[0].score); // Should be the first one
        }

        [Test]
        public void Test_Synchronization()
        {
            var result = new AssessmentResult { session_id = "S001", trainee_id = "T001" };
            storageManager.SaveResult(result);

            // Mark as synced
            storageManager.MarkAsSynced("S001");

            // Pending should now be 0
            List<AssessmentResult> pending = storageManager.GetPendingResults();
            Assert.AreEqual(0, pending.Count);
            
            // Assuming we added ClearSynced
            storageManager.ClearSynced();
        }
    }
}
