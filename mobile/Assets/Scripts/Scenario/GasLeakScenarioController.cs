using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARSafetySimulator.AR;
using ARSafetySimulator.Equipment;
using ARSafetySimulator.Effects;
using ARSafetySimulator.Interaction;
using ARSafetySimulator.UI;
using ARSafetySimulator.Assessment;

namespace ARSafetySimulator.Scenario
{
    public enum ScenarioState
    {
        AwaitingARPlacement,
        Step1_Detection,
        Step2_HazardIdentification,
        Step3_PPESelection,
        Step4_EmergencyAlarm,
        Step5_AirflowAssessment,
        Step6_EvacuateSafeZone,
        Completed
    }

    /// <summary>
    /// Master State Machine Controller for the Gas Leak AR Training Module (SIM_GAS_LEAK_01).
    /// Executes the 6-step DGMS safety protocol, records deterministic telemetry,
    /// and feeds the offline assessment engine.
    /// </summary>
    public class GasLeakScenarioController : MonoBehaviour
    {
        [Header("Trainee Info")]
        [SerializeField] private string traineeId = "T-JH-2026-001";
        [SerializeField] private string scenarioId = "SIM_GAS_LEAK_01";

        [Header("AR Placement")]
        [SerializeField] private ARPlacementManager arPlacementManager;

        [Header("In-Game Prefab References (Spawned in AR)")]
        [SerializeField] private GameObject gasLeakPipeObject;
        [SerializeField] private GameObject gasCloudParticleEffect;
        [SerializeField] private GasDetectorController gasDetector;
        [SerializeField] private EmergencyAlarmSwitch alarmSwitch;
        [SerializeField] private StrobeLightController strobeLight;
        [SerializeField] private GameObject windsockObject;
        [SerializeField] private GameObject escapeWaypointObject;
        [SerializeField] private GameObject refugeChamberDoorObject;
        [SerializeField] private InteractableObject safeZoneMarker;

        [Header("UI Manager")]
        [SerializeField] private GasLeakUIManager uiManager;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource ambientAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioClip gasHissingClip;
        [SerializeField] private AudioClip industrialAlarmClip;
        [SerializeField] private AudioClip scbaBreathingClip;
        [SerializeField] private AudioClip doorOpenClip;
        [SerializeField] private AudioClip successClip;

        private ScenarioState currentState = ScenarioState.AwaitingARPlacement;
        private AssessmentEngine assessmentEngine;
        private OfflineStorageManager storageManager;
        private float scenarioStartTime;
        private string sessionId;

        public ScenarioState CurrentState => currentState;

        private void Start()
        {
            sessionId = Guid.NewGuid().ToString();
            storageManager = new OfflineStorageManager();

            // Initialize standard DGMS Gas Leak scoring rules
            var scoringRule = new ScoringRule
            {
                ScenarioId = scenarioId,
                PassingScore = 75,
                MaxScore = 100,
                MaxTimeSeconds = 180,
                TimeBonusThreshold = 90,
                TimeBonusPoints = 10,
                CorrectActionPoints = new List<ActionScore>
                {
                    new ActionScore { stepId = "STEP_01_DETECTION", points = 20 },
                    new ActionScore { stepId = "STEP_02_IDENTIFICATION", points = 15 },
                    new ActionScore { stepId = "STEP_03_PPE_SELECTION", points = 25 },
                    new ActionScore { stepId = "STEP_04_ALARM_ACTIVATION", points = 20 },
                    new ActionScore { stepId = "STEP_05_AIRFLOW_ASSESSMENT", points = 10 },
                    new ActionScore { stepId = "STEP_06_EVACUATION_SAFE_ZONE", points = 10 }
                }
            };

            assessmentEngine = new AssessmentEngine(traineeId, scoringRule, sessionId);

            // Hook up AR Placement
            if (arPlacementManager != null)
            {
                arPlacementManager.OnTrainingAreaPlaced += OnARAreaPlaced;
            }

            // Hook up UI Events
            if (uiManager != null)
            {
                uiManager.OnHazardIdentified += HandleHazardIdentified;
                uiManager.OnPPESelected += HandlePPESelected;
                uiManager.OnSyncResultsClicked += HandleSyncResults;
            }

            // Hook up Gas Detector
            if (gasDetector != null)
            {
                gasDetector.OnHazardThresholdExceeded += HandleGasThresholdExceeded;
            }

            // Hook up Alarm Switch
            if (alarmSwitch != null)
            {
                alarmSwitch.OnAlarmActivated += HandleAlarmActivated;
            }

            // Hook up Safe Zone trigger
            if (safeZoneMarker != null)
            {
                safeZoneMarker.OnInteracted += HandleSafeZoneReached;
            }
        }

