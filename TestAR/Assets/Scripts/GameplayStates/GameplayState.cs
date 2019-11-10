using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayState : MonoBehaviour, IGameplayState
{
    [HideInInspector]
    public GameManager gameManager;
    public virtual void EndState()
    {
    }

    public virtual void RunState()
    {
    }

    public virtual void StartState()
    {
    }
}
