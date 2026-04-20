using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody2D cameraTarget;
    [SerializeField] float cameraZoom = -10;
    [SerializeField] float cameraHeight = 3;
    [SerializeField] float cameraSmoothTime = 1;

    
    // Camera variables
    private Vector3 cameraVelocity = Vector3.zero;

    public void Look(Vector3 lookOffset) 
    {
        Vector3 targetPosition = new Vector3(cameraTarget.position.x, cameraTarget.position.y + cameraHeight, cameraZoom) + lookOffset;
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition, ref cameraVelocity, cameraSmoothTime);
    }
}