        private void OnARAreaPlaced(GameObject spawnedArea)
        {
            Debug.Log("[GasLeakScenarioController] AR Area anchored. Starting Step 1: Detection.");
            scenarioStartTime = Time.time;
            assessmentEngine.StartScenario(scenarioStartTime);

            // Start ambient hissing sound & particle plume
            if (ambientAudioSource != null && gasHissingClip != null)
            {
                ambientAudioSource.clip = gasHissingClip;
                ambientAudioSource.loop = true;
                ambientAudioSource.Play();
            }

            if (gasCloudParticleEffect != null) gasCloudParticleEffect.SetActive(true);

            TransitionToState(ScenarioState.Step1_Detection);
        }

        private void TransitionToState(ScenarioState newState)
        {
            currentState = newState;
            Debug.Log($"[GasLeakScenarioController] State Transition -> {newState}");

            switch (newState)
            {
                case ScenarioState.Step1_Detection:
                    if (uiManager != null) uiManager.ShowGasHazardHUD(true, 0.2f, 10f);
                    break;

                case ScenarioState.Step2_HazardIdentification:
                    if (uiManager != null) uiManager.ShowHazardIdentificationModal(true);
                    break;

                case ScenarioState.Step3_PPESelection:
                    if (uiManager != null) uiManager.ShowPPESelectionModal(true);
                    break;

                case ScenarioState.Step4_EmergencyAlarm:
                    if (uiManager != null) uiManager.ShowEmergencyAlarmPrompt(true);
                    break;

                case ScenarioState.Step5_AirflowAssessment:
                    if (uiManager != null) uiManager.ShowEscapeNavigation(true, 15f);
                    if (escapeWaypointObject != null) escapeWaypointObject.SetActive(true);
                    break;

                case ScenarioState.Step6_EvacuateSafeZone:
                    if (safeZoneMarker != null) safeZoneMarker.IsInteractive = true;
                    break;

                case ScenarioState.Completed:
                    CompleteScenario();
                    break;
            }
        }

        private void HandleGasThresholdExceeded()
        {
            if (currentState != ScenarioState.Step1_Detection) return;

            RecordStepEvent("STEP_01_DETECTION", "INSPECT_GAS_MONITOR", "pref_eq_gas_detector_01", true, false, 0);
            TransitionToState(ScenarioState.Step2_HazardIdentification);
        }

        private void HandleHazardIdentified(bool isCorrect)
        {
            if (currentState != ScenarioState.Step2_HazardIdentification) return;

            int penalty = isCorrect ? 0 : 15;
            RecordStepEvent("STEP_02_IDENTIFICATION", "IDENTIFY_HAZARD_MCQ", "mcq_gas_leak", isCorrect, !isCorrect, penalty);

            TransitionToState(ScenarioState.Step3_PPESelection);
        }

