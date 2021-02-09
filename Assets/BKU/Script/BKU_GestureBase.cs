using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BKU_GestureBase : MonoBehaviour
{

    protected BKU_GestureInput gestureInput;
    protected Func<bool> Gesture;
    protected OVRSkeleton skeleton;
    protected bool gestureActive;


    protected void Init(BKU_GestureInput gestureInput, Func<bool> Gesture)
    {
        this.gestureInput = gestureInput;
        this.Gesture = Gesture;
        skeleton = gestureInput.Skeleton;
    }

    private void Update()
    {
        Activate();
    }
    public void Activate()
    {
        if (gestureInput.IsInitialized)
        {
            Action log = null;

            if (Gesture() && !gestureActive)
            {
                log = GestureStart;

            }
            else if (Gesture() && gestureActive)
            {
                log = GestureStay;

            }
            else if (!Gesture() && gestureActive)
            {
                log = GestureEnd;

            }

            if (log != null)
            {

                log.Invoke();
                Debug.Log(gestureInput.handType + " - " + GetType() + "/" + log.Method.Name);

            }
        }
    }
    protected abstract void GestureStart();
    protected abstract void GestureStay();
    protected abstract void GestureEnd();

}
