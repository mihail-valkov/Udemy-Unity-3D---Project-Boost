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
    private float verticalBoost;
    private float horizontalBoost;
    private Rigidbody rbBody;
    private Rigidbody rbTop;

    void Awake()
    {
        //Get rigidbody from child RocketBody
        rbBody = rocketBody.GetComponent<Rigidbody>();
        rbTop = rocketTopGameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        //prevent rocket from rotating on y axis
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);

        //track up and side (x axis) movement with keybord and give rocket force/boost
        verticalBoost = Input.GetAxis("Vertical") * verticalBoostAmount * Time.deltaTime * 10;
        horizontalBoost = Input.GetAxis("Horizontal") * horizontalBoostAmount * Time.deltaTime * 10;

        //apply force to rocket
        rbBody.AddRelativeForce(Vector3.up * verticalBoost);
        rbTop.AddRelativeForce(Vector3.right * horizontalBoost);
    }
}
