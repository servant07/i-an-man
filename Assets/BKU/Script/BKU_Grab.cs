using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;



public class BKU_Grab : BKU_GestureBase
{
    Transform hand_Palm;
    Transform grabPoint;


    Transform grabTarget;
    Transform handlingObject;

    [SerializeField]
    Text debugText;


    Collider[] grabableObject;
    delegate void Log();

    // Start is called before the first frame update
    void Start()
    {

        Transform parent = transform.parent;

        Init(parent.GetComponent<BKU_GestureInput>(), () => { return gestureInput.IsGrab; });
        hand_Palm = parent.Find("Pose/Hand_Palm");
        grabPoint = parent.Find("Pose/Hand_Palm/GrabPoint");

        grabableObject = new Collider[10];

    }


    protected override void GestureStart()
    {
        grabTarget = null;
        int numColliders = Physics.OverlapSphereNonAlloc(grabPoint.position, 0.05f, grabableObject);
        if (numColliders <= grabableObject.Length)
        {
            float minDistance = float.MaxValue;

            for (int i = 0; i < numColliders; i++)
            {
                float distance = Vector3.Distance(grabableObject[i].transform.position, grabPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    grabTarget = grabableObject[i].transform;
                }
            }

        }
        handlingObject = grabTarget;
        grabTarget = null;

        gestureActive = true;

        if (handlingObject != null)
        {
            handlingObject.gameObject.layer = 31;
            handlingObject.position = hand_Palm.position;
            handlingObject.rotation = hand_Palm.rotation;
            handlingObject.parent = hand_Palm;
        }
  
    }

    protected override void GestureStay()
    {
        if (skeleton.IsDataValid && skeleton.IsDataHighConfidence)
        {

        }
        else
        {
            GestureEnd();
        }

    }

    protected override void GestureEnd()
    {
        gestureActive = false;
        if (handlingObject != null)
        {
            handlingObject.gameObject.layer = 0;
            handlingObject.parent = null;
            handlingObject = null;
        }
        
    }
}
