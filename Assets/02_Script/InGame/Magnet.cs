using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Debug.Log("보드판 부딪힘");
        if (collision.collider.CompareTag("Board"))
        {
            this.gameObject.GetComponent<Magnet>().enabled = true;
            center = landing.landingPoint;
            StartCoroutine(PullStones());
        }
    }

    public IEnumerator PullStones()
    {
        Debug.Log("풀스톤 시작");
        
        Collider[] otherStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);

        if (otherStones.Length == 1)
        {
            Debug.Log("주변에 돌이 없습니다. 턴을 종료합니다.");
            gameManager.SwitchTurn();
            yield break;
        }

        Debug.Log($"주변에 {otherStones.Length}개의 돌이 감지되었습니다.");

        magnetTime = 3f;

        while (magnetTime > 0f)
        {
            magnetTime -= Time.deltaTime;
            foreach (Collider stone in otherStones)
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
        gameManager.SwitchTurn();
        this.GetComponent<Magnet>().enabled = false;
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0f, distance / magnetRange);
    }
}
