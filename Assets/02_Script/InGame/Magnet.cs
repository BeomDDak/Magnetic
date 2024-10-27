using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;
using Protocol;

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

    private Rigidbody rb;           // ������ ������ vel��
    //public Coroutine syncCoroutine { get; set; }    // ������ ������ �ڷ�ƾ

    private void Awake()
    {
        m_stone = GetComponent<Stone>();
        rb = GetComponent<Rigidbody>();
        canClingLayer = 1 << 8;
    }

    private void Start()
    {
        center = GameManager.Instance.landing.landingPoint;
        StartCoroutine(PullStones());
        //syncCoroutine = StartCoroutine(SyncStoneState());
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
            //StopCoroutine(syncCoroutine);
            yield break;
        }

        Debug.Log($"�ֺ��� {anyStones.Length}���� ���� �����Ǿ����ϴ�.");

        // ������ �޴� ��� ������ ����
        List<Stone> affectedStones = new List<Stone>();
        foreach (Collider stone in anyStones)
        {
            Stone stoneComponent = stone.GetComponent<Stone>();
            affectedStones.Add(stoneComponent);
        }

        while (magnetTime > 0f)
        {
            magnetTime -= Time.deltaTime;
            foreach (Stone stone in affectedStones)
            {
                float distance = Vector3.Distance(center, stone.transform.position);
                if (distance <= magnetRange)
                {
                    float magnetForce = CalculateMagnetForce(distance);
                    Vector3 pullDirection = (center - stone.transform.position).normalized;
                    stone.transform.position += pullDirection * magnetForce * Time.deltaTime;

                    // ��� ����� ������ ���µ� �����Ͽ� ����ȭ
                    List<Stone> connectedStones = new List<Stone>();
                    foreach (var obj in stone.CountConnectedObjects())
                    {
                        Stone connectedStone = obj.GetComponent<Stone>();
                        if (connectedStone != null)
                        {
                            connectedStones.Add(connectedStone);
                        }
                    }

                    // �� ���� ���� ����ȭ
                    foreach (Stone stonestate in connectedStones)
                    {
                        StoneSyncMessage msg = new StoneSyncMessage(
                            stone.stoneId,
                            stone.transform.position,
                            stone.transform.rotation,
                            stone.GetComponent<Rigidbody>().velocity,
                            stone.GetComponent<Rigidbody>().angularVelocity,
                            stone.CountConnectedObjects().Select(obj => obj.GetComponent<Stone>().stoneId).ToList()
                        );
                        BackendMatchManager.Instance.SendDataToInGame(msg);
                    }
                }
            }
            yield return null;
        }

        if (cling && OppentStone())
        {
            StartCoroutine(SyncStoneState());
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

        if (!isSwitchTurn)
        {
            GameManager.Instance.SwitchTurn();
            isSwitchTurn = true;
        }
        //StopCoroutine(syncCoroutine);
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

    // ���� ���¸� ����ȭ�ϴ� �ڷ�ƾ
    private IEnumerator SyncStoneState()
    {
        while (magnetTime > 0f)
        {
            // �پ��ִ� ��� ���� ID ����
            List<string> attachedIds = new List<string>();
            foreach (var obj in m_stone.CountConnectedObjects())
            {
                Stone attachedStone = obj.GetComponent<Stone>();
                if (attachedStone != null)
                {
                    attachedIds.Add(attachedStone.stoneId);
                }
            }

            // ���� ����ȭ �޽��� ����
            StoneSyncMessage msg = new StoneSyncMessage(
                m_stone.stoneId,
                transform.position,
                transform.rotation,
                rb.velocity,
                rb.angularVelocity,
                attachedIds
            );
            BackendMatchManager.Instance.SendDataToInGame(msg);

            yield return new WaitForSeconds(0.1f);  // 0.1�ʸ��� ����ȭ
        }
    }
}