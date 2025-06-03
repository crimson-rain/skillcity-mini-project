using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class FreeCam : MonoBehaviour
{
    public InputHandler inputHandler;
    public Transform player; // Reference to the player's transform
    [SerializeField] private float rotationSpeed = 5f; // Speed of camera rotation with mouse
    [SerializeField] private float posDampingPassive = 10f; // Controls smoothness of position 
    [SerializeField] private float posDampingActive = 10f; // Controls smoothness of position when rotating cam
    [SerializeField] private float rotDampingPassive = 10f; // Controls smoothness of  rotation
    [SerializeField] private float rotDampingActive = 10f; // Controls smoothness of rotatio when rotating cam
    [SerializeField] private Vector3 offset; // Initial offset between camera and player
    [SerializeField] private float zoomSpeed = 5f; // Speed of zooming
    [SerializeField] private float camMinDist = 2f; // Minimum camera distance
    [SerializeField] private float camMaxDist = 10f; // Maximum camera distance
    [SerializeField] private float camDistance;
    [SerializeField] private float cameraMinY;
    [SerializeField] private float scroll;

    private Vector3 targetPosition; // The position the camera is lerping toward
    private Quaternion targetRotation; // The rotation the camera is lerping toward

    void Start()
    {
        if (player != null)
        {
            // Initialize offset and target position
            offset = transform.position - player.position;
            targetPosition = player.position + offset;
            transform.position = targetPosition; // Set initial position to avoid jump
            // Initialize target rotation to look at the player
            targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = targetRotation; // Set initial rotation
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Handle rotation when middle mouse button is held
        //if (Input.GetMouseButton(2)) // Middle mouse button
        if(inputHandler.isLook)
        {
            float mouseX = inputHandler.MouseVector.x * rotationSpeed;  //Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = inputHandler.MouseVector.y * rotationSpeed;   //Input.GetAxis("Mouse Y") * rotationSpeed;



            // Rotate the offset around the player
            offset = Quaternion.AngleAxis(mouseX, Vector3.up) * offset; // Horizontal rotation
            offset = Quaternion.AngleAxis(-mouseY, transform.right) * offset; // Vertical rotation

            // Handle zoom with scroll wheel
            this.scroll = inputHandler.ZoomDelta.y; //Input.GetAxis("Mouse ScrollWheel");

            camDistance = offset.magnitude;
            camDistance = Mathf.Clamp(camDistance - this.scroll * zoomSpeed, camMinDist, camMaxDist);
            offset = offset.normalized * camDistance;

            // Calculate the target position
            targetPosition = player.position + offset;
            if(targetPosition.y < cameraMinY)
            {
                targetPosition.y = cameraMinY;
            }

            // Smoothly move the camera toward the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, posDampingActive * Time.deltaTime);

            // Calculate the target rotation (look at the player)
            targetRotation = Quaternion.LookRotation(player.position - transform.position);

            // Smoothly rotate the camera toward the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotDampingActive * Time.deltaTime);
        }
        else
        {

            // Handle zoom with scroll wheel
            this.scroll = inputHandler.ZoomDelta.y; //Input.GetAxis("Mouse ScrollWheel");

            camDistance = offset.magnitude;
            camDistance = Mathf.Clamp(camDistance - this.scroll * zoomSpeed, camMinDist, camMaxDist);
            offset = offset.normalized * camDistance;

            // Calculate the target position
            targetPosition = player.position + offset;

            if (targetPosition.y < cameraMinY)
            {
                targetPosition.y = cameraMinY;
            }

            // Smoothly move the camera toward the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, posDampingPassive * Time.deltaTime);

            // Calculate the target rotation (look at the player)
            targetRotation = Quaternion.LookRotation(player.position - transform.position);

            // Smoothly rotate the camera toward the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotDampingPassive * Time.deltaTime);
        }
    }
}