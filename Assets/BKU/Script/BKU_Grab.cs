using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;



public class BKU_Grab : BKU_GestureBase
{
    OVRSkeleton skeleton;
    Transform hand_Palm;
    Transform grabPoint;
    LineRenderer lineRenderer;


    Transform grabTarget;
    Transform handlingObject;

    [SerializeField]
    Text debugText;
    bool grabReadyActive;
    bool grabActive;


    Collider[] grabableObject;
    delegate void Log();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Transform parent = transform.parent;
        grabableObject = new Collider[10];
        gestureInput = parent.GetComponent<BKU_GestureInput>();
        hand_Palm = parent.Find("Pose/Hand_Palm");
        grabPoint = parent.Find("Pose/Hand_Palm/GrabPoint");
        lineRenderer = hand_Palm.GetComponentInChildren<LineRenderer>();
        lineRenderer.enabled = false;
        WaitWhile wait = new WaitWhile(() => { return gestureInput.IsInitialized; });
        yield return wait;
        skeleton = gestureInput.Skeleton;
    }

    // Update is called once per frame
    void Update()
    {
        if (gestureInput.IsInitialized)
        {
            Log log = null;
            if(gestureInput.IsGrab && grabActive)
            {
                log = GestureStart;

            }
            else if (gestureInput.IsGrab && !grabActive)
            {
                log = GestureStay;
  
            }
            else if(!gestureInput.IsGrab&& grabActive)
            {
                log = GestureEnd;

            }

            if (log != null)
            {

                log();
                Debug.Log(gestureInput.handType + " - " + log.Method.Name);

            }
        }
    }
    void GrabReadyStart()
    {
        lineRenderer.enabled = true;
        grabReadyActive = true;
    }
    void GrabReadyStay()
    {
        if (skeleton.IsDataValid && skeleton.IsDataHighConfidence)
        {
           
            //Vector3 dir = hand_Palm.right;
            //Ray ray = new Ray(hand_Palm.position, dir);
            //RaycastHit hitinfo = new RaycastHit();
            //Vector3[] positions = new Vector3[2];
            //positions[0] = hand_Palm.position;
            //positions[1] = hand_Palm.position + dir * 5;
            //lineRenderer.SetPositions(positions);
            //if (Physics.SphereCast(ray, 0.05f, out hitinfo, 5f,31))
            //{
            //    lineRenderer.material.color = new Color(0, 0, 0);
            //    grabTarget = hitinfo.transform;
            //}
            //else
            //{
            //    lineRenderer.material.color = new Color(1, 0, 0);
            //    grabTarget = null;
            //}




        }

    }
    void GrabReadyEnd()
    {
        lineRenderer.enabled = false;
        grabReadyActive = false;


    }
    void GrabStart()
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

        grabActive = true;

        if (handlingObject != null)
        {
            handlingObject.gameObject.layer = 31;
            handlingObject.position = hand_Palm.position;
            handlingObject.rotation = hand_Palm.rotation;
            handlingObject.parent = hand_Palm;
        }
    }
    void GrabStay()
    {
        if (skeleton.IsDataValid && skeleton.IsDataHighConfidence)
        {

        }
        else
        {
            GrabEnd();
        }
    }
    void GrabEnd()
    {
        grabActive = false;
        if (handlingObject != null)
        {
            handlingObject.gameObject.layer = 0;
            handlingObject.parent = null;
            handlingObject = null;
        }
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

        grabActive = true;

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
            GrabEnd();
        }

    }

    protected override void GestureEnd()
    {
        grabActive = false;
        if (handlingObject != null)
        {
            handlingObject.gameObject.layer = 0;
            handlingObject.parent = null;
            handlingObject = null;
        }
        
    }
}
