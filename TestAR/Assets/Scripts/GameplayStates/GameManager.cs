using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboRyanTron.Unite2017.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class GameManager : MonoBehaviour
{
    [Header("GameStates")]
    [SerializeField]GameplayState selectBattleZonesState;
    [SerializeField] GameplayState tankPlacementState;

    GameplayState currentState;

    public ARPlane redPlane;
    public ARPlane bluePlane;

    // Start is called before the first frame update
    void Awake()
    {
        selectBattleZonesState.gameManager = this;
        tankPlacementState.gameManager = this;

        StartSelectBattleZonesState();



    }
    void StartNewState(GameplayState newState)
    {
        currentState = newState;
        currentState.StartState();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.RunState();
    }

  
    void EndState()
    {
        currentState.EndState();
    }


    #region State Starters
    public void StartSelectBattleZonesState()
    {
        StartNewState(selectBattleZonesState);
    }

    public void StartTankPlacementState()
    {
        StartNewState(tankPlacementState);
    }

    #endregion

}
