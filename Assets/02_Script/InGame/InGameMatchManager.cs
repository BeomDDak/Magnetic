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

        // ���� ������ �÷��̾� �� Ȯ��
        if (players.TryGetValue(mySessionId, out Define.Player myPlayer))
        {
            return myPlayer == player;
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
