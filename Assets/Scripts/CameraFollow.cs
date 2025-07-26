using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -7);

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 playerPosition = player.TransformPoint(offset);

        transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * followSpeed);

        transform.LookAt(player.position);
    }
}
