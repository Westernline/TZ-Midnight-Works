using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second

    void Update()
    {
        // Calculate the rotation amount based on the rotation speed and time since the last frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply the rotation around the y-axis
        transform.Rotate(0, 0 , rotationAmount);
    }
}
