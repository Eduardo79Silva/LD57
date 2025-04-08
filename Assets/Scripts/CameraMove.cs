using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;

    // New variables for vertical movement.
    public float verticalSpeed = 5f;
    public float fastMultiplier = 2f;

    private void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        // Handle keyboard vertical movement.
        float currentSpeed = verticalSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= fastMultiplier;
        }
        if (Input.GetKey(KeyCode.W))
        {
            // Cap the height to the original camera height.
            Camera.main.transform.position += Vector3.up * currentSpeed * Time.deltaTime;
            if (Camera.main.transform.position.y > ResetCamera.y)
            {
                Camera.main.transform.position = new Vector3(
                    Camera.main.transform.position.x,
                    ResetCamera.y,
                    Camera.main.transform.position.z
                );
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            // Cap the height to 10x the original camera height.
            if (Camera.main.transform.position.y > -108f)
            {
                Camera.main.transform.position += Vector3.down * currentSpeed * Time.deltaTime;
            }
        }

        // Reset camera position when space is pressed.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
}
