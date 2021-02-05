using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKU_TraceBones : MonoBehaviour
{
    public Transform[] myBones;
    OVRSkeleton skeleton;
    SkinnedMeshRenderer skinnedMeshRenderer;
    int BonesCount;

    Transform[] testBone;
    bool isinit;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        skeleton = GetComponentInParent<OVRSkeleton>();
        skinnedMeshRenderer = GetComponentInParent<SkinnedMeshRenderer>();
        BonesCount = myBones.Length;


        while (!skeleton.IsInitialized)
        {
            yield return null;
        }
     
        isinit = true;
            yield return null;

    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if (isinit)
        {
            for (int i = 0; i < BonesCount; i++)
            {
      
                Vector3 temp = skeleton.Bones[i].Transform.eulerAngles;
           
                 myBones[i].transform.eulerAngles = temp;
            }
  
        }
        else
        {

        }
    }
}
