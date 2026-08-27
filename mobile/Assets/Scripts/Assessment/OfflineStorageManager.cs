using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ARSafetySimulator.Assessment
{
    [Serializable]
    public class OfflineSyncQueue
    {
        public List<AssessmentResult> results = new List<AssessmentResult>();
    }

    public class OfflineStorageManager
    {
        private string storagePath;

        public OfflineStorageManager(string overridePath = null)
        {
            storagePath = overridePath ?? Path.Combine(Application.persistentDataPath, "offline_results.json");
        }

        public void SaveResult(AssessmentResult result)
        {
            if (string.IsNullOrEmpty(result.session_id))
            {
                result.session_id = Guid.NewGuid().ToString();
            }
            result.is_synced = false;

            OfflineSyncQueue queue = LoadQueue();
            
            // Prevent duplicate submission by session_id
            if (queue.results.Exists(r => r.session_id == result.session_id))
            {
                return; 
            }

            queue.results.Add(result);
            string json = JsonUtility.ToJson(queue);
            File.WriteAllText(storagePath, json);
        }

        public List<AssessmentResult> GetPendingResults()
        {
            OfflineSyncQueue queue = LoadQueue();
            return queue.results.FindAll(r => !r.is_synced);
        }

        public void MarkAsSynced(string sessionId)
        {
            OfflineSyncQueue queue = LoadQueue();
            var result = queue.results.Find(r => r.session_id == sessionId);
            if (result != null)
            {
                result.is_synced = true;
                string json = JsonUtility.ToJson(queue);
                File.WriteAllText(storagePath, json);
            }
        }

        public void ClearSynced()
        {
            OfflineSyncQueue queue = LoadQueue();
            queue.results.RemoveAll(r => r.is_synced);
            string json = JsonUtility.ToJson(queue);
            File.WriteAllText(storagePath, json);
        }

        private OfflineSyncQueue LoadQueue()
        {
            if (File.Exists(storagePath))
            {
                string json = File.ReadAllText(storagePath);
                var queue = JsonUtility.FromJson<OfflineSyncQueue>(json);
                if (queue != null) return queue;
            }
            return new OfflineSyncQueue();
        }
    }
}
