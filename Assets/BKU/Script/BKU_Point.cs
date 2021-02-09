using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKU_Point : BKU_GestureBase
{


    [SerializeField]
    Transform hand_IndexTip;
    LineRenderer lineRenderer;
   
    // Start is called before the first frame update
    void Start()
    {
    
        Transform parent = transform.parent;

        Init(parent.GetComponent<BKU_GestureInput>(), () => { return gestureInput.IsPoint; });

        lineRenderer = hand_IndexTip.GetComponentInChildren<LineRenderer>();
        lineRenderer.enabled = false;
    }


    protected override void GestureStart()
    {
        lineRenderer.enabled = true;
        gestureActive = true;
    }

    protected override void GestureStay()
    {
        if (skeleton.IsDataValid && skeleton.IsDataHighConfidence)
        {

            Vector3 dir = hand_IndexTip.right;
            Ray ray = new Ray(hand_IndexTip.position, dir);
            RaycastHit hitinfo = new RaycastHit();
            Vector3[] positions = new Vector3[2];
            positions[0] = hand_IndexTip.position;
            positions[1] = hand_IndexTip.position + dir * 5;
            lineRenderer.SetPositions(positions);
            if (Physics.SphereCast(ray, 0.05f, out hitinfo, 5f, 31))
            {
                lineRenderer.material.color = new Color(0, 0, 0);
             
            }
            else
            {
                lineRenderer.material.color = new Color(1, 0, 0);
         
            }




        }
    }
    protected override void GestureEnd()
    {
        lineRenderer.enabled = false;
        gestureActive = false;
    }


   
}
