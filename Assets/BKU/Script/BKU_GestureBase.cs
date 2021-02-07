using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BKU_GestureBase : MonoBehaviour
{
    
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    protected abstract void GestureStart();
    protected abstract void GestureStay();
    protected abstract void GestureEnd();
}
