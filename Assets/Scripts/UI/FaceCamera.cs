using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {
    public Camera targetCamera;

    void Start() {
        if (targetCamera == null) {
            targetCamera = Camera.main;
        }
    }

    void Update() {
        if (targetCamera != null) {
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
        }
    }
}