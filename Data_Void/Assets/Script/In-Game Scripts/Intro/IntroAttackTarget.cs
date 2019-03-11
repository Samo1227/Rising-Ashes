using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAttackTarget : MonoBehaviour
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
        if (rn_Rend == null)
        {
            return;
        }
        float f = (Mathf.Sin(fl_FadeSpeed * Time.time) + 1) / 2;
        //   print(f);
        c.a = f;
        rn_Rend.material.color = c;
    }

    private void OnMouseUp()
    {
        Tile _tempTile = CSGameManager.gameManager.map[4, 9];
        _tempTile.int_health = 0;
        ipb_PR.Clear_Selection();
        ipb_PR.bl_isFinished = true;
        // StartCoroutine(GoToShipMenu());
        ipb_PR.FinalFungusScene();
        Destroy(rn_Rend);
        Destroy(gameObject.GetComponent<AudioSource>());
        //fungus scene
    }
    IEnumerator GoToShipMenu()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
