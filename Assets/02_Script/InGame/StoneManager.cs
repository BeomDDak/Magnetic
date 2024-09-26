using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneManager : Singleton<StoneManager>
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
    public LayerMask canClingLayer;        // 붙을 수 있을 레이어

    [Space(10f)]
    [Header("턴 체크")]
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

        // 자석효과 없애기
        // 턴 교체 ( 스톤클래스에서 턴교체 삭제해주기 )
    }
}
