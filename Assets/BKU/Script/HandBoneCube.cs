using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandBoneCube : MonoBehaviour
{
    public GameObject cubeprefab;
    public OVRHand hand;
    public OVRSkeleton skeleton;
    List<OVRBone> bones;
    Transform[] cubes;
    bool IsInitialized;

    Transform skeletonTransform;
    Transform transform;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(skeleton.IsInitialized == true && !IsInitialized)
        {
            transform = GetComponent<Transform>();
            skeletonTransform = skeleton.transform;
            cubes = new Transform[24];
            bones = new List<OVRBone>();

            for(int i=0;i<skeleton.Bones.Count;i++)
            {
                bones.Add(skeleton.Bones[i]);

            }
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i] = Instantiate(cubeprefab).transform;
                cubes[i].transform.parent = transform;
            }
            IsInitialized = true;
        }

        if(skeleton.IsInitialized && skeleton.IsDataValid)
        {
            //transform.localPosition = skeletonTransform.position;
            //transform.localRotation = skeletonTransform.rotation;
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].position = bones[i].Transform.position;
                cubes[i].rotation = bones[i].Transform.rotation;
                cubes[i].GetComponentInChildren<Text>().text = bones[i].Id.ToString();
            }
        }


    }

    private void LateUpdate()
    {
        
    }
}
