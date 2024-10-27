using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;
using Protocol;

public class Magnet : MonoBehaviour
{
    Stone m_stone;

    public float magnetRange;       // 자성 범위
    public float magnetMaxForce;    // 자성 파워
    public float magnetTime;        // 자성 시간
    private Vector3 center;         // 돌의 중심점 (끌어당길 중심점)
    private int canClingLayer;      // 끌어 당길 수 있는 레이어
    public bool cling;              // 다른 돌과 부딪혔는지 판단
    private bool isSwitchTurn;      // 턴 교체 중인지 판단 (충돌하거나 연결될 시 여러 스톤에서 중복 호출 발생된 것을 확인)

    private Rigidbody rb;           // 서버에 전달할 vel값
    //public Coroutine syncCoroutine { get; set; }    // 서버에 전달할 코루틴

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
        Debug.Log("풀스톤 시작");

        magnetRange = 0.5f;
        magnetMaxForce = 1f;
        magnetTime = 3f;

        Collider[] anyStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);
       
        if (anyStones.Length == 0)
        {
            Debug.Log("주변에 돌이 없습니다. 턴을 종료합니다.");
            GameManager.Instance.SwitchTurn();
            GetComponent<Magnet>().enabled = false;
            //StopCoroutine(syncCoroutine);
            yield break;
        }

        Debug.Log($"주변에 {anyStones.Length}개의 돌이 감지되었습니다.");

        // 영향을 받는 모든 돌들을 저장
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

                    // 모든 연결된 돌들의 상태도 포함하여 동기화
                    List<Stone> connectedStones = new List<Stone>();
                    foreach (var obj in stone.CountConnectedObjects())
                    {
                        Stone connectedStone = obj.GetComponent<Stone>();
                        if (connectedStone != null)
                        {
                            connectedStones.Add(connectedStone);
                        }
                    }

                    // 각 돌의 상태 동기화
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
            // 상대방 플레이어 결정
            Player opponentPlayer = (GameManager.Instance.CurrentPlayer == Player.One) ? Player.Two : Player.One;

            // 상대방 플레이어의 돌 갯수 증가
            GameManager.Instance.playerManager.IncrementStoneCount(m_stone.CountConnectedObjects().Count, opponentPlayer);
            Debug.Log($"붙은갯수:{m_stone.CountConnectedObjects().Count} 추가되는 곳:{opponentPlayer}");
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

    // 돌의 상태를 동기화하는 코루틴
    private IEnumerator SyncStoneState()
    {
        while (magnetTime > 0f)
        {
            // 붙어있는 모든 돌의 ID 수집
            List<string> attachedIds = new List<string>();
            foreach (var obj in m_stone.CountConnectedObjects())
            {
                Stone attachedStone = obj.GetComponent<Stone>();
                if (attachedStone != null)
                {
                    attachedIds.Add(attachedStone.stoneId);
                }
            }

            // 상태 동기화 메시지 전송
            StoneSyncMessage msg = new StoneSyncMessage(
                m_stone.stoneId,
                transform.position,
                transform.rotation,
                rb.velocity,
                rb.angularVelocity,
                attachedIds
            );
            BackendMatchManager.Instance.SendDataToInGame(msg);

            yield return new WaitForSeconds(0.1f);  // 0.1초마다 동기화
        }
    }
}