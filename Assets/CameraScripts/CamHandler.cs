using UnityEngine;
using UnityEngine.InputSystem;

public class CamHandler : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private FreeCam freeCamScript;
    [SerializeField] private CameraFollow cameraFollowScript;
    [SerializeField] private bool isFreeCam = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputHandler.ToggleCameraEvent += ToggleCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleCamera()
    {
        // Toggle between FreeCamera and CameraFollow
        isFreeCam = !isFreeCam;
        SetCameraMode(isFreeCam);
        Debug.Log("camera is" + isFreeCam);
    }

    private void SetCameraMode(bool useFreeCamera)
    {
        if (freeCamScript != null && cameraFollowScript != null)
        {
            freeCamScript.enabled = useFreeCamera;
            cameraFollowScript.enabled = !useFreeCamera;
            Debug.Log(useFreeCamera ? "Free Camera Active" : "Camera Follow Active");
        }
        else
        {
            Debug.LogError("Camera scripts not assigned in CameraManager!");
        }
    }

    private void OnDestroy()
    {
        inputHandler.ToggleCameraEvent -= ToggleCamera;
    }
}
