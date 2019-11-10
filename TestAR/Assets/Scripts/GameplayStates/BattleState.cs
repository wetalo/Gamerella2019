using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using RoboRyanTron.Unite2017.Events;

public class BattleState : GameplayState
{
    //[SerializeField] LayerMask raycastMask;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager planeManager;


    enum PlayerTurn{
        Red,
        Blue,
        Max
    }


    PlayerTurn currentPlayerTurn = PlayerTurn.Red;

    bool tankIsSelected = false;
    TankSelect selectedTank = null;
    TankSelect tankTouched = null;
    bool isDrawingLine = false;

    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    float minDistanceBetweenPoints = 0.3f;

    public override void StartState()
    {
        Debug.Log("LogCat BattleState.StartState");

        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        SetPlayer();
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
    void SetPlayer()
    {
        DisableAllTanks();
        Color uiColor = Color.white;

        UIManager.Instance.proUGUI.text = "DisableAllTanks, currentPlayerTurn:" + currentPlayerTurn;
        switch (currentPlayerTurn)
        {
            case PlayerTurn.Red:
                uiColor = Color.red;
                foreach (GameObject redTank in gameManager.redTanks)
                {
                    redTank.GetComponentInChildren<TankSelect>().enabled = true;
                }
                break;
            case PlayerTurn.Blue:
                uiColor = Color.blue;
                foreach (GameObject blueTank in gameManager.blueTanks)
                {
                    blueTank.GetComponentInChildren<TankSelect>().enabled = true;
                }
                break;
            default:
                break;
        }
        UIManager.Instance.proUGUI.color = uiColor;
        UIManager.Instance.proUGUI.text = "FIGHT ON";

    }

    void DisableAllTanks()
    {
        UIManager.Instance.proUGUI.text = "DisableAllTanks";
        foreach (GameObject redTank in gameManager.redTanks)
        {
            redTank.GetComponentInChildren<TankSelect>().enabled = false;
        }

        foreach (GameObject blueTank in gameManager.blueTanks)
        {
            blueTank.GetComponentInChildren<TankSelect>().enabled = false;
        }
    }

    public override void RunState()
    {

        string uiManagerText = "";

        uiManagerText += "isDrawingLine: " + isDrawingLine + "\n";
        uiManagerText += "tankIsSelected: " + tankIsSelected + "\n";

        if (isDrawingLine && Input.touchCount <= 0)
        {
            isDrawingLine = false;
        }
        if ((Input.touchCount > 0))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;

            if (isDrawingLine && tankIsSelected)
            {
                
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    uiManagerText += "Physics raycast hit \n";
                    if (DistanceToLastPoint(raycastHit.point) > minDistanceBetweenPoints)
                    {
                        points.Add(raycastHit.point);
                        lineRenderer.positionCount = points.Count;
                        lineRenderer.SetPositions(points.ToArray());
                    }
                } else
                {
                    uiManagerText += "Physics raycast MISSED \n";
                    OnNewPathCreated(points);
                    lineRenderer.enabled = false;
                    isDrawingLine = false;
                }
                
            } else if (tankIsSelected)
            {
                    
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(raycast, out raycastHit))
                    {
                        if (raycastHit.collider.gameObject.GetComponent<TankSelect>() && raycastHit.collider.gameObject.GetComponent<TankSelect>().isActiveAndEnabled)
                        {
                            tankTouched = raycastHit.collider.gameObject.GetComponent<TankSelect>();

                            if (tankTouched == selectedTank)
                            {
                                lineRenderer.enabled = true;
                                points.Clear();
                                lineRenderer.positionCount = points.Count;
                                lineRenderer.SetPositions(points.ToArray());
                                points.Add(raycastHit.point);
                                isDrawingLine = true;
                            }
                            else
                            {
                                UnselectAllTanks();
                                SelectTank(raycastHit.collider.gameObject.GetComponent<TankSelect>());
                            }
                        }
                        else
                        {
                            UnselectAllTanks();
                            tankIsSelected = false;
                        }
                    }
                }

                   
            }
            else
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(raycast, out raycastHit))
                    {
                        if (raycastHit.collider.gameObject.GetComponent<TankSelect>() && raycastHit.collider.gameObject.GetComponent<TankSelect>().isActiveAndEnabled)
                        {
                            Debug.Log("LogCat Something Hit " + raycastHit.collider.gameObject.name);
                            UnselectAllTanks();
                            SelectTank(raycastHit.collider.gameObject.GetComponent<TankSelect>());
                        }
                    }
                }
            }
            
        }

        UIManager.Instance.proUGUI.text = uiManagerText;
    }

    void OnNewPathCreated(List<Vector3> points)
    {
    }
    float DistanceToLastPoint(Vector3 point)
    {
        if (points.Count <= 0)
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(points[points.Count - 1], point);
    }

    void UnselectAllTanks()
    {
        foreach (GameObject redTank in gameManager.redTanks)
        {
            if (redTank.GetComponentInChildren<TankSelect>().enabled)
            {

                redTank.GetComponentInChildren<TankSelect>().Unselect();
            }
        }

        foreach (GameObject blueTank in gameManager.blueTanks)
        {
            if (blueTank.GetComponentInChildren<TankSelect>().enabled)
            {

                blueTank.GetComponentInChildren<TankSelect>().Unselect();
            }
        }
    }

    void SelectTank(TankSelect tankSelect)
    {
        UIManager.Instance.proUGUI.text = "Tank selected: " + tankSelect.gameObject;
        tankSelect.Select();

        selectedTank = tankSelect;
        tankIsSelected = true;
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



    public override void EndState()
    {
        UIManager.Instance.proUGUI.color = Color.white;
        UIManager.Instance.proUGUI.text = "";
    }
}
