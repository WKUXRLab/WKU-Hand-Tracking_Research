using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GetJointRotation : MonoBehaviour
{
    public GameObject Dial;
    HingeJoint hingedJoint;
    ConfigurableJoint configJoint;
    private float Rotation;

    //newVars

    private float _lastDegrees = 0;
    private float _lastSnapDegrees = 0;

    void Update()
    {
        float degrees = getSmoothedValue(transform.localEulerAngles.y);

        if (Dial.transform.eulerAngles.x <= 180f)
        {
            Rotation = Dial.transform.eulerAngles.x;
        }
        else
        {
            Rotation = Dial.transform.eulerAngles.x - 360f;
        }
        Debug.Log("The degrees of the dial are: " + degrees);
    }

    float getSmoothedValue(float val)
    {
        if (val < 0)
        {
            val = 360 - val;
        }
        if (val == 360)
        {
            val = 0;
        }

        return val;
    }
}
