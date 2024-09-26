using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BaseStone : MonoBehaviour
{
    public GameObject[] stoneTypes;                       // 돌 종류

    protected GameObject landingStone;                    // 돌 ( 착수되기 전 보여줄 오브젝트 )
    protected int stoneTypesCount;                        // 돌 종류 갯수 ( 돌 랜덤값 뽑기 위해 )
    protected List<int> stoneIndex = new List<int>();     // 플레이어가 사용하는 돌의 인덱스를 저장
    protected int whoTurn = 0;                            // 턴 ( 0 일때는 플레이어1 1일때는 플레이어2 )
    protected Collider stoneColl;                         // 콜라이더

    // 게임 시작시 돌 랜덤 배정
    protected void FirstStoneAssign()
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
    protected void InitializeStone()
    {
        // 랜딩 스톤은 할당만 되어있기에 새로 만들어주자 ( 각 플레이어가 사용하는 돌로 )
        landingStone = Instantiate(stoneTypes[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        // 콜라이더도 붙여주기
        stoneColl = landingStone.GetComponent<Collider>();
    }

}

public class Landing : BaseStone
{
    [SerializeField]
    private LayerMask boardLayer;               // 레이어마스크
    private RaycastHit hit;                     // 레이캐스트
    private Vector3 landingPoint;               // 착지점
    StoneManager stoneManager;                  // 스톤 매니저

    private void Awake()
    {
        stoneManager = GetComponent<StoneManager>();
    }

    private void Start()
    {
        FirstStoneAssign();         // 베이스에 있는 첫번째 랜덤 돌 설정
        InitializeStone();          // 착수위치 보여주는 돌 초기화
    }

    private void FixedUpdate()
    {
        // 마우스 위치로 랜딩포인트 변경
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, boardLayer))
        {
            landingPoint = hit.point;
        }
    }

    void Update()
    {
        if (landingStone != null)
        {
            // 마우스 왼쪽 클릭하면 착수될 위치에 돌이 어떻게 떨어지는지 미리보기
            // 추후에 터치로 변경
            if (Input.GetMouseButton(0))
            {
                landingStone.transform.position = landingPoint;
                stoneColl.enabled = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                stoneColl.enabled = true;
                Destroy(landingStone);
                LandingPointStone();
            }
        }
    }

    void LandingPointStone()
    {

        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
        Vector3 originlandingPoint;
        originlandingPoint = landingStone.transform.position;
        originlandingPoint.y += 1f;
        landingStone.transform.position = originlandingPoint;

        if (stoneManager.isPlayerOneTurn)
        {
            Instantiate(stoneTypes[stoneIndex[0]], landingStone.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(stoneTypes[stoneIndex[1]], landingStone.transform.position, Quaternion.identity);
        }

        stoneManager.SwitchTurn();
        whoTurn = stoneManager.isPlayerOneTurn ? 0 : 1;
        InitializeStone();
    }

    void CountingStone()
    {
        if (stoneManager.playerOneHaveStone <= 0 || stoneManager.playerTwoHaveStone <= 0)
        {
            stoneManager.ChangeState(GameState.GameOver);

        }

        // 착수하면 돌 카운트 줄이기
        if (stoneManager.isPlayerOneTurn)
        {
            stoneManager.playerOneHaveStone -= 1;
        }
        else
        {
            stoneManager.playerTwoHaveStone -= 1;
        }
    }
}

public class StoneManager5 : Singleton<StoneManager5>
{
    [Header("플레이어의 돌 갯수")]
    public int playerOneHaveStone = 10;
    public int playerTwoHaveStone = 10;

    [Space(10f)]
    [Header("자성효과들")]
    public float magnetRange = 2;       // 자성 거리
    public float magnetMaxForce = 1;    // 자성 최대 파워
    public float magnetTime = 3;        // 자성 효력 시간
    public int clingStone = 0;          // 붙은 돌 갯수
    public LayerMask canClingLayer;     // 붙을 수 있을 레이어

    [Space(10f)]
    [Header("턴 체크")]
    public bool isPlayerOneTurn = true;
    public float turnTimeLimit = 10f;

    public void SwitchTurn()
    {
        isPlayerOneTurn = !isPlayerOneTurn;
    }

    public bool ChangeState(GameState state)
    {
        bool canPlaying;
        if (state == GameState.Playing)
        {
            canPlaying = true;
        }
        else
        {
            canPlaying = false;
        }

        return canPlaying;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Board"))
        {
            StartCoroutine(PullStone());
        }

        if (collision.collider.CompareTag("LandingStone"))
        {
            magnetTime = 3;
        }
    }

    IEnumerator PullStone()
    {
        Collider[] otherStones = Physics.OverlapSphere(transform.position, magnetRange, canClingLayer);

        foreach (Collider stone in otherStones)
        {
            float distance = Vector3.Distance(transform.position, stone.transform.position);
            if (distance <= magnetRange)
            {
                float magnetForce = magnetMaxForce - (distance / magnetRange);
                Vector3 disLandingStone = transform.position - stone.transform.position;
                Vector3 dirLandingStone = disLandingStone.normalized;
                stone.transform.position += dirLandingStone * magnetForce * Time.deltaTime;
            }
        }

        yield return new WaitForSeconds(magnetTime);

        // 자석효과 없애기
        // 턴 교체 ( 스톤클래스에서 턴교체 삭제해주기 )
    }
}*/