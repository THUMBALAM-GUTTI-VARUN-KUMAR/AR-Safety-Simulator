using System;
using UnityEngine;

namespace ARSafetySimulator.Equipment
{
    /// <summary>
    /// Manual Call Point / Emergency Alarm Switch (pref_eq_alarm_switch_01).
    /// Provides interaction trigger, lever/button animation, and event dispatch to trigger siren and strobe.
    /// </summary>
    public class EmergencyAlarmSwitch : MonoBehaviour
    {
        [Header("Alarm Visuals & Elements")]
        [SerializeField] private GameObject switchLever;
        [SerializeField] private GameObject statusLED;
        [SerializeField] private Material activeLEDMaterial;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pullSound;

        private bool isActivated = false;

        public event Action OnAlarmActivated;

        public bool IsActivated => isActivated;

        public void ActivateAlarm()
        {
            if (isActivated) return;

            isActivated = true;

            // Visual feedback: pull lever rotation
            if (switchLever != null)
            {
                switchLever.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            }

            // Status LED color change
            if (statusLED != null && activeLEDMaterial != null)
            {
                var renderer = statusLED.GetComponent<Renderer>();
                if (renderer != null) renderer.material = activeLEDMaterial;
            }

            // Audio feedback
            if (audioSource != null && pullSound != null)
            {
                audioSource.PlayOneShot(pullSound);
            }

            OnAlarmActivated?.Invoke();
            Debug.Log("[EmergencyAlarmSwitch] Manual Call Point Alarm Activated!");
        }
    }
}