        private void HandlePPESelected(bool isSCBA)
        {
            if (currentState != ScenarioState.Step3_PPESelection) return;

            if (isSCBA)
            {
                RecordStepEvent("STEP_03_PPE_SELECTION", "DON_PPE", "pref_ppe_scba_mask_01", true, false, 0);
                if (uiManager != null) uiManager.ShowSCBAVisorOverlay(true, 300f);

                // Play SCBA breathing sound
                if (sfxAudioSource != null && scbaBreathingClip != null)
                {
                    sfxAudioSource.PlayOneShot(scbaBreathingClip);
                }
            }
            else
            {
                // Critical Failure: Dust mask does not protect against toxic/explosive gas
                RecordStepEvent("STEP_03_PPE_SELECTION", "DON_PPE", "pref_ppe_dust_mask_01", false, true, 30);
            }

            TransitionToState(ScenarioState.Step4_EmergencyAlarm);
        }

        private void HandleAlarmActivated()
        {
            if (currentState != ScenarioState.Step4_EmergencyAlarm) return;

            RecordStepEvent("STEP_04_ALARM_ACTIVATION", "PULL_MANUAL_ALARM", "pref_eq_alarm_switch_01", true, false, 0);

            if (uiManager != null) uiManager.ShowEmergencyAlarmPrompt(false);

            // Start industrial siren alarm & strobe light
            if (strobeLight != null) strobeLight.StartStrobe();
            if (ambientAudioSource != null && industrialAlarmClip != null)
            {
                ambientAudioSource.clip = industrialAlarmClip;
                ambientAudioSource.loop = true;
                ambientAudioSource.Play();
            }

            TransitionToState(ScenarioState.Step5_AirflowAssessment);
            StartCoroutine(AirflowAssessmentSequence());
        }

        private IEnumerator AirflowAssessmentSequence()
        {
            yield return new WaitForSeconds(3.0f);
            RecordStepEvent("STEP_05_AIRFLOW_ASSESSMENT", "READ_WINDSOCK", "pref_env_windsock_01", true, false, 0);
            TransitionToState(ScenarioState.Step6_EvacuateSafeZone);
        }

        private void HandleSafeZoneReached(InteractableObject obj)
        {
            if (currentState != ScenarioState.Step6_EvacuateSafeZone) return;

            RecordStepEvent("STEP_06_EVACUATION_SAFE_ZONE", "ENTER_REFUGE_CHAMBER", "pref_env_refuge_door_01", true, false, 0);

            if (sfxAudioSource != null && doorOpenClip != null)
            {
                sfxAudioSource.PlayOneShot(doorOpenClip);
            }

            TransitionToState(ScenarioState.Completed);
        }

        private void CompleteScenario()
        {
            float finishTime = Time.time;
            AssessmentResult result = assessmentEngine.EndScenario(finishTime);

            // Save to local offline SQLite / JSON storage queue
            storageManager.SaveResult(result);

            if (sfxAudioSource != null && successClip != null)
            {
                sfxAudioSource.PlayOneShot(successClip);
            }

            if (uiManager != null)
            {
                uiManager.ShowCompletionAssessment(result);
            }

            Debug.Log($"[GasLeakScenarioController] Scenario Ended. Score: {result.score}, Passed: {result.passed}");
        }

        private void HandleSyncResults()
        {
            StartCoroutine(storageManager.SyncQueuedResults((syncedCount) =>
            {
                Debug.Log($"[GasLeakScenarioController] Results successfully synchronized to Cloud Backend. Count: {syncedCount}");
            }));
        }

        private void RecordStepEvent(string stepId, string actionType, string objectId, bool isCorrect, bool isCritical, int penalty)
        {
            var telemetry = new GasLeakEvent
            {
                sessionId = this.sessionId,
                traineeId = this.traineeId,
                scenarioId = this.scenarioId,
                timestampIso = DateTime.UtcNow.ToString("o"),
                elapsedSeconds = Time.time - scenarioStartTime,
                stepId = stepId,
                actionType = actionType,
                objectId = objectId,
                isCorrect = isCorrect,
                isCriticalHazard = isCritical,
                penaltyPoints = penalty,
                hazardContext = "Gas Leak CH4/CO Ingress",
                metadata = "{}"
            };

            assessmentEngine.RecordEvent(telemetry);
        }
    }
}
