using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float magnetRange = 2f;
    public float magnetMaxForce = 1f;
    public float magnetTime;
    private Vector3 center;
    

    [SerializeField]
    private LayerMask canClingLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Board"))
        {
            center = GameManager.Instance.landing.landingPoint;
            StartCoroutine(PullStones());
        }

        if (collision.collider.CompareTag("Stone"))
        {
            collision.gameObject.AddComponent<Magnet>();
        }
    }

    private IEnumerator PullStones()
    {
        Collider[] otherStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);

        // 당기는 범위 안에 아무것도 없으면 턴 종료
        if(otherStones == null)
        {
            GameManager.Instance.SwitchTurn();
            yield break;
        }

        magnetTime = 3f;    // 3초간 당김
        
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
        GameManager.Instance.SwitchTurn();
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0f, distance / magnetRange);
    }
}
