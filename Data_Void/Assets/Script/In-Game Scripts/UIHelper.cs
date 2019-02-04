using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{

    public void EndPRTurn()
    {
        CSGameManager.gameManager.EndTurnButton();
    }
}
