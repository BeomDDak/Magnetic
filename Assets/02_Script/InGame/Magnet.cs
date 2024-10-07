using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;

public class Magnet : MonoBehaviour
{
    GameManager gameManager;
    Landing landing;
    public float magnetRange = 0.5f;
    public float magnetMaxForce = 1f;
    public float magnetTime;
    private Vector3 center;
    private int canClingLayer;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        landing = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Landing>();
        canClingLayer = 1 << 9;
        this.GetComponent<Magnet>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("������ �ε���");
        if (collision.collider.CompareTag("Board"))
        {
            this.gameObject.GetComponent<Magnet>().enabled = true;
            center = landing.landingPoint;
            StartCoroutine(PullStones());
        }
    }

    public IEnumerator PullStones()
    {
        Debug.Log("Ǯ���� ����");
        
        Collider[] anyStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);
        Collider[] opponentStones = anyStones
            .Where(collider => collider.gameObject != this.gameObject)
            .Where(collider =>
            {
                Stone stone = collider.gameObject.GetComponent<Stone>();
                return stone != null && stone.m_CurrentPlayer != gameManager.CurrentPlayer;
            }).ToArray();

        if (opponentStones.Length == 0)
        {
            Debug.Log("�ֺ��� ���� �����ϴ�. ���� �����մϴ�.");
            gameManager.SwitchTurn();
            yield break;
        }

        Debug.Log($"�ֺ��� {anyStones.Length}���� ���� �����Ǿ����ϴ�.");

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
            yield return null;
        }

        if(opponentStones.Length != 0)
        {
            // ���� �÷��̾� ����
            Player opponentPlayer = (gameManager.CurrentPlayer == Player.One) ? Player.Two : Player.One;

            // ���� �÷��̾��� �� ���� ����
            gameManager.playerManager.IncrementStoneCount(opponentStones.Length, opponentPlayer);

            foreach (var stone in opponentStones)
            {
                Destroy(stone.gameObject);
            }
            Destroy(gameObject);
            
        }

        gameManager.SwitchTurn();
        this.GetComponent<Magnet>().enabled = false;
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0f, distance / magnetRange);
    }
}
