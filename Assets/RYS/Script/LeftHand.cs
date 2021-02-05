using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    public GameObject Player;
    public float moveSpeed = 5;

    CharacterController pC = null;

    public Animator anim;


    void Start()
    {
        pC = Player.GetComponent<CharacterController>();
    }

    void Update()
    {

        if (RYS.instance.agent.isStopped == true)
        {
            PlayerMove();
        }
        ///////////////////////////////////////////
        ///
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            anim.SetTrigger("Grab");
        }
        ///
        ///////////////////////////////////////////
    }

    void PlayerMove()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        Vector3 dir = new Vector3(primaryAxis.x, 0, primaryAxis.y);

        dir = Camera.main.transform.TransformDirection(dir);

        dir.y = 0;
        dir.Normalize();
        dir *= moveSpeed;

        pC.Move(dir * Time.deltaTime); 
    }
}
