using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using TMPro;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] GameObject prefabToPlace;
    [SerializeField] GameObject placementIndicator;
    [SerializeField] TextMeshProUGUI proUGUI;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;


    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(prefabToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        } else
        {
            placementIndicator.SetActive(true);
        }

        proUGUI.text = "placementPoseIsValid: " + placementPoseIsValid;
    }

    private void UpdatePlacementPose()
    {
        try { 
            var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);


            placementPoseIsValid = hits.Count > 0;
            if (placementPoseIsValid)
            {
                Debug.Log("Found Plane!");
                placementPose = hits[0].pose;

                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            } else
            {
                Debug.Log("No Plane!");
            }
            Debug.Log("hits.Count: " + hits.Count);
        } catch(Exception e)
        {
            Debug.Log("Raycast error: " + e.Message);
        }
    }
}
