using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _stoneTypes;                           // �� ����
    protected GameObject[] stoneTypes                           // �� ������ �ٸ��������� ���� ���� ���ϰ� �Ϸ��� ���� ������Ƽ
    {   
        get { return _stoneTypes; } 
        private set { _stoneTypes = value; }
    }                                                           

    protected GameObject landingStone { get; private set; }     // �� ( �����Ǳ� �� ������ ������Ʈ )
    protected int stoneTypesCount { get; private set; }         // �� ���� ���� ( �� ������ �̱� ���� )
    protected List<int> stoneIndex = new List<int>();           // �÷��̾ ����ϴ� ���� �ε����� ����
    protected int whoTurn = 0;                                  // �� ( 0 �϶��� �÷��̾�1 1�϶��� �÷��̾�2 )
    protected Collider stoneColl { get; set; }                  // �ݶ��̴�
    protected GameManager GameManager { get; private set; }     // ���ӸŴ���

    public StoneManager()
    {
        FirstStoneAssign();
    }

    // ���� ���۽� �� ���� ����
    private void FirstStoneAssign()
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
    public void InitializeStone()
    {
        // ���� ������ �Ҵ縸 �Ǿ��ֱ⿡ ���� ��������� ( �� �÷��̾ ����ϴ� ���� )
        landingStone = Instantiate(stoneTypes[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        // �ݶ��̴��� �ٿ��ֱ�
        stoneColl = landingStone.GetComponent<Collider>();
    }

}
