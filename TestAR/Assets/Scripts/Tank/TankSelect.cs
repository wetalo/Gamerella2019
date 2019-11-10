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
    Vector3 target = Vector3.zero;
    float minDistance = 0.2f;

    List<Vector3> targets = new List<Vector3>();
    int currentTargetIndex = 0;


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
            parent.transform.position = Vector3.MoveTowards(parent.transform.position, target, step);

            if(Vector3.Distance (parent.transform.position, target) < minDistance)
            {
                parent.transform.position = target;
                isMoving = false;
                GetNextTarget();

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
        selectedTank = false;
        selectedRing.SetActive(false);
    }

    public void GoToPosition(Vector3 position)
    {
        target = position;
        parent.transform.LookAt(target);
        Vector3 parentRotation = parent.transform.rotation.eulerAngles;
        parent.transform.rotation =  Quaternion.Euler(new Vector3(0f, parentRotation.y, parentRotation.z));
        isMoving = true;
    }

    public void SetPath(List<Vector3>points)
    {
        targets = points;
        currentTargetIndex = 0;
        isMoving = true;
    }

    void GetNextTarget()
    {
        currentTargetIndex++;
        if(currentTargetIndex < targets.Count)
        {
            isMoving = true;
            target = targets[currentTargetIndex];
        } else
        {
            isMoving = false;
        }
    }

    public string GetUIText()
    {

        string uiText = "Targets: " + targets.Count + "\n";
        uiText += "isMoving: " + isMoving + "\n";
        uiText += "parent.transform.position: " + parent.transform.position + "\n";
        uiText += "target: " + target + "\n";
        return uiText;
    }
}
