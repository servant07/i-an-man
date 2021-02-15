using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKU_TestMovePosition : MonoBehaviour
{
    Rigidbody rigidbody;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();   
    }

    private void FixedUpdate()
    {

        rigidbody.WakeUp();
        rigidbody.MovePosition(target.position);
        rigidbody.MoveRotation(target.rotation);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
