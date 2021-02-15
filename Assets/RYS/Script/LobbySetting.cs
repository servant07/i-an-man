using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySetting : MonoBehaviour
{
    public static LobbySetting instance;
    private void Awake()
    {
        instance = this;
    }

    public Text main_Text, controller_Text;

    public GameObject TV_Controller, TV_HandTracking, ch_Controller_R, ch_Controller_L;
    public GameObject neon;

    public Light Dr;

    public Canvas tutorial_HandCanvas, tutorial_PlayerCanvas;
    public bool tutorial_Canvas_judgment = false;


    float objectRotate_y;
    float gameTime = 0;

    public bool Controller_Texture_judgment = false;

    void Start()
    {
        neon.SetActive(false);

        tutorial_HandCanvas.enabled = false;
        tutorial_PlayerCanvas.enabled = false;

        //controller_Text.enabled = false;
    }

    void Update()
    {
        if(Controller_Texture_judgment == true)
        {
            Controller_Texture();
        }
        Tv_Texture();
        Rotate_Robby();
    }

    private void Controller_Texture()
    {
       // controller_Text.enabled = true;
    }

    void Rotate_Robby() 
    {
        objectRotate_y += Time.deltaTime * 40;
        TV_Controller.transform.rotation = Quaternion.Euler(0, objectRotate_y, 0);
        TV_HandTracking.transform.rotation = Quaternion.Euler(0, objectRotate_y, 0);

        ch_Controller_R.transform.rotation = Quaternion.Euler(0, -objectRotate_y + 60 , 0);
        ch_Controller_L.transform.rotation = Quaternion.Euler(0, objectRotate_y + 120 , 0);
    }
    void Tv_Texture()
    {
 
        gameTime += Time.deltaTime;

        if (gameTime < 4.8)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * 2));
            main_Text.color = new Color(1, 1, 1, 1) * flicker;

            if (Mathf.Round(gameTime) == 2)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if(4.8 < gameTime && gameTime < 8.5)
        {
            float flicker = 0;
            main_Text.text = "Please select a device";

            flicker = Mathf.Abs(Mathf.Sin(Time.time * 2));
            main_Text.color = new Color(1, 1, 1, 1) * flicker;

            if (Mathf.Round(gameTime) == 5)
            {
                gameObject.GetComponent<AudioSource>().clip = Resources.Load("Please select a device") as AudioClip;
                gameObject.GetComponent<AudioSource>().Play();
            }

            if (tutorial_Canvas_judgment == false)
            {
                tutorial_HandCanvas.enabled = true;
                tutorial_PlayerCanvas.enabled = true;
            }

            Dr.enabled = false;
        }
    }
}
