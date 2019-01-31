using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour {

    public float fl_cam_move;
    public float fl_cam_zoom = 5;
    public Camera cam;

	// Use this for initialization
	void Start ()
    {
        cam = GetComponentInChildren<Camera>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 1) * fl_cam_move * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-1, 0, -1) * fl_cam_move * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(-1, 0, 1) * fl_cam_move * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(1, 0, -1) * fl_cam_move * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, 90, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, -90, 0));
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // forward
        {
            fl_cam_zoom++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            fl_cam_zoom--;
        }

        fl_cam_zoom = Mathf.Clamp(fl_cam_zoom, 1f,10f);

        cam.orthographicSize = fl_cam_zoom;
    }
}
