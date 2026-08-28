using System;
using UnityEngine;
using TMPro;

namespace ARSafetySimulator.Equipment
{
    /// <summary>
    /// Portable Multi-Gas Detector Controller (pref_eq_gas_detector_01).
    /// Supports distance-to-leak detection, dynamic beep frequency, and live gas concentration display.
    /// </summary>
    public class GasDetectorController : MonoBehaviour
    {
        [Header("Leak Source Reference")]
        [SerializeField] private Transform leakSourceTransform;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip beepClip;

        [Header("Detection Thresholds")]
        [SerializeField] private float maxDetectionDistance = 5.0f;
        [SerializeField] private float minDetectionDistance = 0.5f;
        [SerializeField] private float minBeepInterval = 0.15f;
        [SerializeField] private float maxBeepInterval = 1.2f;

        [Header("Gas Concentration Range")]
        [SerializeField] private float baselineCH4 = 0.05f; // Normal %
        [SerializeField] private float peakCH4 = 7.2f;      // High hazard %
        [SerializeField] private float baselineCO = 5.0f;   // ppm
        [SerializeField] private float peakCO = 120.0f;     // ppm

        [Header("Display UI")]
        [SerializeField] private TextMeshPro screenText;

        private float nextBeepTime = 0f;
        private float currentDistance = 10f;
        private bool isEquipped = false;
        private bool isAlarmTriggered = false;

        public event Action<float, float> OnGasLevelUpdated; // CH4%, CO ppm
        public event Action OnHazardThresholdExceeded;

        public float CurrentCH4 { get; private set; }
        public float CurrentCO { get; private set; }

        public void SetLeakSource(Transform source)
        {
            leakSourceTransform = source;
        }

        public void EquipDetector(bool equip)
        {
            isEquipped = equip;
        }

        private void Update()
        {
            if (leakSourceTransform == null) return;

            currentDistance = Vector3.Distance(transform.position, leakSourceTransform.position);

            // Normalized proximity (0 = far away, 1 = right next to leak)
            float proximity = Mathf.Clamp01(1.0f - ((currentDistance - minDetectionDistance) / (maxDetectionDistance - minDetectionDistance)));

            CurrentCH4 = Mathf.Lerp(baselineCH4, peakCH4, proximity);
            CurrentCO = Mathf.Lerp(baselineCO, peakCO, proximity);

            UpdateScreenDisplay();
            HandleBeepFrequency(proximity);

            OnGasLevelUpdated?.Invoke(CurrentCH4, CurrentCO);

            // Trigger threshold event if concentration exceeds safe limits (CH4 > 1.25%)
            if (CurrentCH4 > 1.25f && !isAlarmTriggered)
            {
                isAlarmTriggered = true;
                OnHazardThresholdExceeded?.Invoke();
            }
        }

        private void HandleBeepFrequency(float proximity)
        {
            if (proximity <= 0.05f) return; // Silent if far away

            float currentInterval = Mathf.Lerp(maxBeepInterval, minBeepInterval, proximity);

            if (Time.time >= nextBeepTime)
            {
                if (audioSource != null && beepClip != null)
                {
                    audioSource.PlayOneShot(beepClip, 0.7f + (proximity * 0.3f));
                }
                nextBeepTime = Time.time + currentInterval;
            }
        }

        private void UpdateScreenDisplay()
        {
            if (screenText != null)
            {
                screenText.text = $"<b>MULTI-GAS SENTRY</b>\n" +
                                  $"CH4: {CurrentCH4:F1}%\n" +
                                  $"CO:  {CurrentCO:F0} ppm\n" +
                                  $"O2:  19.5% VOL\n" +
                                  (CurrentCH4 > 1.25f ? "<color=red><b>[CRITICAL HAZARD]</b></color>" : "<color=green>[NORMAL]</color>");
            }
        }
    }
}
