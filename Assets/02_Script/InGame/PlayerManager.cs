using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerManager
{
    private Dictionary<Player, int> stoneCount = new Dictionary<Player, int>();

    // ���� ���۽� �ʱ�ȭ ���� �Լ�
    public PlayerManager()
    {
        InitializePlayers();
    }

    // �ʱ� �� ���� ����
    private void InitializePlayers()
    {
        stoneCount[Player.One] = 10;
        stoneCount[Player.Two] = 10;
    }

    // �� ���� �� ī��Ʈ --
    public void DecrementStoneCount(Player player)
    {
        stoneCount[player]--;
        if (stoneCount[player] <= 0)
        {
            GameManager.Instance.EndGame();
        }
    }

    // ���� ���� ����ؼ� ��ģ �� �߰�
    public void IncrementStoneCount(int clingStone, Player player)
    {
        stoneCount[player] += clingStone;
    }
}
