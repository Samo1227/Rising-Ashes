using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoarding : MonoBehaviour {

    public Camera cam_Camera;
    private void Start()
    {
        cam_Camera = Camera.main;
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam_Camera.transform.rotation * Vector3.forward,
            cam_Camera.transform.rotation * Vector3.up);
    }
}
