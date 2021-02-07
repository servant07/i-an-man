using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TraceAnchor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    OVRSkeleton skeleton;
    [SerializeField]
    Transform target;
    Transform transform;
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(skeleton.IsInitialized)
        {
            if(skeleton.IsDataValid && skeleton.IsDataHighConfidence)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
            }
        }
    }
}
