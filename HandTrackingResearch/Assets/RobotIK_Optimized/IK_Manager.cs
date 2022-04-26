using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Manager : MonoBehaviour
{
    public Transform TargetTransform;
    public RobotJoint[] Joints;
    public float[] Angles;


    private float SamplingDistance = 5f;

    private float LearningRate = 100f;

    private float DistanceThreshold = 0.0001f;

    // Start is called before the first frame update
    void Start()
    {
        Angles = new float[Joints.Length];

        for (int i = 0; i < Joints.Length; i++)
        {
            //Debug.Log(Joints.Length);
            if (Joints[i].Axis.x == 1)
            {
                Angles[i] = Joints[i].transform.localRotation.eulerAngles.x;
            }
            else if (Joints[i].Axis.y == 1)
            {
                Angles[i] = Joints[i].transform.localRotation.eulerAngles.y;
            }
            else if (Joints[i].Axis.z == 1)
            {
                Angles[i] = Joints[i].transform.localRotation.eulerAngles.z;
            }
        }
        Debug.Log(Angles[0]);
    }

    // Update is called once per frame
    void Update()
    {
        InverseKinematics(TargetTransform.position, Angles);
        
    }
    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }
        return prevPoint;
    }
    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }
    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];
        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);
        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);
        float gradient = (f_x_plus_d - f_x) / SamplingDistance;
        // Restores
        angles[i] = angle;
        return gradient;
    }
    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
            return;
        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;
            // Clamp
            angles[i] = Mathf.Clamp(angles[i], Joints[i].MinAngle, Joints[i].MaxAngle);
            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;


            if (Joints[i].Axis.x == 1)
            {
                Joints[i].transform.localEulerAngles = new Vector3(angles[i], 0, 0);
            }
            else if (Joints[i].Axis.y == 1)
            {
                Joints[i].transform.localEulerAngles = new Vector3(0, angles[i], 0);
            }
            else if (Joints[i].Axis.z == 1)
            {
                Joints[i].transform.localEulerAngles = new Vector3(0, 0, angles[i]);
            }
        }
    }
}
