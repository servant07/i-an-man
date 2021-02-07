using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;



public class BKU_Grab : MonoBehaviour
{

    public OVRSkeleton skeleton;
    BKU_Gesture customGesture;
    public LineRenderer lineRenderer;
    public Transform hand_Palm;
    Transform grabTarget;
    Transform handlingObject;

    [SerializeField]
    Text debugText;
    bool grabReadyActive;
    bool grabActive;

    // Start is called before the first frame update
    void Start()
    {
        customGesture = GetComponent<BKU_Gesture>();
    }

    // Update is called once per frame
    void Update()
    {
        if (customGesture.isInitialized)
        {
            if(customGesture.IsGrabReady && grabReadyActive)
            {
                GrabReadyStay();
                Debug.Log("GrabReadyStay");
            }
            else if(customGesture.IsGrab && grabActive)
            {
                GrabStay();
                Debug.Log("GrabStay");
            }
            else if (customGesture.IsGrabReady && !grabReadyActive)
            {
                GrabReadyStart();
                Debug.Log("GrabReadyStart");
            }
            else if (!customGesture.IsGrabReady && grabReadyActive)
            {
                GrabReadyEnd();
                Debug.Log("GrabReadyEnd");
            }
            else if (customGesture.IsGrab && !grabActive)
            {
                GrabStart();
                Debug.Log("GrabStart");
            }
            else if(!customGesture.IsGrab&& grabActive)
            {
                GrabEnd();
                Debug.Log("GrabReadGrabEndyStart");
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
            Ray ray = new Ray(hand_Palm.position, hand_Palm.right);
            RaycastHit hitinfo = new RaycastHit();
            Vector3[] positions = new Vector3[2];
            positions[0] = hand_Palm.position;
            positions[1] = hand_Palm.position + hand_Palm.right * 5;
            lineRenderer.SetPositions(positions);
            if (Physics.SphereCast(ray, 0.05f, out hitinfo, 5f,31))
            {
                lineRenderer.material.color = new Color(0, 0, 0);
                grabTarget = hitinfo.transform;
            }
            else
            {
                lineRenderer.material.color = new Color(1, 0, 0);
                grabTarget = null;
            }

        }
        else
        {
            GrabReadyEnd();
        }
    }
    void GrabReadyEnd()
    {
        lineRenderer.enabled = false;
        grabReadyActive = false;


    }
    void GrabStart()
    {
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
}
