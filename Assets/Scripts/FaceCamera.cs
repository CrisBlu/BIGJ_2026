using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Quaternion initialRotation;

    void Start()
    {
        mainCamera = Camera.main;
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward, -mainCamera.transform.up) * initialRotation;
    }
}
