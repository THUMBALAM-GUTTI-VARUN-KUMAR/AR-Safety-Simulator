using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARSafetySimulator.AR
{
    /// <summary>
    /// Handles AR Plane detection, visual placement reticle, and tap-to-place anchoring
    /// of the Gas Leak Training Area onto real-world horizontal surfaces.
    /// </summary>
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("AR Foundation Components")]
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private ARPlaneManager planeManager;

        [Header("Placement Settings")]
        [SerializeField] private GameObject placementReticlePrefab;
        [SerializeField] private GameObject trainingAreaPrefab;

        [Header("Runtime State")]
        private GameObject spawnedReticle;
        private GameObject spawnedTrainingArea;
        private Pose placementPose;
        private bool isPlacementValid = false;
        private bool isAreaPlaced = false;

        public event Action<GameObject> OnTrainingAreaPlaced;

        public bool IsAreaPlaced => isAreaPlaced;
        public GameObject SpawnedTrainingArea => spawnedTrainingArea;

        private void Awake()
        {
            if (raycastManager == null) raycastManager = FindObjectOfType<ARRaycastManager>();
            if (planeManager == null) planeManager = FindObjectOfType<ARPlaneManager>();
        }

        private void Start()
        {
            if (placementReticlePrefab != null && spawnedReticle == null)
            {
                spawnedReticle = Instantiate(placementReticlePrefab);
                spawnedReticle.SetActive(false);
            }
        }

        private void Update()
        {
            if (isAreaPlaced) return;

            UpdatePlacementPose();
            UpdatePlacementReticle();

            // Handle touch / tap placement
            if (isPlacementValid && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    PlaceTrainingArea();
                }
            }
            #if UNITY_EDITOR
            // Mouse click support for Unity Editor simulation
            if (isPlacementValid && Input.GetMouseButtonDown(0))
            {
                PlaceTrainingArea();
            }
            #endif
        }

        private void UpdatePlacementPose()
        {
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var hits = new List<ARRaycastHit>();

            if (raycastManager != null && raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                isPlacementValid = hits.Count > 0;
                if (isPlacementValid)
                {
                    placementPose = hits[0].pose;
                    var cameraForward = Camera.main.transform.forward;
                    var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
            else
            {
                #if UNITY_EDITOR
                // Editor fallback raycast against ground plane
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                if (groundPlane.Raycast(ray, out float distance))
                {
                    isPlacementValid = true;
                    placementPose.position = ray.GetPoint(distance);
                    placementPose.rotation = Quaternion.identity;
                }
                #else
                isPlacementValid = false;
                #endif
            }
        }

        private void UpdatePlacementReticle()
        {
            if (spawnedReticle == null) return;

            if (isPlacementValid && !isAreaPlaced)
            {
                spawnedReticle.SetActive(true);
                spawnedReticle.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
            else
            {
                spawnedReticle.SetActive(false);
            }
        }

        public void PlaceTrainingArea()
        {
            if (isAreaPlaced || !isPlacementValid) return;

            if (trainingAreaPrefab != null)
            {
                spawnedTrainingArea = Instantiate(trainingAreaPrefab, placementPose.position, placementPose.rotation);
            }

            isAreaPlaced = true;
            if (spawnedReticle != null) spawnedReticle.SetActive(false);

            // Optionally disable visual planes to reduce visual clutter once placed
            if (planeManager != null)
            {
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
            }

            OnTrainingAreaPlaced?.Invoke(spawnedTrainingArea);
            Debug.Log("[ARPlacementManager] Training Area successfully anchored in AR space.");
        }

        public void ResetPlacement()
        {
            if (spawnedTrainingArea != null)
            {
                Destroy(spawnedTrainingArea);
                spawnedTrainingArea = null;
            }
            isAreaPlaced = false;
            if (planeManager != null)
            {
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(true);
                }
            }
        }
    }
}
