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

    public GameObject[] nickChangeUI;

    protected override void Init()
    {
        base.Init();
        isDestoryOnLoad = true;
    }

    private void Start()
    {
        if(BackendGameData.userData.userName != string.Empty)
        {
            SetInfo();
        }
        else
        {
            SetFirstNick();
        }
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

    private void SetFirstNick()
    {
        for(int i = 0; i < nickChangeUI.Length; i++)
        {
            nickChangeUI[i].SetActive(true);
        }
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
            StartCoroutine(OpenPopup("�г����� �ʹ� ��ϴ�"));
            return;
        }

        var bro = Backend.BMember.UpdateNickname(changeNickName.text);

        if (bro.IsSuccess())
        {
            StartCoroutine(OpenPopup("�г��� ���濡 �����߽��ϴ�"));
            BackendGameData.userData.userName = changeNickName.text;
            BackendGameData.Instance.GameDataUpdate();
            for(int i = 0;i < nickChangeUI.Length; i++)
            {
                nickChangeUI[i].SetActive(false);
            }
            lobbyNickName.text = $"{BackendGameData.userData.userName}";
            profileNickName.text = $"�� �� �� :{BackendGameData.userData.userName}";
        }
        else
        {
            StartCoroutine(OpenPopup("�г��� ���濡 �����߽��ϴ�"));
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
