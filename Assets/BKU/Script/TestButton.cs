using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestButton : MonoBehaviour
{
    Rigidbody rigidbody;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {



    }
    private void OnTriggerStay(Collider other)
    {
       
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
