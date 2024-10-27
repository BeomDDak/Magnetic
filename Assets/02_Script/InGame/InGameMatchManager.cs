using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public partial class BackendMatchManager : Singleton<BackendMatchManager>
{
    private void GameSetup()
    {
        LobbyUIManager.energy -= 1;
        isHost = false;
        isSetHost = false;
        OnGameReady();
    }

    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // ȣ��Ʈ�� �������� ���� �����̸� ȣ��Ʈ ����
            isSetHost = SetHostSession();
        }
        Debug.Log("ȣ��Ʈ ���� �Ϸ�");
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

        Debug.Log($"IsMyPlayer üũ - �� ����: {mySessionId}");
        Debug.Log($"��ϵ� �÷��̾� ��: {players.Count}");
        foreach (var pair in players)
        {
            Debug.Log($"��ϵ� �÷��̾� ���� - SessionId: {pair.Key}, Player: {pair.Value}");
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
        Debug.LogError($"�÷��̾ ã�� �� �����ϴ�. SessionId: {mySessionId}");
        return false;
    }

    // ������ ������ ��Ŷ ����
    // ���������� �� ��Ŷ�� �޾� ��� Ŭ���̾�Ʈ(��Ŷ ���� Ŭ���̾�Ʈ ����)�� ��ε�ĳ���� ���ش�.
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }
}