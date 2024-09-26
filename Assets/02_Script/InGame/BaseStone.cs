using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStone : MonoBehaviour
{
    protected GameObject stone;                           // �� ( �����Ǳ� �� ������ ������Ʈ )
    protected int stoneKind;                              // �� ���� ( �� ������ �̱� ���� )
    protected List<int> stoneIndex = new List<int>();     // ���õ� ��
    protected GameObject[] playerStone;                   // �÷��̾ ����� �� ( �÷��̾ ����� ���� ���� )
    protected int whoTurn = 0;                            // �� ( 0 �϶��� �÷��̾�1 1�϶��� �÷��̾�2 )
    protected Collider stoneColl;                         // �ݶ��̴�

    // ���� ���۽� �� ���� ����
    protected void FirstStoneAssign()
    {
        // ���� �� �������� ����Ʈ�� 2���� �־����
        for (int i = 0; i < 2; i++)
        {
            int j = Random.Range(0, stoneKind);
            stoneIndex.Add(j);
        }
    }

    // �� �ʱ�ȭ
    protected void InitializeStone()
    {
        stone = Instantiate(playerStone[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
        stoneColl = stone.GetComponent<Collider>();
    }

}
