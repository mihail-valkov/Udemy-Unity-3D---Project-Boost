using System;
using UnityEngine;

public class RocketMover : MonoBehaviour
{
    [SerializeField] float verticalBoostAmount = 10f;
    [SerializeField] float horizontalBoostAmount = 5f;
    [SerializeField] GameObject rocketBody;
    [SerializeField] GameObject rocketTopGameObject;
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;

    [SerializeField] GameObject topLeftBooster;
    [SerializeField] GameObject topRightBooster;
    [Range(0, 1)][SerializeField] float rocketUpDotThreshold = 0.94f;

    Health playerHealth;
    private bool isCollisionTracking;
    float verticalBoost;
    float horizontalBoost;
    Rigidbody rbBody;
    Rigidbody rbTop;

    AudioSource thrustersAudio;
    AudioSource thrustersTopAudio;

    public bool ControlsEnabled { get; set; } = true;
    public bool HasLanded { get; private set; }

    void Awake()
    {
        //Get rigidbody from child RocketBody
        rbBody = rocketBody.GetComponent<Rigidbody>();
        rbTop = rocketTopGameObject.GetComponent<Rigidbody>();

        //Get audio source from child RocketBody
        thrustersAudio = rocketBody.GetComponent<AudioSource>();
        thrustersTopAudio = rocketTopGameObject.GetComponent<AudioSource>();
    
        playerHealth = GetComponent<Health>();

        isCollisionTracking = false;
        playerHealth.IsHealthTrackingActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.Instance.PlayerSettings.IsRocketBreakable)
        {
            //find all the fixedjoints within the rocket and set their strength to 1000
            FixedJoint[] fixedJoints = FindObjectsOfType<FixedJoint>();
            foreach(var joint in fixedJoints)
            {
                joint.breakForce = float.PositiveInfinity;
                joint.breakTorque = float.PositiveInfinity;
            }
        }

        //enable the health tracking after a while to avoid playing the sound on start
        Invoke("EnableCollisionTracking", 1f);
    }

    void EnableCollisionTracking()
    {
        isCollisionTracking = true;
        playerHealth.IsHealthTrackingActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameTestingKeys();
        PlayFXOnMove();
        MoveRocket();
    }

    private void HandleGameTestingKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LandRocket();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            LandingPad lp = FindObjectOfType<LandingPad>();
            if (lp)
            {
                transform.position = new Vector3(lp.transform.position.x, lp.transform.position.y + 3, lp.transform.position.z);
            }
        }
    }

    private void MoveRocket()
    {
        if (!ControlsEnabled)
        {
            return;
        }

        //count the seconds when there is vertical or horizontal input to take fuel accordingly
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            playerHealth.TakeFuel(GameManager.Instance.PlayerSettings.FuelConsumptionRate * Time.deltaTime);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            verticalBoost = Input.GetAxis("Vertical") * verticalBoostAmount * Time.deltaTime * 10 * 0.5f;
            topLeftBooster.transform.localRotation = Quaternion.Euler(45, 90, 0);
            topRightBooster.transform.localRotation = Quaternion.Euler(45, -90, 0);
        }
        else
        {
            verticalBoost = Input.GetAxis("Vertical") * verticalBoostAmount * Time.deltaTime * 10;
            topLeftBooster.transform.localRotation = Quaternion.Euler(-28, 90, 0);
            topRightBooster.transform.localRotation = Quaternion.Euler(-28, -90, 0);
        }

        horizontalBoost = Input.GetAxis("Horizontal") * horizontalBoostAmount * Time.deltaTime * 10;

        //apply force to rocket
        rbBody.AddRelativeForce(Vector3.up * verticalBoost);
        rbTop.AddRelativeForce(Vector3.right * horizontalBoost);
    }

    private void PlayFXOnMove()
    {
        if (!ControlsEnabled)
        {
            mainBoosterParticles.Stop();
            leftBoosterParticles.Stop();
            rightBoosterParticles.Stop();
            thrustersAudio.Stop();
            thrustersTopAudio.Stop();
            return;
        }

        //play audio when moving
        if (Input.GetAxis("Vertical") > 0)
        {
            if (!thrustersAudio.isPlaying)
                thrustersAudio.Play();
            if (!mainBoosterParticles.isPlaying)
                mainBoosterParticles.Play();
        }
        else 
        if (Input.GetAxis("Vertical") == 0)
        {
            if (thrustersAudio.isPlaying)
                thrustersAudio.Stop();

            if (mainBoosterParticles.isPlaying)
                mainBoosterParticles.Stop();
        }
        else
        //if (Input.GetAxis("Vertical") < 0)
        {
            if (!thrustersAudio.isPlaying)
                thrustersAudio.Play();

            if (!rightBoosterParticles.isPlaying)
                rightBoosterParticles.Play();

            if (!leftBoosterParticles.isPlaying)
                leftBoosterParticles.Play();
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            if (!thrustersTopAudio.isPlaying)
                thrustersTopAudio.Play();

            if (horizontalInput < 0)
            {
                leftBoosterParticles.Stop();
                if (!rightBoosterParticles.isPlaying)
                    rightBoosterParticles.Play();

            }
            else
            {
                rightBoosterParticles.Stop();
                if (!leftBoosterParticles.isPlaying)
                    leftBoosterParticles.Play();
            }
        }
        else if (Input.GetAxis("Vertical") >= 0)
        {
            if (thrustersTopAudio.isPlaying)
                thrustersTopAudio.Stop();

            leftBoosterParticles.Stop();
            rightBoosterParticles.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    public void HandleCollision(Collision collision)
    {
        if (!isCollisionTracking)
        {
            return;
        }

        if (collision.gameObject.tag == "Obstacle" || 
            collision.gameObject.tag == "Ground")
        {
            playerHealth.TakeDamage(GameManager.Instance.PlayerSettings.HitDamage, collision.contacts[0].point);
        }

        if (collision.gameObject.tag == "Finish")
        {
            //only play hit sound
            playerHealth.TakeDamage(0, collision.contacts[0].point);
        }
    }

    public bool IsUpright()
    {
        return Vector3.Dot(transform.up, Vector3.up) > rocketUpDotThreshold;
    }

    public float GetVelocity()
    {
        return rbBody.velocity.magnitude;
    }

    public void LandRocket()
    {
        if (HasLanded)
        {
            return;
        }

        HasLanded = true;
        Debug.Log("Rocket Landed");
        GameManager.Instance.LevelCompleted();
        playerHealth.IsHealthTrackingActive = false;
    }
}
