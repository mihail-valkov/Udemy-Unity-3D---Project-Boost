using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPart : MonoBehaviour
{
    [SerializeField] RocketMover rocketMover;

    void OnCollisionEnter(Collision collision)
    {
        rocketMover.PlaySoundOnCollision(collision);
    }
}
