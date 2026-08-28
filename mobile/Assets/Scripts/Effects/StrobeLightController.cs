using System.Collections;
using UnityEngine;

namespace ARSafetySimulator.Effects
{
    /// <summary>
    /// Emergency Flashing Strobe Light Controller (pref_fx_strobe_light_01).
    /// Uses Unity Light component and material emissive blinking to provide lightweight mobile AR visual alerts.
    /// </summary>
    public class StrobeLightController : MonoBehaviour
    {
        [Header("Light Components")]
        [SerializeField] private Light strobeLight;
        [SerializeField] private Renderer lightHousingRenderer;

        [Header("Strobe Parameters")]
        [SerializeField] private float flashRateHz = 2.0f; // 2 flashes per second
        [SerializeField] private Color activeColor = Color.red;

        private bool isFlashing = false;
        private Coroutine flashCoroutine;

        private void Awake()
        {
            if (strobeLight == null) strobeLight = GetComponentInChildren<Light>();
            if (strobeLight != null) strobeLight.enabled = false;
        }

        public void StartStrobe()
        {
            if (isFlashing) return;
            isFlashing = true;
            flashCoroutine = StartCoroutine(FlashLoop());
        }

        public void StopStrobe()
        {
            isFlashing = false;
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }
            if (strobeLight != null) strobeLight.enabled = false;
        }

        private IEnumerator FlashLoop()
        {
            float halfPeriod = 0.5f / flashRateHz;

            while (isFlashing)
            {
                if (strobeLight != null) strobeLight.enabled = true;
                if (lightHousingRenderer != null)
                {
                    lightHousingRenderer.material.SetColor("_EmissionColor", activeColor * 2.0f);
                }

                yield return new WaitForSeconds(halfPeriod);

                if (strobeLight != null) strobeLight.enabled = false;
                if (lightHousingRenderer != null)
                {
                    lightHousingRenderer.material.SetColor("_EmissionColor", Color.black);
                }

                yield return new WaitForSeconds(halfPeriod);
            }
        }
    }
}
