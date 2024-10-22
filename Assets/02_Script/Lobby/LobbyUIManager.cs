using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyUIManager : MonoBehaviour
{
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI rating;
    public TextMeshProUGUI energyCount;
    public static int energy = 5;

    private void OnEnable()
    {
        nickName.text = Backend.PlayerNickName;
        energyCount.text = energy.ToString();
    }

    private void Start()
    {
        
    }
}
