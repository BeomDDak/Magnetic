using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public partial class BackendMatchManager : Singleton<BackendMatchManager>
{
    private MatchGameResult matchGameResult;

    private void GameSetup()
    {
        BackendGameData.userData.energy -= 1;
        BackendGameData.Instance.GameDataUpdate();
        isHost = false;
        isSetHost = false;
        OnGameReady();
    }

    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // 호스트가 설정되지 않은 상태이면 호스트 설정
            isSetHost = SetHostSession();
        }
        Debug.Log("호스트 설정 완료");
    }

    public bool IsHost()
    {
        if(isHost)
            return true;

        return false;
    }

    public bool IsMyPlayer(Define.Player player)
    {
        SessionId mySessionId = Backend.Match.GetMySessionId();

        Debug.Log($"IsMyPlayer 체크 - 내 세션: {mySessionId}");
        Debug.Log($"등록된 플레이어 수: {players.Count}");
        foreach (var pair in players)
        {
            Debug.Log($"등록된 플레이어 정보 - SessionId: {pair.Key}, Player: {pair.Value}");
        }

        if(sessionIdList.Count > 0)
        {
            if (mySessionId == sessionIdList[0])
            {
                return player == Define.Player.One;
            }
            else
            {
                return player == Define.Player.Two;
            }
        }
        Debug.LogError($"플레이어를 찾을 수 없습니다. SessionId: {mySessionId}");
        return false;
    }

    // 서버로 데이터 패킷 전송
    // 서버에서는 이 패킷을 받아 모든 클라이언트(패킷 보낸 클라이언트 포함)로 브로드캐스팅 해준다.
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }

    public void MatchGameOver(Stack<SessionId> record)
    {
        matchGameResult = OneOnOneRecord(record);

        InGameUI.Instance.SetResultUI(matchGameResult);
        Backend.Match.MatchEnd(matchGameResult);
        StartCoroutine(ReturnToLobby());
    }

    private MatchGameResult OneOnOneRecord(Stack<SessionId> record)
    {
        MatchGameResult nowGameResult = new MatchGameResult();

        nowGameResult.m_winners = new List<SessionId>();
        nowGameResult.m_winners.Add(record.Pop());

        nowGameResult.m_losers = new List<SessionId>();
        nowGameResult.m_losers.Add(record.Pop());

        nowGameResult.m_draws = null;

        return nowGameResult;
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(2f);
        SceneLoader.Instance.LoadScene(SceneType.Lobby);
    }
}
