using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BaseStone : MonoBehaviour
{
    public GameObject[] stoneTypes;                       // �� ����

    protected GameObject landingStone;                    // �� ( �����Ǳ� �� ������ ������Ʈ )
    protected int stoneTypesCount;                        // �� ���� ���� ( �� ������ �̱� ���� )
    protected List<int> stoneIndex = new List<int>();     // �÷��̾ ����ϴ� ���� �ε����� ����
    protected int whoTurn = 0;                            // �� ( 0 �϶��� �÷��̾�1 1�϶��� �÷��̾�2 )
    protected Collider stoneColl;                         // �ݶ��̴�

    // ���� ���۽� �� ���� ����
    protected void FirstStoneAssign()
    {
        // �� ������ ���� ����
        stoneTypesCount = stoneTypes.Length;

        // �����ϸ� �������� �÷��̾ ����� �� ���ϱ�
        for (int i = 0; i < 2; i++)
        {
            // 0��°�� ����Ǵ� ���� 1�÷��̾ ����ҰŰ�
            // 1��°�� ����Ǵ� ���� 2�÷��̾ ����Ұ���
            int j = Random.Range(0, stoneTypesCount);
            stoneIndex.Add(j);
        }
    }

    // �� �ʱ�ȭ
    protected void InitializeStone()
    {
        // ���� ������ �Ҵ縸 �Ǿ��ֱ⿡ ���� ��������� ( �� �÷��̾ ����ϴ� ���� )
        landingStone = Instantiate(stoneTypes[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        // �ݶ��̴��� �ٿ��ֱ�
        stoneColl = landingStone.GetComponent<Collider>();
    }

}

public class Landing : BaseStone
{
    [SerializeField]
    private LayerMask boardLayer;               // ���̾��ũ
    private RaycastHit hit;                     // ����ĳ��Ʈ
    private Vector3 landingPoint;               // ������
    StoneManager stoneManager;                  // ���� �Ŵ���

    private void Awake()
    {
        stoneManager = GetComponent<StoneManager>();
    }

    private void Start()
    {
        FirstStoneAssign();         // ���̽��� �ִ� ù��° ���� �� ����
        InitializeStone();          // ������ġ �����ִ� �� �ʱ�ȭ
    }

    private void FixedUpdate()
    {
        // ���콺 ��ġ�� ��������Ʈ ����
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
            // ���콺 ���� Ŭ���ϸ� ������ ��ġ�� ���� ��� ���������� �̸�����
            // ���Ŀ� ��ġ�� ����
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

        // ���콺�� ����Ŭ���� ������ �ش��ڸ����� y������ 1��ŭ ������ ������
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

        // �����ϸ� �� ī��Ʈ ���̱�
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
    [Header("�÷��̾��� �� ����")]
    public int playerOneHaveStone = 10;
    public int playerTwoHaveStone = 10;

    [Space(10f)]
    [Header("�ڼ�ȿ����")]
    public float magnetRange = 2;       // �ڼ� �Ÿ�
    public float magnetMaxForce = 1;    // �ڼ� �ִ� �Ŀ�
    public float magnetTime = 3;        // �ڼ� ȿ�� �ð�
    public int clingStone = 0;          // ���� �� ����
    public LayerMask canClingLayer;     // ���� �� ���� ���̾�

    [Space(10f)]
    [Header("�� üũ")]
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

        // �ڼ�ȿ�� ���ֱ�
        // �� ��ü ( ����Ŭ�������� �ϱ�ü �������ֱ� )
    }
}*/