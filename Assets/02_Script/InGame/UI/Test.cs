using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Define;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI txt;

    void Update()
    {
        if(GameManager.Instance.CurrentPlayer == Player.One)
        {
            txt.text = "playerOne";
        }
        else
        {
            txt.text = "playerTwo";
        }
    }
}
