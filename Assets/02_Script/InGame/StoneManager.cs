using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StoneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] StoneTypes;   // �� ����
    public GameObject[] _stoneTypes    // �� ������ �ٸ��������� ���� ���� ���ϰ� �Ϸ��� ���� ������Ƽ
    {   
        get { return StoneTypes; } 
        private set { StoneTypes = value; }
    }

    public GameObject landingStone;    // �� ( �����Ǳ� �� ������ ������Ʈ )
    public int stoneTypesCount { get; private set; }  // �� ���� ���� ( �� ������ �̱� ���� )
    public List<int> stoneIndex { get; private set; } = new List<int>(); // �÷��̾ ����ϴ� ���� �ε����� ����
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

    // ���� ���۽� �� ���� ����
    public void FirstStoneAssign()
    {
        // �� ������ ���� ����
        stoneTypesCount = _stoneTypes.Length;

        // �����ϸ� �������� �÷��̾ ����� �� ���ϱ�
        for (int i = 0; i < (int)Player.Count; i++)
        {
            // 0��°�� ����Ǵ� ���� 1�÷��̾ ����ҰŰ�
            // 1��°�� ����Ǵ� ���� 2�÷��̾ ����Ұ���
            int j = Random.Range(0, stoneTypesCount);
            stoneIndex.Add(j);

        }
        Debug.Log("�÷��̾�1 : " + stoneIndex[0]);
        Debug.Log("�÷��̾�2 : " + stoneIndex[1]);

        Debug.Log("�� ���� �Ϸ�");
    }

    // �� �ʱ�ȭ
    public void InitializeStone()
    {
        if(gameManager.CurrentPlayer == Player.One)
        {
            // ���� ������ �Ҵ縸 �Ǿ��ֱ⿡ ���� ��������� ( �� �÷��̾ ����ϴ� ���� )
            landingStone = Instantiate(_stoneTypes[stoneIndex[0]], Vector3.zero, Quaternion.identity);
        }
        else
        {
            // ���� ������ �Ҵ縸 �Ǿ��ֱ⿡ ���� ��������� ( �� �÷��̾ ����ϴ� ���� )
            landingStone = Instantiate(_stoneTypes[stoneIndex[1]], Vector3.zero, Quaternion.identity);
        }

        // �ݶ��̴��� �ٿ��ֱ�
        stoneColl = landingStone.GetComponent<Collider>();
    }
}
