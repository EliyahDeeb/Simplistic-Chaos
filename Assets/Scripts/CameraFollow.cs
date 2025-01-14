using UnityEngine;

public class CameraFollow : MonoBehaviour  
{
    public Transform player;  // Reference to the player
    public Vector3 offset = new Vector3(0f, 1f, -10f);  // Camera offset
    public float smoothSpeed = 0.125f;  // How smooth the camera follows the player

    void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the target position
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera to the target position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
