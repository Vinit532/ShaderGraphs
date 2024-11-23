using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cubes Setup")]
    public List<GameObject> cubeObjects; // List of all cube objects

    [Header("Camera Movement Settings")]
    public float moveSpeed = 10f; // Speed at which the camera moves
    public float rotateSpeed = 5f; // Speed at which the camera rotates

    private int currentCubeIndex = 0; // Tracks the current cube in the list
    private bool isMoving = false; // Flag to prevent multiple triggers

    void Update()
    {
        // Check for right-click or touch
        if (Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (!isMoving && cubeObjects.Count > 0)
            {
                StartCoroutine(MoveToNextCube());
            }
        }
    }

    private System.Collections.IEnumerator MoveToNextCube()
    {
        isMoving = true;

        // Get the current cube
        GameObject targetCube = cubeObjects[currentCubeIndex];
        Transform cubeTransform = targetCube.transform;

        // Determine camera position and rotation based on cube
        Vector3 cameraPosition = CalculateCameraPosition(cubeTransform);
        Quaternion cameraRotation = CalculateCameraRotation(cubeTransform);

        // Smoothly move and rotate the camera
        while (Vector3.Distance(transform.position, cameraPosition) > 0.1f || Quaternion.Angle(transform.rotation, cameraRotation) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, cameraPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraRotation, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to final position and rotation
        transform.position = cameraPosition;
        transform.rotation = cameraRotation;

        // Move to the next cube in the list
        currentCubeIndex = (currentCubeIndex + 1) % cubeObjects.Count;

        isMoving = false;
    }


    private Vector3 CalculateCameraPosition(Transform cubeTransform)
    {
        // Get the cube's size (using scale for variable-sized cubes)
        float cubeScaleZ = cubeTransform.localScale.z;

        // Determine distance based on the cube's scale
        float distance = 6f; // Default distance
        if (cubeScaleZ > 10) distance = 12f;
        if (cubeScaleZ > 14) distance = 18f;
        if (cubeScaleZ > 20) distance = 30f;

        // Adjust camera position based on cube's rotation and size
        Vector3 cubePosition = cubeTransform.position;
        Vector3 cameraPosition = cubePosition;

        // Normalize the cube's Y-axis rotation for consistent behavior
        float cubeRotationY = Mathf.Round(cubeTransform.eulerAngles.y) % 360;

        // Handle Unity's representation of rotations
        if (cubeRotationY == 270f) cubeRotationY = -90f; // Map 270 to -90

        switch (Mathf.RoundToInt(cubeRotationY))
        {
            case 0:
                cameraPosition.x = cubePosition.x + distance; // X > Cube X
                break;
            case 90:
                cameraPosition.z = cubePosition.z + distance; // Z > Cube Z
                break;
            case -90:
                cameraPosition.z = cubePosition.z - distance; // Z < Cube Z
                break;
            case 180:
                cameraPosition.x = cubePosition.x - distance; // X < Cube X
                break;
            default:
                Debug.LogWarning($"Unexpected cube rotation on Y-axis: {cubeRotationY}");
                break;
        }

        cameraPosition.y = cubePosition.y; // Ensure camera height matches cube
        return cameraPosition;
    }



    private Quaternion CalculateCameraRotation(Transform cubeTransform)
    {
        // Determine camera rotation based on cube's Y-axis rotation
        float cubeRotationY = Mathf.RoundToInt(cubeTransform.eulerAngles.y);
        float cameraRotationY = 0f;

        switch (cubeRotationY)
        {
            case 0:
                cameraRotationY = -90f;
                break;
            case 90:
                cameraRotationY = 180f;
                break;
            case -90:
                cameraRotationY = 0f;
                break;
            case 180:
                cameraRotationY = 90f;
                break;
        }

        // Return the calculated rotation
        return Quaternion.Euler(0f, cameraRotationY, 0f);
    }
}
