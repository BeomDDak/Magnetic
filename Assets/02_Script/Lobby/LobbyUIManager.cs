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
    public TextMeshProUGUI lobbyNickName;
    public TextMeshProUGUI profileNickName;
    public TMP_InputField changeNickName;
    public TextMeshProUGUI energyCount;
    public static int energy = 5;

    private void Start()
    {
        Nick();
    }

    void Nick()
    {
        if(changeNickName == null)
        {
            changeNickName.text = " ";
        }
        lobbyNickName.text = $"{changeNickName}";
        profileNickName.text = $"닉 네 임 :{lobbyNickName}";
    }

    public void ChangeNick()
    {
        var bro = Backend.BMember.UpdateNickname(changeNickName.text);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
            Nick();
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
        }
    }

}
