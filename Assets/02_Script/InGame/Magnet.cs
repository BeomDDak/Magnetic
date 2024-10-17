using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;

public class Magnet : MonoBehaviour
{
    GameManager gameManager;
    Landing landing;
    PlayerManager playerManager;
    Stone m_stone;

    public float magnetRange;
    public float magnetMaxForce;
    public float magnetTime;
    private Vector3 center;
    private int canClingLayer;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        landing = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Landing>();
        playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        canClingLayer = 1 << 9;
        this.GetComponent<Magnet>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Board"))
        {
            this.gameObject.GetComponent<Magnet>().enabled = true;
            m_stone = GetComponent<Stone>();
            center = landing.landingPoint;
            StartCoroutine(PullStones());
        }
    }

    public IEnumerator PullStones()
    {
        Debug.Log("풀스톤 시작");

        magnetRange = 0.5f;
        magnetMaxForce = 1f;

        Collider[] anyStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);

        if (anyStones.Length == 1)
        {
            Debug.Log("주변에 돌이 없습니다. 턴을 종료합니다.");
            gameManager.SwitchTurn();
            yield break;
        }

        Debug.Log($"주변에 {anyStones.Length}개의 돌이 감지되었습니다.");

        magnetTime = 3f;
        
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
            Debug.Log(magnetMaxForce);
            yield return null;
        }

        Debug.Log(m_stone.joints.Count);
        if(m_stone.joints.Count != 0)
        {
            // 상대방 플레이어 결정
            Player opponentPlayer = (gameManager.CurrentPlayer == Player.One) ? Player.Two : Player.One;

            // 상대방 플레이어의 돌 갯수 증가
            playerManager.IncrementStoneCount(m_stone.CountConnectedObjects().Count, opponentPlayer);

            for(int i = 0; i < m_stone.CountConnectedObjects().Count; i++)
            {
                m_stone.CountConnectedObjects()[i].SetActive(false);
            }

            gameManager.SwitchTurn();
            this.gameObject.SetActive(false);
        }

        gameManager.SwitchTurn();
        this.GetComponent<Magnet>().enabled = false;
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0f, distance / magnetRange);
    }
}
