using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    public GameObject popupUI;
    public TextMeshProUGUI popupText;

    public TextMeshProUGUI lobbyNickName;
    public TextMeshProUGUI profileNickName;
    public TMP_InputField changeNickName;
    public TextMeshProUGUI lobbyWinCount;
    public TextMeshProUGUI winCount;
    public TextMeshProUGUI lobbyLoseCount;
    public TextMeshProUGUI loseCount;
    public TextMeshProUGUI energyCount;

    protected override void Init()
    {
        base.Init();
        isDestoryOnLoad = true;
    }

    private void Start()
    {
        SetInfo();
    }

    private void SetInfo()
    {
        lobbyNickName.text = $"{BackendGameData.userData.userName}";
        profileNickName.text = $"�� �� �� :{BackendGameData.userData.userName}";
        lobbyWinCount.text = $"��: {BackendGameData.userData.win}";
        winCount.text = $"�� : {BackendGameData.userData.win}";
        lobbyLoseCount.text = $"�� : {BackendGameData.userData.lose}";
        loseCount.text = $"��: {BackendGameData.userData.lose}";
        energyCount.text = BackendGameData.userData.energy.ToString();
    }

    public void ChangeNick()
    {
        if(changeNickName.text.Length > 8)
        {
            OpenPopup("�г����� �ʹ� ��ϴ�");
            return;
        }

        var bro = Backend.BMember.UpdateNickname(changeNickName.text);

        if (bro.IsSuccess())
        {
            OpenPopup("�г��� ���濡 �����߽��ϴ�");
            BackendGameData.Instance.GameDataUpdate();
        }
        else
        {
            OpenPopup("�г��� ���濡 �����߽��ϴ�");
        }
    }

    public IEnumerator OpenPopup(string message)
    {
        popupUI.SetActive(true);
        popupText.text = message;
        yield return new WaitForSeconds(1f);
        popupUI.SetActive(false);
    }
}
