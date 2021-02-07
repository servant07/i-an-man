using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

class BKU_TraceCapsules : MonoBehaviour
{
    public OVRSkeleton skeleton;
    BKU_TraceBones traceBones;
    List<OVRBoneCapsule> myBoneCapsule;


    bool isinit;
    // Start is called before the first frame update
    IEnumerator Start()
    {


        while (!skeleton.IsInitialized)
        {
            yield return null;
        }
        myBoneCapsule = new List<OVRBoneCapsule>();
        traceBones = skeleton.GetComponentInChildren<BKU_TraceBones>();
        Rigidbody[] myCapsules = GetComponentsInChildren<Rigidbody>();

        for(int i=0;i< myCapsules.Length;i++)
        {
            string temp = myCapsules[i].name;
            char sp = '_';
            string[] splitedTemp = temp.Split(sp);
            temp = splitedTemp[0] + "_" + splitedTemp[1];
            OVRSkeleton.BoneId boneindex;
            Enum.TryParse(temp, out boneindex);
            myBoneCapsule.Add(new OVRBoneCapsule((short)boneindex, myCapsules[i], myCapsules[i].transform.GetComponentInChildren<CapsuleCollider>()));

        }
        isinit = true;
        yield return null;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isinit)
        {
            for (int i = 0; i < myBoneCapsule.Count; ++i)
            {
                OVRBoneCapsule capsule = myBoneCapsule[i];
                var capsuleGO = capsule.CapsuleRigidbody.gameObject;

                if (skeleton.IsDataValid)
                {
                    Transform bone = traceBones.myBones[(int)capsule.BoneIndex];
        
                    if (capsuleGO.activeSelf)
                    {
                        capsule.CapsuleRigidbody.MovePosition(bone.position);
                        capsule.CapsuleRigidbody.MoveRotation(bone.rotation);
                    }
                    else
                    {
                        capsuleGO.SetActive(true);
                        capsule.CapsuleRigidbody.position = bone.position;
                        capsule.CapsuleRigidbody.rotation = bone.rotation;
                    }
                }
                else
                {
                    if (capsuleGO.activeSelf)
                    {
                        capsuleGO.SetActive(false);
                    }
                }

            }

            
        }
    }
}
