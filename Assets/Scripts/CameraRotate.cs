using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotationSpeed;

    private void Update()
    {
        // Calculate the rotation amounts based on time
        float rotationX = Time.deltaTime * rotationSpeed;
        float rotationY = Time.deltaTime * rotationSpeed;

        // Apply the rotation to the camera
        transform.Rotate(-rotationX, rotationY, 0f);
    }
}
