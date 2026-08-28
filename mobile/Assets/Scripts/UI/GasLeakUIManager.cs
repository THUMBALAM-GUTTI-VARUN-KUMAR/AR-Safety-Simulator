using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARSafetySimulator.Assessment;

namespace ARSafetySimulator.UI
{
    /// <summary>
    /// Coordinates all AR In-Game UI views:
    /// - GasHazardHUD
    /// - HazardIdentificationModal (MCQ)
    /// - PPE Selection Modal
    /// - SCBAVisorOverlay
    /// - EmergencyAlarmPrompt
    /// - EscapeNavigation
    /// - CompletionAssessment
    /// </summary>
    public class GasLeakUIManager : MonoBehaviour
    {
        [Header("1. Gas Hazard HUD")]
        [SerializeField] private GameObject gasHazardHUD;
        [SerializeField] private TextMeshProUGUI gasLevelText;

        [Header("2. Hazard Identification Modal (MCQ)")]
        [SerializeField] private GameObject hazardIdentificationModal;
        [SerializeField] private Button btnHazardGasLeak;      // Correct
        [SerializeField] private Button btnHazardWaterLeak;    // Incorrect
        [SerializeField] private Button btnHazardElectric;     // Incorrect

        [Header("3. PPE Selection Modal")]
        [SerializeField] private GameObject ppeSelectionModal;
        [SerializeField] private Button btnSelectSCBA;         // Correct
        [SerializeField] private Button btnSelectDustMask;     // Incorrect (Critical hazard)

        [Header("4. SCBA Visor Overlay")]
        [SerializeField] private GameObject scbaVisorOverlay;
        [SerializeField] private TextMeshProUGUI o2PressureText;

        [Header("5. Emergency Alarm Prompt")]
        [SerializeField] private GameObject emergencyAlarmPrompt;

        [Header("6. Escape Navigation")]
        [SerializeField] private GameObject escapeNavigation;
        [SerializeField] private TextMeshProUGUI escapeDistanceText;

        [Header("7. Completion Assessment")]
        [SerializeField] private GameObject completionAssessment;
        [SerializeField] private TextMeshProUGUI resultScoreText;
        [SerializeField] private TextMeshProUGUI resultMistakesText;
        [SerializeField] private TextMeshProUGUI resultDurationText;
        [SerializeField] private TextMeshProUGUI resultStatusText;
        [SerializeField] private Button btnSyncResults;

        public event Action<bool> OnHazardIdentified; // true if gas leak, false if wrong
        public event Action<bool> OnPPESelected;       // true if SCBA, false if Dust Mask
        public event Action OnSyncResultsClicked;

        private void Awake()
        {
            // Bind MCQ choices
            if (btnHazardGasLeak != null) btnHazardGasLeak.onClick.AddListener(() => SubmitHazardChoice(true));
            if (btnHazardWaterLeak != null) btnHazardWaterLeak.onClick.AddListener(() => SubmitHazardChoice(false));
            if (btnHazardElectric != null) btnHazardElectric.onClick.AddListener(() => SubmitHazardChoice(false));

            // Bind PPE choices
            if (btnSelectSCBA != null) btnSelectSCBA.onClick.AddListener(() => SubmitPPEChoice(true));
            if (btnSelectDustMask != null) btnSelectDustMask.onClick.AddListener(() => SubmitPPEChoice(false));

            // Bind Sync button
            if (btnSyncResults != null) btnSyncResults.onClick.AddListener(() => OnSyncResultsClicked?.Invoke());

            HideAllUI();
        }

        public void HideAllUI()
        {
            if (gasHazardHUD != null) gasHazardHUD.SetActive(false);
            if (hazardIdentificationModal != null) hazardIdentificationModal.SetActive(false);
            if (ppeSelectionModal != null) ppeSelectionModal.SetActive(false);
            if (scbaVisorOverlay != null) scbaVisorOverlay.SetActive(false);
            if (emergencyAlarmPrompt != null) emergencyAlarmPrompt.SetActive(false);
            if (escapeNavigation != null) escapeNavigation.SetActive(false);
            if (completionAssessment != null) completionAssessment.SetActive(false);
        }

        public void ShowGasHazardHUD(bool show, float ch4 = 0f, float co = 0f)
        {
            if (gasHazardHUD != null) gasHazardHUD.SetActive(show);
            if (gasLevelText != null)
            {
                gasLevelText.text = $"⚠️ <b>HAZARD DETECTED</b> | CH4: {ch4:F1}% | CO: {co:F0} ppm";
            }
        }

        public void ShowHazardIdentificationModal(bool show)
        {
            if (hazardIdentificationModal != null) hazardIdentificationModal.SetActive(show);
        }

        private void SubmitHazardChoice(bool isGasLeak)
        {
            ShowHazardIdentificationModal(false);
            OnHazardIdentified?.Invoke(isGasLeak);
        }

        public void ShowPPESelectionModal(bool show)
        {
            if (ppeSelectionModal != null) ppeSelectionModal.SetActive(show);
        }

        private void SubmitPPEChoice(bool isSCBA)
        {
            ShowPPESelectionModal(false);
            OnPPESelected?.Invoke(isSCBA);
        }

        public void ShowSCBAVisorOverlay(bool show, float o2Bar = 300f)
        {
            if (scbaVisorOverlay != null) scbaVisorOverlay.SetActive(show);
            if (o2PressureText != null)
            {
                o2PressureText.text = $"SCBA ONLINE | O2 PRESSURE: {o2Bar:F0} BAR";
            }
        }

        public void ShowEmergencyAlarmPrompt(bool show)
        {
            if (emergencyAlarmPrompt != null) emergencyAlarmPrompt.SetActive(show);
        }

        public void ShowEscapeNavigation(bool show, float distanceMeters = 0f)
        {
            if (escapeNavigation != null) escapeNavigation.SetActive(show);
            if (escapeDistanceText != null)
            {
                escapeDistanceText.text = $"REFUGE CHAMBER: {distanceMeters:F1}m UPWIND";
            }
        }

        public void ShowCompletionAssessment(AssessmentResult result)
        {
            HideAllUI();
            if (completionAssessment != null)
            {
                completionAssessment.SetActive(true);
                if (resultScoreText != null) resultScoreText.text = $"Score: {result.score} / 100";
                if (resultMistakesText != null) resultMistakesText.text = $"Mistakes: {result.mistakes}";
                if (resultDurationText != null) resultDurationText.text = $"Duration: {result.duration_seconds}s";
                if (resultStatusText != null)
                {
                    resultStatusText.text = result.passed ?
                        "<color=#22c55e><b>PASSED - SAFETY CERTIFIED</b></color>" :
                        "<color=#ef4444><b>FAILED - CRITICAL PROTOCOL BREACH</b></color>";
                }
            }
        }
    }
}
