using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    public static RightHand instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject player;
    public GameObject rightHandController;

    public float rotateSpeed = 150;

    public Animator anim;
    public LineRenderer lr;

    float ry;

    bool isSound = true;
    
    public bool frist_motion = false;

    public enum State
    {
        Idle,
        Grab
    }
    public State state;

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                //Idle_Preparations();
                break;

            case State.Grab:
                //Idle_Aiming();
                break;
        }
        if (RYS.instance.agent.isStopped == true)
        {
            PlayerRotate();
        }

        GrabJudgment();
    }
    void PlayerRotate()
    {
        Vector2 primaryAxis_Rotate = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        ry += primaryAxis_Rotate.x * rotateSpeed * Time.deltaTime;

        if (frist_motion == false)
        {
            player.transform.eulerAngles = new Vector3(0, ry, 0);
        }
        else if (frist_motion == true)
        {
            player.transform.eulerAngles = new Vector3(0, ry + 90, 0);
        }
    }

    void GrabJudgment()
    {
        if (state == State.Idle)
        {

            Ray ray = new Ray(rightHandController.transform.position, rightHandController.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                lr.enabled = true;
                lr.SetPosition(0, rightHandController.transform.position);
                lr.SetPosition(1, hitInfo.point);

                if (hitInfo.transform.tag == "Tv_Controller")
                {
                    AudioSource audioSource = LobbySetting.instance.TV_Controller.GetComponent<AudioSource>();

                    if (!audioSource.isPlaying && isSound == true)
                    {
                        audioSource.Play();
                        isSound = false;
                    }
                    hitInfo.transform.parent.GetChild(2).GetChild(4).GetComponent<Light>().color = Color.green;
                    /// 여기까지
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                    {
                        TriggerVibration(40, 1, 255, OVRInput.Controller.RTouch);

                        if (RYS.instance.agent.isStopped == true && frist_motion == false)
                        {
                            RYS.instance.agent.isStopped = false; // 여기까지 실행
                        }
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////
                else if (hitInfo.transform.name == "hammer")
                {
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                    {
                        anim.SetTrigger("Grab");
                        state = State.Grab;

                        Destroy(hitInfo.transform.GetComponent<Rigidbody>());

                        hitInfo.transform.parent = rightHandController.transform;

                        rightHandController.transform.GetChild(2).transform.localPosition = new Vector3(-0.055f, 0.112f, -0.024f);
                        rightHandController.transform.GetChild(2).transform.localEulerAngles = new Vector3(-5.92f, 160.076f, 52.366f);

                        lr.enabled = false;
                    }
                }
               
            }
            else
            {
                lr.enabled = false;
                isSound = true;
                LobbySetting.instance.TV_Controller.transform.GetChild(2).GetChild(4).GetComponent<Light>().color = new Color(1.000f, 0.429f, 0.286f, 1.000f);
            }
        }
        else if (state == State.Grab)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                anim.SetTrigger("Idle");
                state = State.Idle;

                rightHandController.transform.GetChild(2).gameObject.AddComponent<Rigidbody>();
                rightHandController.transform.GetChild(2).transform.parent = null;

                lr.enabled = true;
            }
        }
    }
    public void TriggerVibration(int iteration, int frequency, int strength, OVRInput.Controller controller)
    {
        OVRHapticsClip clip = new OVRHapticsClip();

        for(int i=0; i<iteration; i++)
        {
            clip.WriteSample(i % frequency == 0 ? (byte)strength : (byte)0);
        }
        if (controller == OVRInput.Controller.RTouch)
        {
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }
}
