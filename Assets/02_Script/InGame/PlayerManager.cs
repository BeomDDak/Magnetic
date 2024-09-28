using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerManager
{
    private Dictionary<Player, int> stoneCount = new Dictionary<Player, int>();

    // 게임 시작시 초기화 해줄 함수
    public PlayerManager()
    {
        InitializePlayers();
    }

    // 초기 돌 갯수 설정
    private void InitializePlayers()
    {
        stoneCount[Player.One] = 10;
        stoneCount[Player.Two] = 10;
    }

    // 돌 착수 시 카운트 --
    public void DecrementStoneCount(Player player)
    {
        stoneCount[player]--;
        if (stoneCount[player] <= 0)
        {
            GameManager.Instance.EndGame();
        }
    }

    // 붙은 돌들 계산해서 합친 후 추가
    public void IncrementStoneCount(int clingStone, Player player)
    {
        stoneCount[player] += clingStone;
    }
}
