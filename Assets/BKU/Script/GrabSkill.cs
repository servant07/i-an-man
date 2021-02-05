using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSkill : MonoBehaviour
{
    public OVRSkeleton skeleton;
    CustomGesture customGesture;
    public LineRenderer lineRenderer;
    public Transform hand_Palm;
    // Start is called before the first frame update
    void Start()
    {

        customGesture = GetComponent<CustomGesture>();
 

    }

    // Update is called once per frame
    void Update()
    {
        if(customGesture.isInitialized)
        {
            if(customGesture.IsGrabReady)
            {
                lineRenderer.enabled = true;
                Ray ray = new Ray(hand_Palm.position, hand_Palm.right);
                Vector3[] positions = new Vector3[2];
                positions[0] = hand_Palm.position;
                positions[1] = hand_Palm.position + hand_Palm.right*5;
                lineRenderer.SetPositions(positions);
            }
            else
            {
                lineRenderer.enabled = false;
            }

        }
    }
}
