using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneManager : Singleton<StoneManager>
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
    public LayerMask canClingLayer;        // ���� �� ���� ���̾�

    [Space(10f)]
    [Header("�� üũ")]
    public bool isPlayerOneTurn = true;
    public float turnTimeLimit = 10f;

    public void SwitchTurn()
    {
        isPlayerOneTurn = !isPlayerOneTurn;
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
}
