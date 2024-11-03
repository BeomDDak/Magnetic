using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using UnityEngine;
using static Define;

public class InGameUI : Singleton<InGameUI>
{
    protected override void Init()
    {
        base.Init();
        isDestoryOnLoad = true;
    }

    public GameObject[] resultUI;
    public GameObject turnUI;
    private SessionId mySessionId;
    private UserData userData;

    private void Start()
    {
        mySessionId = Backend.Match.GetMySessionId();
        userData = BackendGameData.userData;
        GameManager.Instance.OnTurnChanged += UpdateTurnUI;
    }

    public void SetResultUI(MatchGameResult result)
    {
        // 이전 UI 모두 비활성화
        foreach (GameObject ui in resultUI)
        {
            ui.SetActive(false);
        }

        // 승자 목록에 내가 있는지 확인
        if (result.m_winners != null && result.m_winners.Contains(mySessionId))
        {
            // 승리 UI 표시
            resultUI[0].SetActive(true);
            userData.win++;
        }
        // 패자 목록에 내가 있는지 확인
        else if (result.m_losers != null && result.m_losers.Contains(mySessionId))
        {
            // 패배 UI 표시
            resultUI[1].SetActive(true);
            userData.lose++;
        }
        BackendGameData.Instance.GameDataUpdate();
    }

    private void UpdateTurnUI(Player currentPlayer)
    {
        // 현재 플레이어가 자신인지 확인
        bool isMyTurn = BackendMatchManager.Instance.IsMyPlayer(currentPlayer);

        if(isMyTurn)
        {
            turnUI.SetActive(true);
        }
        StartCoroutine(HideTurnUI());
    }

    private IEnumerator HideTurnUI()
    {
        yield return new WaitForSeconds(1f);
        turnUI.SetActive(false);
    }
}
