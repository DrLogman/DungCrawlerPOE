
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothnessFactor;
    public Vector3 minValues, maxValues;

    private void FixedUpdate()
    {
        Follow();
    }

  
    void Follow()
    {
        Vector3 targetPosition = player.position + offset;
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));
      
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, boundPosition, smoothnessFactor*Time.fixedDeltaTime);
        transform.position = smoothedPosition;
    }
}
