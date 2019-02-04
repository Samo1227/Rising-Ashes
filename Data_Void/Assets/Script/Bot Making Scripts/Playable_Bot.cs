using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable_Bot : MonoBehaviour {

    public int[] int_arr_parts;
    public Transform[] tr_arr_body;
    public string[] st_arr_resources;


    // Use this for initialization
    void Start ()
    {
	    for(int i = 0; i < 4; i++)
        {
            Instantiate(Resources.Load<GameObject>(st_arr_resources[i] + int_arr_parts[i]),new Vector3(tr_arr_body[i].position.x, tr_arr_body[i].position.y, tr_arr_body[i].position.z), Quaternion.identity, tr_arr_body[i]);
        }	
	}
	
    public void ActivateBot()
    { 

    }
}
