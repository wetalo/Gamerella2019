using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using RoboRyanTron.Unite2017.Events;

public class TankPlacement : GameplayState
{

    [SerializeField] GameObject placementIndicator;
    [SerializeField] int maxNumTanks;

    [SerializeField] GameObject redTankPrefab;
    [SerializeField] GameObject blueTankPrefab;
    [SerializeField] GameEvent startFightEvent;
    int currentNumTanks = 0;


    List<ARRaycastHit> hits = new List<ARRaycastHit>();



    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager planeManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    enum PlayerTurn{
        Red,
        Blue,
        Max
    }


    PlayerTurn currentPlayerTurn = PlayerTurn.Red;
         
    public override void StartState()
    {

        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        SetPlayer();
    }

    void SetPlayer()
    {
        Color uiColor = Color.white;
        switch (currentPlayerTurn)
        {
            case PlayerTurn.Red:
                uiColor = Color.red;
                break;
            case PlayerTurn.Blue:
                uiColor = Color.blue;
                break;
        }
        currentNumTanks = 0;
        UIManager.Instance.proUGUI.color = uiColor;
        UIManager.Instance.proUGUI.text = "Point the screen at your plane, wait for the UI to appear, and click the button to spawn a tank";
    }

    public override void RunState()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SpawnTank();
        }
    }

    void SpawnTank()
    {
        if (currentPlayerTurn == PlayerTurn.Red)
        {
            GameObject tank = Instantiate(redTankPrefab, placementPose.position, placementPose.rotation);
            gameManager.redTanks.Add(tank);
            currentNumTanks++;

        } else if(currentPlayerTurn == PlayerTurn.Blue)
        {
            GameObject tank = Instantiate(blueTankPrefab, placementPose.position, placementPose.rotation);
            gameManager.blueTanks.Add(tank);
            currentNumTanks++;
        }
        if(currentNumTanks >= maxNumTanks)
        {
            NextPlayerTurn();
        }

    }



    void NextPlayerTurn()
    {
        currentPlayerTurn++;
        if(currentPlayerTurn >= PlayerTurn.Max)
        {
            EndState();
        } else
        {
            SetPlayer();
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    private void UpdatePlacementPose()
    {
        
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        ARPlane playerPlane = null;

        if(currentPlayerTurn == PlayerTurn.Blue)
        {
            playerPlane = gameManager.bluePlane;
        } else if(currentPlayerTurn == PlayerTurn.Red)
        {
            playerPlane = gameManager.redPlane;
        }

        placementPoseIsValid = (hits.Count > 0 && planeManager.GetPlane(hits[0].trackableId) == playerPlane);
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        
    }

    public override void EndState()
    {
        UIManager.Instance.proUGUI.color = Color.white;
        UIManager.Instance.proUGUI.text = "";
        placementIndicator.SetActive(false);

        startFightEvent.Raise();
    }
}
