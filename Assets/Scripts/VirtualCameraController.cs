using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class VirtualCameraController : MonoBehaviour
{
    [SerializeField] GameObject rocket;
    [SerializeField] float minDistance = 20;
    [SerializeField] float maxDistance = 65;
    [SerializeField] float maxRocketSpeed = 35;
    private CinemachineVirtualCamera vcam;
    private Rigidbody rocketRB;



    // Start is called before the first frame update
    void Start()
    {
        //get reference tot he cinmachine virtual camera
        vcam = GetComponent<CinemachineVirtualCamera>();
        if (rocket != null)
        { 
            rocketRB = rocket.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rocket == null || rocketRB == null)
        {
            return;
        }
        //set vcam distance based on rocket distance to the ground, vcam body is framing transposer
        //camera distance should go from 20 to 40 based on rocket distance 
        var body = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        float speed = rocketRB.velocity.magnitude;
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, speed / maxRocketSpeed);
        float newCameraDistance = body.m_CameraDistance;
        if (targetDistance < body.m_CameraDistance)
        {
            //change distance slower according to time
            newCameraDistance = Mathf.Lerp(newCameraDistance, targetDistance, Time.deltaTime / 3);
        }
        else
        {
            //change distance faster according to time
            newCameraDistance = Mathf.Lerp(newCameraDistance, targetDistance, Time.deltaTime / 2);
        }

        body.m_CameraDistance = newCameraDistance;
    }
}
