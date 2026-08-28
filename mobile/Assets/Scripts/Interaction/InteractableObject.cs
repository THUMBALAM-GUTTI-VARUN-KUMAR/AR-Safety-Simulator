using System;
using UnityEngine;

namespace ARSafetySimulator.Interaction
{
    /// <summary>
    /// Generic AR Screen-Tap / Raycast interaction handler for equipment, PPE, and alarm switches.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractableObject : MonoBehaviour
    {
        [Header("Item Identification")]
        [SerializeField] private string objectId;
        [SerializeField] private string displayName;
        [SerializeField] private bool isInteractive = true;

        [Header("Highlight / Feedback")]
        [SerializeField] private GameObject highlightGlow;
        [SerializeField] private AudioClip tapAudio;

        public event Action<InteractableObject> OnInteracted;

        public string ObjectId => objectId;
        public string DisplayName => displayName;
        public bool IsInteractive
        {
            get => isInteractive;
            set
            {
                isInteractive = value;
                if (highlightGlow != null) highlightGlow.SetActive(value);
            }
        }

        public void TriggerInteraction()
        {
            if (!isInteractive) return;

            if (tapAudio != null)
            {
                AudioSource.PlayClipAtPoint(tapAudio, transform.position);
            }

            OnInteracted?.Invoke(this);
            Debug.Log($"[InteractableObject] Interacted with: {displayName} ({objectId})");
        }

        private void OnMouseDown()
        {
            // Unity Editor & Android touch raycast fallback
            TriggerInteraction();
        }
    }
}
