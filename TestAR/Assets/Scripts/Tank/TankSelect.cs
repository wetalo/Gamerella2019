using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSelect : MonoBehaviour
{
    bool selectedTank = false;
    bool isMoving = false;
    float tankMovementSpeed = 3f;
    [SerializeField] GameObject selectedRing;

    GameObject parent;
    Vector3 target;
    float minDistance = 0.2f;
    


    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            float step = tankMovementSpeed * Time.deltaTime; // calculate distance to move
            parent.transform.position = Vector3.MoveTowards(transform.position, target, step);

            if(Vector3.Distance (parent.transform.position, target) < minDistance)
            {
                parent.transform.position = target;
                isMoving = false;
            }
        }
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

    public void GoToPosition(Vector3 position)
    {
        target = position;
        parent.transform.LookAt(target);
        isMoving = true;
    }
}
