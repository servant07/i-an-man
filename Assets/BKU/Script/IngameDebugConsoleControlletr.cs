using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameDebugConsoleControlletr : MonoBehaviour
{
    public GameObject root;
    public GameObject popup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        popup.GetComponent<RectTransform>().position = root.GetComponent<RectTransform>().position;
    }
}
