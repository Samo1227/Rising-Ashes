using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingRay : MonoBehaviour {

    public GameObject target;

    public GameObject rayOrigin;
    public float MaxCheckDistance = 10;

    bool bl_hit_wall = false;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKeyDown("space"))
        {
            RaycastHit hit;

            Vector3 dir = target.transform.position - rayOrigin.transform.position;
            dir = dir.normalized;

            Ray ray_cast = new Ray (rayOrigin.transform.position, dir * MaxCheckDistance);

            Debug.DrawRay(rayOrigin.transform.position, dir * MaxCheckDistance, Color.red, 0.1f);

            if(Physics.Raycast(ray_cast, out hit))
            {
                if(hit.collider.tag == "wall")
                {
                    Destroy(hit.collider.gameObject);
                }
                if (hit.collider.tag == "Foe")
                {
                    Destroy(hit.collider.gameObject);
                }
            }


        }
	}
    
}
