using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    Vector2 input;

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
        GetInput();
        MoveRocket();
    }

    private void HandleGameTestingKeys()
    {
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     LandRocket();
        // }

        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     LandingPad lp = FindObjectOfType<LandingPad>();
        //     if (lp)
        //     {
        //         transform.position = new Vector3(lp.transform.position.x, lp.transform.position.y + 3, lp.transform.position.z);
        //     }
        // }
    }

    private void GetInput()
    {
        // //determine which input type is used by the user. On Android do not use this method
        // if (mobileInputUsed || 
        //     Application.platform == RuntimePlatform.Android || 
        //     Application.platform == RuntimePlatform.IPhonePlayer)
        // {
        //     return;
        // }

        // float vertical = Input.GetAxis("Vertical");
        // float horizontal = Input.GetAxis("Horizontal");
        
        // input.x = horizontal;
        // input.y = vertical;
    }

    public void OnMoveRocket(InputValue value)
    {
        // //call the move rocket function with the input value
        // input = value.Get<Vector2>();
        // CalibrateInput();
    }

    public void OnMoveUpDown(InputValue value)
    {
        //call the move rocket function with the input value
        var inputLeftRight = value.Get<Vector2>();
        input.y = inputLeftRight.y;
        CalibrateInput();
    }

    public void OnMoveLeftRight(InputValue value)
    {
        //call the move rocket function with the input value
        var inputLeftRight = value.Get<Vector2>();
        input.x = inputLeftRight.x;
        CalibrateInput();
    }

    private void CalibrateInput()
    {
        float inputThreshold = 0.01f;
        if (input.x > inputThreshold || input.x < -inputThreshold)
        {
            //apply easing to the horizontal input to make it less sensitive
            //input.x = /*MathF.Sign(input.x) **/ MathF.Pow(input.x, 3);
        }
        else
        {
            input.x = 0;
        }

        if (input.y > inputThreshold || input.y < -inputThreshold)
        {
            //apply easing to the horizontal input to make it less sensitive
            //input.y = MathF.Pow(input.y, 3); // * MathF.Sign(input.y);
        }
        else
        {
            input.y = 0;
        }

        Debug.Log("Input: " + input);
    }

    private void MoveRocket()
    {
        if (!ControlsEnabled)
        {
            return;
        }

        //count the seconds when there is vertical or horizontal input to take fuel accordingly
        if (input.y != 0 || input.x != 0)
        {
            float fuelConsumptionRate = GameManager.Instance.PlayerSettings.FuelConsumptionRate * Time.deltaTime;
            float inputMagnitude = input.magnitude;
            playerHealth.TakeFuel(fuelConsumptionRate * inputMagnitude);
        }

        if (input.y < 0)
        {
            verticalBoost = input.y * verticalBoostAmount * Time.deltaTime * 10 * 0.5f;
            topLeftBooster.transform.localRotation = Quaternion.Euler(45, 90, 0);
            topRightBooster.transform.localRotation = Quaternion.Euler(45, -90, 0);
        }
        else
        {
            verticalBoost = input.y * verticalBoostAmount * Time.deltaTime * 10;
            topLeftBooster.transform.localRotation = Quaternion.Euler(-28, 90, 0);
            topRightBooster.transform.localRotation = Quaternion.Euler(-28, -90, 0);
        }

        horizontalBoost = input.x * horizontalBoostAmount * Time.deltaTime * 10;

        //apply force to rocket
        if (verticalBoost != 0)
        {
            rbBody.AddRelativeForce(Vector3.up * verticalBoost);
        }

        if (horizontalBoost != 0)
        {
            rbTop.AddRelativeForce(Vector3.right * horizontalBoost);
        }
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
        if (input.y > 0)
        {
            if (!thrustersAudio.isPlaying)
                thrustersAudio.Play();
            if (!mainBoosterParticles.isPlaying)
                mainBoosterParticles.Play();
        }
        else if (input.y == 0)
        {
            if (thrustersAudio.isPlaying)
                thrustersAudio.Stop();

            if (mainBoosterParticles.isPlaying)
                mainBoosterParticles.Stop();
        }
        else
        {
            if (!thrustersAudio.isPlaying)
                thrustersAudio.Play();

            if (!rightBoosterParticles.isPlaying)
                rightBoosterParticles.Play();

            if (!leftBoosterParticles.isPlaying)
                leftBoosterParticles.Play();
        }

        if (input.x != 0)
        {
            if (!thrustersTopAudio.isPlaying)
                thrustersTopAudio.Play();

            if (input.x < 0)
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
        else if (input.y >= 0)
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
