using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StoneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] StoneTypes;   // 돌 종류
    public GameObject[] _stoneTypes    // 돌 종류는 다른곳에서는 접근 하지 못하게 하려고 만든 프로퍼티
    {   
        get { return StoneTypes; } 
        private set { StoneTypes = value; }
    }

    public GameObject landingStone;    // 돌 ( 착수되기 전 보여줄 오브젝트 )
    public int stoneTypesCount { get; private set; }  // 돌 종류 갯수 ( 돌 랜덤값 뽑기 위해 )
    public List<int> stoneIndex { get; private set; } = new List<int>(); // 플레이어가 사용하는 돌의 인덱스를 저장
    public Collider stoneColl { get; set; }
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Start()
    {
        FirstStoneAssign();
    }

    // 게임 시작시 돌 랜덤 배정
    public void FirstStoneAssign()
    {
        // 돌 종류의 갯수 설정
        stoneTypesCount = _stoneTypes.Length;

        // 시작하면 랜덤으로 플레이어가 사용할 돌 정하기
        for (int i = 0; i < (int)Player.Count; i++)
        {
            // 0번째에 저장되는 돌은 1플레이어가 사용할거고
            // 1번째에 저장되는 돌은 2플레이어가 사용할거임
            int j = Random.Range(0, stoneTypesCount);
            stoneIndex.Add(j);

        }
        Debug.Log("플레이어1 : " + stoneIndex[0]);
        Debug.Log("플레이어2 : " + stoneIndex[1]);

        Debug.Log("돌 저장 완료");
    }

    // 돌 초기화
    public void InitializeStone()
    {
        if(gameManager.CurrentPlayer == Player.One)
        {
            // 랜딩 스톤은 할당만 되어있기에 새로 만들어주자 ( 각 플레이어가 사용하는 돌로 )
            landingStone = Instantiate(_stoneTypes[stoneIndex[0]], Vector3.zero, Quaternion.identity);
        }
        else
        {
            // 랜딩 스톤은 할당만 되어있기에 새로 만들어주자 ( 각 플레이어가 사용하는 돌로 )
            landingStone = Instantiate(_stoneTypes[stoneIndex[1]], Vector3.zero, Quaternion.identity);
        }

        // 콜라이더도 붙여주기
        stoneColl = landingStone.GetComponent<Collider>();
    }
}
