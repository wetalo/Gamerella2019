using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using RoboRyanTron.Unite2017.Events;

public class BattleState : GameplayState
{

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

    public override void StartState()
    {
        Debug.Log("LogCat BattleState.StartState");

        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        SetPlayer();
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
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                //UIManager.Instance.proUGUI.text += "Raycast hit: " + raycastHit.collider.gameObject.name;
                //UIManager.Instance.proUGUI.text += "tankselect is active and enabled: " + raycastHit.collider.gameObject.GetComponent<TankSelect>().isActiveAndEnabled;
                //Debug.Log("LogCat Something Hit " + raycastHit.collider.gameObject.name);

                if (tankIsSelected)
                {
                    if (raycastHit.collider.gameObject.GetComponent<TankSelect>() && raycastHit.collider.gameObject.GetComponent<TankSelect>().isActiveAndEnabled)
                    {
                        TankSelect tankTouched = raycastHit.collider.gameObject.GetComponent<TankSelect>();

                        if(tankTouched == selectedTank)
                        {

                        } else
                        {
                            UnselectAllTanks();
                            SelectTank(raycastHit.collider.gameObject.GetComponent<TankSelect>());
                        }
                    }
                } else {
                    if (raycastHit.collider.gameObject.GetComponent<TankSelect>() && raycastHit.collider.gameObject.GetComponent<TankSelect>().isActiveAndEnabled)
                    {
                        Debug.Log("LogCat Something Hit " + raycastHit.collider.gameObject.name);
                        UnselectAllTanks();
                        SelectTank(raycastHit.collider.gameObject.GetComponent<TankSelect>());
                    }
                } 
            {
                UIManager.Instance.proUGUI.text += "Nothing hit";
            }
        }
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
