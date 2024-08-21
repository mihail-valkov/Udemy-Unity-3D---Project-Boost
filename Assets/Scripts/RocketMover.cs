using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class RocketMover : MonoBehaviour
{
    [SerializeField] float verticalBoostAmount = 10f;
    [SerializeField] float horizontalBoostAmount = 5f;
    [SerializeField] GameObject rocketBody;
    [SerializeField] GameObject rocketTopGameObject;
    [SerializeField] AudioClip[] hitClips;

    float verticalBoost;
    float horizontalBoost;
    Rigidbody rbBody;
    Rigidbody rbTop;

    AudioSource thrustersAudio;
    AudioSource thrustersTopAudio;
    AudioSource hitsAudio;

    void Awake()
    {
        //Get rigidbody from child RocketBody
        rbBody = rocketBody.GetComponent<Rigidbody>();
        rbTop = rocketTopGameObject.GetComponent<Rigidbody>();

        //Get audio source from child RocketBody
        thrustersAudio = rocketBody.GetComponent<AudioSource>();
        thrustersTopAudio = rocketTopGameObject.GetComponent<AudioSource>();
        hitsAudio = GetComponent<AudioSource>();

        hitsAudio.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //enable the hits audio source after a while to avoid playing the sound on start
        Invoke("EnableHitsAudio", 1f);
    }

    void EnableHitsAudio()
    {
        hitsAudio.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        verticalBoost = Input.GetAxis("Vertical") * verticalBoostAmount * Time.deltaTime * 10;
        horizontalBoost = Input.GetAxis("Horizontal") * horizontalBoostAmount * Time.deltaTime * 10;

        //apply force to rocket
        rbBody.AddRelativeForce(Vector3.up * verticalBoost);
        rbTop.AddRelativeForce(Vector3.right * horizontalBoost);
    }

    void FixedUpdate()
    {
        //play audio when moving
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!thrustersAudio.isPlaying)
                thrustersAudio.Play();
        }
        else
        {
            if (thrustersAudio.isPlaying)
                thrustersAudio.Stop();
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            if (!thrustersTopAudio.isPlaying)
                thrustersTopAudio.Play();
        }
        else
        {
            if (thrustersTopAudio.isPlaying)
                thrustersTopAudio.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlaySoundOnCollision(collision);
    }

    public void PlaySoundOnCollision(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" || 
            collision.gameObject.tag == "Ground" || 
            collision.gameObject.tag == "Finish")
        {
            //play hit sound`
            if (hitsAudio.enabled)
                hitsAudio.PlayOneShot(hitClips[Random.Range(0, hitClips.Length)]);
        }
    }
}
