using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticStone : MonoBehaviour
{
    // �÷��̾� ���� �� ����
    public int playerOneHaveStone = 10;
    public int playerTwoHaveStone = 10;
    private int clingStone = 0;

    // �ڼ� �Ÿ�, �Ŀ�, ���ѽð�
    [SerializeField]
    private float magnetRange = 2;
    [SerializeField]
    private float magnetMaxForce = 1;
    [SerializeField]
    private LayerMask stoneLayer;

    // ���� �ð�
    private float turnTime = 10f;




    void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Board"))
        {
            StartCoroutine(PullStone());
        }
    }

    IEnumerator PullStone()
    {
        Collider[] otherStones = Physics.OverlapSphere(transform.position, magnetRange, stoneLayer);

        foreach (Collider stone in otherStones)
        {
            float distance = Vector3.Distance(transform.position, stone.transform.position);
            if(distance <= magnetRange)
            {
                float magnetForce = magnetMaxForce - (distance / magnetRange);
                Vector3 disLandingStone = transform.position - stone.transform.position;
                Vector3 dirLandingStone = disLandingStone.normalized;
                stone.transform.position += dirLandingStone * magnetForce * Time.deltaTime;
            }
        }

        yield return new WaitForSeconds(GameManager.Instance.magnetTime);

        // �ڼ�ȿ�� ���ֱ�
        // �� ��ü ( ����Ŭ�������� �ϱ�ü �������ֱ� )
    }
}
