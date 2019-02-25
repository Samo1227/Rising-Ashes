using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTarget : MonoBehaviour
{
    Renderer rn_Rend;
    Color c;
    public float fl_FadeSpeed = 3f;
    public IntroPlayerBot ipb_PR;

    void Start()
    {
        rn_Rend = gameObject.GetComponent<Renderer>();
        c = rn_Rend.material.color;
        c.a = 0;
    }
    

    void Update()
    {
        float f = (Mathf.Sin(fl_FadeSpeed*Time.time)+1)/2;
        c.a = f;
        rn_Rend.material.color = c;
    }

    private void OnMouseUp()
    {
        ipb_PR.StartCoroutine("GoTherePlease");

    }
}
