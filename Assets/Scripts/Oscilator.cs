using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(5f, 0f, 0f);
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0f, 0f);
    [SerializeField] float period = 2f;
    [SerializeField] bool isCentered = false; 

    float movementFactor;
    float rotationFactor;

    Vector3 startingPos;
    Vector3 startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        startingRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; //grows continually from 0

        const float tau = Mathf.PI * 2; //about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to 1

        if (isCentered)
        {
            movementFactor = rawSinWave / 2f;
            //rotationFactor = rawSinWave / 2f;
        }
        else
        {
            movementFactor = (rawSinWave + 1f) / 2f;
        }

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;

        rotationFactor = (rawSinWave) / 2f;

        if (rotationVector != Vector3.zero)
        {
            //rotation vector is amount in degrees for each axis 
            Vector3 rotation = rotationVector * rotationFactor / 36f;
            transform.Rotate(rotation);
        }
    }
}
