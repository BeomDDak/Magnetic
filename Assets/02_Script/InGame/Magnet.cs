using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;

public class Magnet : MonoBehaviour
{
    Stone m_stone;

    public float magnetRange;       // �ڼ� ����
    public float magnetMaxForce;    // �ڼ� �Ŀ�
    public float magnetTime;        // �ڼ� �ð�
    private Vector3 center;         // ���� �߽��� (������ �߽���)
    private int canClingLayer;      // ���� ��� �� �ִ� ���̾�
    public bool cling;              // �ٸ� ���� �ε������� �Ǵ�
    private bool isSwitchTurn;      // �� ��ü ������ �Ǵ� (�浹�ϰų� ����� �� ���� ���濡�� �ߺ� ȣ�� �߻��� ���� Ȯ��)

    private void Awake()
    {
        m_stone = GetComponent<Stone>();
        canClingLayer = 1 << 8;
    }

    private void Start()
    {
        center = GameManager.Instance.landing.landingPoint;
        StartCoroutine(PullStones());
    }

    public IEnumerator PullStones()
    {
        Debug.Log("Ǯ���� ����");

        magnetRange = 0.5f;
        magnetMaxForce = 1f;
        magnetTime = 3f;

        Collider[] anyStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);
       

        if (anyStones.Length == 0)
        {
            Debug.Log("�ֺ��� ���� �����ϴ�. ���� �����մϴ�.");
            GameManager.Instance.SwitchTurn();
            GetComponent<Magnet>().enabled = false;
            yield break;
        }

        Debug.Log($"�ֺ��� {anyStones.Length}���� ���� �����Ǿ����ϴ�.");

        while (magnetTime > 0f)
        {
            magnetTime -= Time.deltaTime;
            foreach (Collider stone in anyStones)
            {
                float distance = Vector3.Distance(center, stone.transform.position);
                if (distance <= magnetRange)
                {
                    float magnetForce = CalculateMagnetForce(distance);
                    Vector3 pullDirection = (center - stone.transform.position).normalized;
                    stone.transform.position += pullDirection * magnetForce * Time.deltaTime;
                }
            }
            yield return null;
        }

        if (cling)
        {
            if (OppentStone())
            {
                // ���� �÷��̾� ����
                Player opponentPlayer = (GameManager.Instance.CurrentPlayer == Player.One) ? Player.Two : Player.One;

                // ���� �÷��̾��� �� ���� ����
                GameManager.Instance.playerManager.IncrementStoneCount(m_stone.CountConnectedObjects().Count, opponentPlayer);
                Debug.Log($"��������:{m_stone.CountConnectedObjects().Count} �߰��Ǵ� ��:{opponentPlayer}");
                Debug.Log(GameManager.Instance.playerManager.stoneCount);

                for (int i = 0; i < m_stone.CountConnectedObjects().Count; i++)
                {
                    m_stone.CountConnectedObjects()[i].SetActive(false);
                }

                this.gameObject.SetActive(false);
            }
        }

        if (!isSwitchTurn)
        {
            GameManager.Instance.SwitchTurn();
            isSwitchTurn = true;
        }
        GetComponent<Magnet>().enabled = false;
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0f, distance / magnetRange);
    }

    private bool OppentStone()
    {
        foreach (GameObject stone in m_stone.CountConnectedObjects())
        {
            var stonePlayer = stone.GetComponent<Stone>().m_CurrentPlayer;

            if (stonePlayer != GameManager.Instance.CurrentPlayer)
            {
                return true;
            }
        }
        return false;
    }
}
