using UnityEngine;
using System.Collections;

public class BlueBullet : MonoBehaviour
{
    float speed;
    bool hasHit = false; // Flag to check if the bullet has hit something
    private float playerZPosition; // Variable to store the player's current Z position for teleportation
    private Vector3 TeleportOffset;
    Vector3 direction;
    private Transform playerTransformComponent; // Reference to the player transform, needed for teleportation
    private Vector3 CurrentContactPoint;
    private Vector3 CurrentContactPointNormal; // Variable to store the normal of the contact point
    private Rigidbody rb;

    int count = 0; // Counter for the number of collisions
    int lastProcessedFrame = -1; // Variable to track the last processed frame

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the bullet
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Set the collision detection mode to Continuous Dynamic for better collision handling
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Set interpolation for smoother movement
        rb.drag = 0f; // Set drag to 0 for no air resistance
        rb.angularDrag = 0f; // Set angular drag to 0 for no rotational resistance

        Destroy(gameObject, 5f); // Destroy the bullet after 5 seconds if it has not collided with anything
    }

    private void OnCollisionEnter(Collision collision)
    {

        // Check if the collision is processed in the same frame to avoid multiple calls
        if (Time.frameCount == lastProcessedFrame)
        {
            return; // If the collision is already processed in this frame, exit the method
        }

        lastProcessedFrame = Time.frameCount; // Update the last processed frame to the current frame

        count++; // Increment the collision counter

        if (collision.gameObject.CompareTag("Ground") && collision.contactCount > 0 && count == 1)
        {
            hasHit = true; // Set the hasHit flag to true when the bullet collides with the ground for the first time

            CurrentContactPoint = collision.contacts[0].point; // Get the first contact point coordinates of the collision

            CurrentContactPointNormal = collision.contacts[0].normal; // Get the normal of the first contact point

            Bounce(rb.velocity, collision.contacts[0].normal); // Call the Bounce method to handle the first contact, // passing the bullet's velocity and the normal of the contact point(collision angle)

            Destroy(gameObject, 3f); // Destroy the bullet after 5 seconds if it has not collided with the ground again
        }

        if (collision.gameObject.CompareTag("Ground") && collision.contactCount > 0 && count == 2)
        {
            hasHit = true; // Set the hasHit flag to true when the bullet collides with the ground for the second time

            CurrentContactPoint = collision.contacts[0].point; // Get the second contact point coordinates of the collision
            CurrentContactPointNormal = collision.contacts[0].normal; // Get the normal of the second contact point
            Destroy(gameObject); // Destroy the bullet after the second contact
        }
        Debug.Log($"Hit {collision.gameObject.name} | Velocity: {rb.velocity} | Frame: {Time.frameCount} | Count: {count}");

    }

    void Bounce(Vector3 velocity, Vector3 bounceAngle)
    {
        if (bounceAngle == Vector3.zero || velocity == Vector3.zero) // Check if the bounce angle is zero
            return; // If the bounce angle is zero, exit the method to prevent division by zero or invalid reflection calculations

        direction = Vector3.Reflect(velocity.normalized, bounceAngle.normalized); // Calculate the new direction of the bullet after bouncing off the wall
        speed = Mathf.Max(velocity.magnitude, 2f); // keep enough bounce power

        if (Mathf.Abs(bounceAngle.y) > 0.95f && Mathf.Abs(direction.y) < 0.1f) // Check if the bounce angle is steep enough
        {
            direction.y = 0.5f; // Adjust the y component of the direction to prevent the bullet from getting stuck in the ground
            direction.Normalize();
        }

        Vector3 newVelocity = direction * speed; // Calculate the new velocity based on the direction and speed

        rb.velocity = Vector3.zero; // Reset the velocity to prevent the bullet from continuing in its previous direction
        rb.angularVelocity = Vector3.zero; // Reset the angular velocity to prevent spinning
        rb.AddForce(newVelocity, ForceMode.VelocityChange); // Apply the new velocity to the bullet using VelocityChange mode
    }

    void OnDestroy()
    {
        if (hasHit)
        {
            Teleport(CurrentContactPoint, CurrentContactPointNormal); // Call the Teleport method when the bullet is destroyed
        }
    }

    void Teleport(Vector3 coordinates, Vector3 normal)
    {
        SetTeleportOffset(normal); // Call the SetTeleportOffset method to set the teleport offset
        Vector3 teleportPosition = coordinates + TeleportOffset; // Calculate the teleport position by adding the offset to the coordinates

        teleportPosition.z = playerZPosition; // Ensure the z-coordinate is set to the player's current z position to prevent the player from teleporting sideways

        playerTransformComponent.position = teleportPosition; // Teleport the player to the specified coordinates with an offset that prevents the player from getting stuck in the ground
    }

    void SetTeleportOffset(Vector3 currentTeleportNormal) // Method to set the teleport offset
    {
        Vector3 newOffset;

        if (Vector3.Dot(currentTeleportNormal, Vector3.up) > 0.9f)
        {
            newOffset = new Vector3(0, 1, 0);
        }
        else if (Vector3.Dot(currentTeleportNormal, Vector3.down) > 0.9f)
        {
            newOffset = new Vector3(0, -1, 0);
        }
        else if (Vector3.Dot(currentTeleportNormal, Vector3.right) > 0.9f)
        {
            newOffset = new Vector3(1, 0, 0);
        }
        else if (Vector3.Dot(currentTeleportNormal, Vector3.left) > 0.9f)
        {
            newOffset = new Vector3(-1, 0, 0);
        }
        else
        {
            newOffset = currentTeleportNormal.normalized * 0.5f; // Default offset if no specific direction is matched
        }

        TeleportOffset = newOffset; // Set the teleport offset to the specified value
    }

    public void SetPlayer(Transform playerTransform)
    {
        playerTransformComponent = playerTransform; // Set the player transform to the specified transform
        playerZPosition = playerTransform.position.z;
    }
}