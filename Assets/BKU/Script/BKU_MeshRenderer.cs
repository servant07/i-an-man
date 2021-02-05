using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKU_MeshRenderer : MonoBehaviour
{
    OVRSkeleton skeleton;
    SkinnedMeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.enabled = skeleton.IsDataValid  && skeleton.IsDataHighConfidence;
    }
}
