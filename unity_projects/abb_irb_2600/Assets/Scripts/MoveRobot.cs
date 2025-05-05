using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRobot : MonoBehaviour
{
    [Header("Robot Configuration")]
    [SerializeField] private RobotLinks robotLinks;

    [Header("Joint Rotations (Degrees)")]
    [SerializeField] private float link1_rotation;
    [SerializeField] private float link2_rotation;
    [SerializeField] private float link3_rotation;
    [SerializeField] private float link4_rotation;
    [SerializeField] private float link5_rotation;
    [SerializeField] private float link6_rotation;

    [Header("Invert Rotation Direction")]
    [SerializeField] private bool invert1;
    [SerializeField] private bool invert2;
    [SerializeField] private bool invert3;
    [SerializeField] private bool invert4;
    [SerializeField] private bool invert5;
    [SerializeField] private bool invert6;

    [Header("Rotation Offsets (Degrees)")]
    [SerializeField] private float offset1;
    [SerializeField] private float offset2;
    [SerializeField] private float offset3;
    [SerializeField] private float offset4;
    [SerializeField] private float offset5;
    [SerializeField] private float offset6;

    [Header("Smoothing")]
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothingSpeed = 10f;

    [System.Serializable]
    public class RobotLinks
    {
        public GameObject link1;
        public GameObject link2;
        public GameObject link3;
        public GameObject link4;
        public GameObject link5;
        public GameObject link6;
    }

    public void SetLinkRotation(int linkIndex, float data)
    {
        switch (linkIndex)
        {
            case 1:
                link1_rotation = data;
                break;
            case 2:
                link2_rotation = data;
                break;
            case 3:
                link3_rotation = data;
                break;
            case 4:
                link4_rotation = data;
                break;
            case 5:
                link5_rotation = data;
                break;
            case 6:
                link6_rotation = data;
                break;
            default:
                Debug.LogError("Invalid link index");
                break;
        }
    }

    void Update()
    {
        float r1 = offset1 + link1_rotation * (invert1 ? -1 : 1);
        float r2 = offset2 + link2_rotation * (invert2 ? -1 : 1);
        float r3 = offset3 + link3_rotation * (invert3 ? -1 : 1);
        float r4 = offset4 + link4_rotation * (invert4 ? -1 : 1);
        float r5 = offset5 + link5_rotation * (invert5 ? -1 : 1);
        float r6 = offset6 + link6_rotation * (invert6 ? -1 : 1);

        if (smooth)
        {
            robotLinks.link1.transform.localRotation = Quaternion.Lerp(robotLinks.link1.transform.localRotation, Quaternion.Euler(0, 0, r1), Time.deltaTime * smoothingSpeed);
            robotLinks.link2.transform.localRotation = Quaternion.Lerp(robotLinks.link2.transform.localRotation, Quaternion.Euler(0, r2, 0), Time.deltaTime * smoothingSpeed);
            robotLinks.link3.transform.localRotation = Quaternion.Lerp(robotLinks.link3.transform.localRotation, Quaternion.Euler(0, r3, 0), Time.deltaTime * smoothingSpeed);
            robotLinks.link4.transform.localRotation = Quaternion.Lerp(robotLinks.link4.transform.localRotation, Quaternion.Euler(r4, 0, 0), Time.deltaTime * smoothingSpeed);
            robotLinks.link5.transform.localRotation = Quaternion.Lerp(robotLinks.link5.transform.localRotation, Quaternion.Euler(0, r5, 0), Time.deltaTime * smoothingSpeed);
            robotLinks.link6.transform.localRotation = Quaternion.Lerp(robotLinks.link6.transform.localRotation, Quaternion.Euler(r6, 0, 0), Time.deltaTime * smoothingSpeed);
        }
        else
        {
            robotLinks.link1.transform.localRotation = Quaternion.Euler(0, 0, r1);
            robotLinks.link2.transform.localRotation = Quaternion.Euler(0, r2, 0);
            robotLinks.link3.transform.localRotation = Quaternion.Euler(0, r3, 0);
            robotLinks.link4.transform.localRotation = Quaternion.Euler(r4, 0, 0);
            robotLinks.link5.transform.localRotation = Quaternion.Euler(0, r5, 0);
            robotLinks.link6.transform.localRotation = Quaternion.Euler(r6, 0, 0);
        }
    }

}
