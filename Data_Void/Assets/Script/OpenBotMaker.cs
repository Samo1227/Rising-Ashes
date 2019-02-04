using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBotMaker : MonoBehaviour
{
    public GameObject go_bot_maker;

    public void OpenBotmMaker()
    {
        go_bot_maker.SetActive(true);
        gameObject.SetActive(false);
    }
}
