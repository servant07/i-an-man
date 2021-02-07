using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BKU_TraceBones : MonoBehaviour
{
    public Transform[] myBones;
    OVRSkeleton skeleton;
    int BonesCount;
    // Start is called before the first frame update
    void Start()
    {
        skeleton = GetComponentInParent<OVRSkeleton>();
        BonesCount = myBones.Length;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (skeleton.IsInitialized)
        {
            if (skeleton.IsDataValid && skeleton.IsDataHighConfidence)
            {
                for (int i = 0; i < BonesCount; i++)
                {
                    Vector3 temp = skeleton.Bones[i].Transform.eulerAngles;
                    myBones[i].transform.eulerAngles = temp;
                }

            }

        }

    }
}
