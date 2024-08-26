using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class VirtualCameraController : MonoBehaviour
{
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject ground;
    [SerializeField] float zoomFactor = 2;
    [SerializeField] float minDistance = 20;
    [SerializeField] float maxDistance = 50;
    private CinemachineVirtualCamera vcam;


    // Start is called before the first frame update
    void Start()
    {
        //get reference tot he cinmachine virtual camera
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rocket == null || ground == null)
        {
            return;
        }
        //set vcam distance based on rocket distance to the ground, vcam body is framing transposer
        //camera distance should go from 20 to 40 based on rocket distance 
        var body = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        float cameraDistance = Mathf.Clamp(rocket.transform.position.y * zoomFactor, minDistance, maxDistance);
        body.m_CameraDistance = cameraDistance;
    }
}
