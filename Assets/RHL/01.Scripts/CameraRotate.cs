using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float speed = 200f;

    float rx = 0, ry;

    // Start is called before the first frame update
    void Start()
    {
        rx = transform.eulerAngles.x;
        ry = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        rx += my * speed * Time.deltaTime;
        ry += mx * speed * Time.deltaTime;

        rx = Mathf.Clamp(rx, -65, 65);
        transform.eulerAngles = new Vector3(-rx, ry, 0);
    }

}
