using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] StoneTypes;                        // �� ����
    public GameObject[] _stoneTypes                         // �� ������ �ٸ��������� ���� ���� ���ϰ� �Ϸ��� ���� ������Ƽ
    {   
        get { return StoneTypes; } 
        private set { StoneTypes = value; }
    }

    public GameObject landingStone { get; private set; }     // �� ( �����Ǳ� �� ������ ������Ʈ )
    public int stoneTypesCount { get; private set; }         // �� ���� ���� ( �� ������ �̱� ���� )
    public List<int> stoneIndex { get; private set; } = new List<int>();           // �÷��̾ ����ϴ� ���� �ε����� ����
    public int whoTurn = 0;                                  // �� ( 0 �϶��� �÷��̾�1 1�϶��� �÷��̾�2 )
    public Collider stoneColl { get; set; }                  // �ݶ��̴�
    GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        gameManager.OnStartGame += FirstStoneAssign;
    }

    // ���� ���۽� �� ���� ����
    private void FirstStoneAssign()
    {
        // �� ������ ���� ����
        stoneTypesCount = _stoneTypes.Length;

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
    public void InitializeStone()
    {
        // ���� ������ �Ҵ縸 �Ǿ��ֱ⿡ ���� ��������� ( �� �÷��̾ ����ϴ� ���� )
        landingStone = Instantiate(StoneTypes[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        // �ݶ��̴��� �ٿ��ֱ�
        stoneColl = landingStone.GetComponent<Collider>();
    }
}
