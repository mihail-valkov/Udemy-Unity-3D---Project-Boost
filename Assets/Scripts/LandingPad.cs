using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPad : MonoBehaviour
{
    private bool rocketTouching;
    RocketMover rocket;

    // Start is called before the first frame update

    void Awake()
    {
        rocket = FindObjectOfType<RocketMover>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Rocket")
        {
            rocketTouching = true;
            Debug.Log("Rocket collision " + rocketTouching);
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Rocket")
        {
            rocketTouching = false;
            Debug.Log("Rocket collision" + rocketTouching);
        }
    }

    void Update()
    {
        //if rocket is touching the landing pad,and is upright and velocity is less than 1, then land the rocket
        if (rocketTouching && rocket.IsUpright() && rocket.GetVelocity() < 0.05f)
        {
            rocket.LandRocket();
        }
    }
}
