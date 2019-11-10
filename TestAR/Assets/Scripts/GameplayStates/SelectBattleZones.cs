using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using RoboRyanTron.Unite2017.Events;

public class SelectBattleZones : GameplayState
{

    [SerializeField] GameObject placementIndicator;

    [SerializeField] GameObject redBattlePlanePresentation;
    [SerializeField] GameObject blueBattlePlanePresentation;

    [SerializeField] GameEvent startTankPlacementEvent;


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
        Debug.Log("LogCat: SelectBattleZone StartState");
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        SetPlayer();
    }

    void SetPlayer()
    {
        Debug.Log("LogCat: SelectBattleZone SetPlayer");
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
        UIManager.Instance.proUGUI.color = uiColor;
        UIManager.Instance.proUGUI.text = "Point the screen at your table, wait for the UI to appear, and click the button when ready";
    }

    public override void RunState()
    {
        Debug.Log("LogCat: SelectBattleZone RunState");
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SelectBattlePlane();
        }
    }

    void SelectBattlePlane()
    {
        Debug.Log("LogCat: SelectBattleZone SelectBattlePlane");
        if (currentPlayerTurn == PlayerTurn.Red)
        {
            gameManager.redPlane = planeManager.GetPlane(hits[0].trackableId);
            GameObject redPlaneInstance = Instantiate(redBattlePlanePresentation, placementPose.position, placementPose.rotation);
            redPlaneInstance.transform.localScale = planeManager.transform.localScale;

        } else if(currentPlayerTurn == PlayerTurn.Blue)
        {
            gameManager.bluePlane = planeManager.GetPlane(hits[0].trackableId);
            GameObject bluePlaneInstance = Instantiate(blueBattlePlanePresentation, placementPose.position, placementPose.rotation);
            bluePlaneInstance.transform.localScale = planeManager.transform.localScale;
        }

        NextPlayerTurn();

    }



    void NextPlayerTurn()
    {
        Debug.Log("LogCat: SelectBattleZone NextPlayerTurn");
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


        Debug.Log("LogCat: SelectBattleZone UpdatePlacementPost hits.count: " + hits.Count);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            Debug.Log("Found Plane!");
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            Debug.Log("No Plane!");
        }
        Debug.Log("hits.Count: " + hits.Count);
        
    }

    public override void EndState()
    {
        Debug.Log("LogCat: SelectBattleZone EndState");
        UIManager.Instance.proUGUI.color = Color.white;
        UIManager.Instance.proUGUI.text = "";
        startTankPlacementEvent.Raise();
    }
}
