using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSelect : MonoBehaviour
{
    bool selectedTank = false;
    [SerializeField] GameObject selectedRing;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        selectedTank = true;
        selectedRing.SetActive(true);
    }

    public void Unselect()
    {
        selectedRing.SetActive(false);
    }
}
