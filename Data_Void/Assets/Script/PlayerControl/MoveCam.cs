using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour {

    public float fl_cam_move;
    public float fl_cam_Move_Speed = 20f;
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

        if (Input.GetKey(KeyCode.Q))
        {
            //transform.Rotate(new Vector3(0, 90, 0));

            //  transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 1, transform.rotation.z));
            transform.Rotate(Vector3.up * fl_cam_Move_Speed * Time.deltaTime, Space.World);

        }
        if (Input.GetKey(KeyCode.E))
        {
            //transform.Rotate(new Vector3(0, -90, 0));

           // transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y -1, transform.rotation.z));
            transform.Rotate(Vector3.up * -fl_cam_Move_Speed * Time.deltaTime, Space.World);
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
