using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Vector3 cameraOffset;

    void Start()
    {
        
    }

    void Update()
    {
        cameraTransform.position = playerTransform.position + cameraOffset;
    }
}
