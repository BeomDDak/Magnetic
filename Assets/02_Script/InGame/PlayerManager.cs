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
        player1Stone.text = $"플레이어1\n남은 돌 : {stoneCount[Player.One]}";
        player2Stone.text = $"플레이어2\n남은 돌 : {stoneCount[Player.Two]}";
    }

    // 게임 시작시 초기화 해줄 함수
    public PlayerManager()
    {
        InitializePlayers();
    }

    // 초기 돌 갯수 설정
    public void InitializePlayers()
    {
        stoneCount[Player.One] = 10;
        stoneCount[Player.Two] = 10;
    }

    // 돌 착수 시 카운트 --
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

    // 붙은 돌들 계산해서 합친 후 추가
    public void IncrementStoneCount(int clingStone, Player player)
    {
        stoneCount[player] += clingStone;
        PlayerSyncMessage syncMsg = new PlayerSyncMessage(stoneCount);
        BackendMatchManager.Instance.SendDataToInGame(syncMsg);
    }
}
