using Protocol;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public Dictionary<Player, int> stoneCount = new Dictionary<Player, int>();
    public TextMeshProUGUI player1Stone;
    public TextMeshProUGUI player2Stone;

    private void Update()
    {
        player1Stone.text = $"�÷��̾�1\n���� �� : {stoneCount[Player.One]}";
        player2Stone.text = $"�÷��̾�2\n���� �� : {stoneCount[Player.Two]}";
    }

    // ���� ���۽� �ʱ�ȭ ���� �Լ�
    public PlayerManager()
    {
        InitializePlayers();
    }

    // �ʱ� �� ���� ����
    public void InitializePlayers()
    {
        stoneCount[Player.One] = 10;
        stoneCount[Player.Two] = 10;
    }

    // �� ���� �� ī��Ʈ --
    public void DecrementStoneCount(Player player)
    {
        stoneCount[player]--;
        PlayerSyncMessage syncMsg = new PlayerSyncMessage(stoneCount);
        BackendMatchManager.Instance.SendDataToInGame(syncMsg);
        if (stoneCount[player] <= 0)
        {
            GameManager.Instance.EndGame();
        }
    }

    // ���� ���� ����ؼ� ��ģ �� �߰�
    public void IncrementStoneCount(int clingStone, Player player)
    {
        stoneCount[player] += clingStone;
        PlayerSyncMessage syncMsg = new PlayerSyncMessage(stoneCount);
        BackendMatchManager.Instance.SendDataToInGame(syncMsg);
    }
}
