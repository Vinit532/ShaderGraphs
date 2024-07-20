using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public List<Camera> cameras = new List<Camera>();
    public float switchInterval = 5f;

    private int currentCameraIndex = 0;

    void Start()
    {
        if (cameras.Count == 0)
        {
            Debug.LogError("No cameras assigned to the CameraSwitcher script.");
            return;
        }

        // Sort cameras based on their depth value
        cameras.Sort((cam1, cam2) => cam1.depth.CompareTo(cam2.depth));

        // Enable the first camera and disable others
        EnableCurrentCamera();
        InvokeRepeating(nameof(SwitchCamera), switchInterval, switchInterval);
    }

    void SwitchCamera()
    {
        cameras[currentCameraIndex].gameObject.SetActive(false);

        currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;

        EnableCurrentCamera();
    }

    void EnableCurrentCamera()
    {
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}
