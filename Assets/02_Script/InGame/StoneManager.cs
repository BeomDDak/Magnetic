using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _stoneTypes;                           // 돌 종류
    protected GameObject[] stoneTypes                           // 돌 종류는 다른곳에서는 접근 하지 못하게 하려고 만든 프로퍼티
    {   
        get { return _stoneTypes; } 
        private set { _stoneTypes = value; }
    }                                                           

    protected GameObject landingStone { get; private set; }     // 돌 ( 착수되기 전 보여줄 오브젝트 )
    protected int stoneTypesCount { get; private set; }         // 돌 종류 갯수 ( 돌 랜덤값 뽑기 위해 )
    protected List<int> stoneIndex = new List<int>();           // 플레이어가 사용하는 돌의 인덱스를 저장
    protected int whoTurn = 0;                                  // 턴 ( 0 일때는 플레이어1 1일때는 플레이어2 )
    protected Collider stoneColl { get; set; }                  // 콜라이더
    protected GameManager GameManager { get; private set; }     // 게임매니저

    public StoneManager()
    {
        FirstStoneAssign();
    }

    // 게임 시작시 돌 랜덤 배정
    private void FirstStoneAssign()
    {
        // 돌 종류의 갯수 설정
        stoneTypesCount = stoneTypes.Length;

        // 시작하면 랜덤으로 플레이어가 사용할 돌 정하기
        for (int i = 0; i < 2; i++)
        {
            // 0번째에 저장되는 돌은 1플레이어가 사용할거고
            // 1번째에 저장되는 돌은 2플레이어가 사용할거임
            int j = Random.Range(0, stoneTypesCount);
            stoneIndex.Add(j);
        }
    }

    // 돌 초기화
    public void InitializeStone()
    {
        // 랜딩 스톤은 할당만 되어있기에 새로 만들어주자 ( 각 플레이어가 사용하는 돌로 )
        landingStone = Instantiate(stoneTypes[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        // 콜라이더도 붙여주기
        stoneColl = landingStone.GetComponent<Collider>();
    }

}
