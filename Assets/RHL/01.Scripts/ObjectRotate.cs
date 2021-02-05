using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public float speedRotate = 200;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

    Vector3 dir = Vector3.zero;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        OnRotateVec();
        transform.Rotate(dir * speedRotate * Time.deltaTime);
    }

    void OnRotateVec()
    {
        dir = Vector3.zero;

        if (rotateX)
            dir += Vector3.right;

        if (rotateY)
            dir += Vector3.up;

        if (rotateZ)
            dir += Vector3.forward;
    }
}
