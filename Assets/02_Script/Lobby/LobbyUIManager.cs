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
        profileNickName.text = $"닉 네 임 :{BackendGameData.userData.userName}";
        lobbyWinCount.text = $"승: {BackendGameData.userData.win}";
        winCount.text = $"승 : {BackendGameData.userData.win}";
        lobbyLoseCount.text = $"패 : {BackendGameData.userData.lose}";
        loseCount.text = $"패: {BackendGameData.userData.lose}";
        energyCount.text = BackendGameData.userData.energy.ToString();
    }

    private void SetFirstNick()
    {
        for(int i = 0; i < nickChangeUI.Length; i++)
        {
            nickChangeUI[i].SetActive(true);
        }
        lobbyWinCount.text = $"승: {BackendGameData.userData.win}";
        winCount.text = $"승 : {BackendGameData.userData.win}";
        lobbyLoseCount.text = $"패 : {BackendGameData.userData.lose}";
        loseCount.text = $"패: {BackendGameData.userData.lose}";
        energyCount.text = BackendGameData.userData.energy.ToString();
    }

    public void ChangeNick()
    {
        if(changeNickName.text.Length > 8)
        {
            StartCoroutine(OpenPopup("닉네임이 너무 깁니다"));
            return;
        }

        var bro = Backend.BMember.UpdateNickname(changeNickName.text);

        if (bro.IsSuccess())
        {
            StartCoroutine(OpenPopup("닉네임 변경에 성공했습니다"));
            BackendGameData.userData.userName = changeNickName.text;
            BackendGameData.Instance.GameDataUpdate();
            for(int i = 0;i < nickChangeUI.Length; i++)
            {
                nickChangeUI[i].SetActive(false);
            }
            lobbyNickName.text = $"{BackendGameData.userData.userName}";
            profileNickName.text = $"닉 네 임 :{BackendGameData.userData.userName}";
        }
        else
        {
            StartCoroutine(OpenPopup("닉네임 변경에 실패했습니다"));
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
