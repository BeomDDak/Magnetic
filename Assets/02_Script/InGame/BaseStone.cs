using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStone : MonoBehaviour
{
    protected GameObject stone;                           // 돌 ( 착수되기 전 보여줄 오브젝트 )
    protected int stoneKind;                              // 돌 종류 ( 돌 랜덤값 뽑기 위해 )
    protected List<int> stoneIndex = new List<int>();     // 선택될 돌
    protected GameObject[] playerStone;                   // 플레이어가 사용할 돌 ( 플레이어가 사용할 돌을 저장 )
    protected int whoTurn = 0;                            // 턴 ( 0 일때는 플레이어1 1일때는 플레이어2 )
    protected Collider stoneColl;                         // 콜라이더

    // 게임 시작시 돌 랜덤 배정
    protected void FirstStoneAssign()
    {
        // 최초 돌 랜덤으로 리스트에 2가지 넣어놓음
        for (int i = 0; i < 2; i++)
        {
            int j = Random.Range(0, stoneKind);
            stoneIndex.Add(j);
        }
    }

    // 돌 초기화
    protected void InitializeStone()
    {
        stone = Instantiate(playerStone[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        stoneColl = stone.GetComponent<Collider>();
    }

}
