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
        // ���� UI ��� ��Ȱ��ȭ
        foreach (GameObject ui in resultUI)
        {
            ui.SetActive(false);
        }

        // ���� ��Ͽ� ���� �ִ��� Ȯ��
        if (result.m_winners != null && result.m_winners.Contains(mySessionId))
        {
            // �¸� UI ǥ��
            resultUI[0].SetActive(true);
            userData.win++;
        }
        // ���� ��Ͽ� ���� �ִ��� Ȯ��
        else if (result.m_losers != null && result.m_losers.Contains(mySessionId))
        {
            // �й� UI ǥ��
            resultUI[1].SetActive(true);
            userData.lose++;
        }
        BackendGameData.Instance.GameDataUpdate();
    }

    private void UpdateTurnUI(Player currentPlayer)
    {
        // ���� �÷��̾ �ڽ����� Ȯ��
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
