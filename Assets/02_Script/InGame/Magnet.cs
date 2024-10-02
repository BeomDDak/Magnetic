using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private float magnetRange = 2f;
    private float magnetMaxForce = 1f;
    private float magnetTime;
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
    }

    private IEnumerator PullStones()
    {
        Collider[] otherStones = Physics.OverlapSphere(center, magnetRange, canClingLayer);

        // 범위 안에 아무것도 없으면 턴 종료
        if(otherStones == null)
        {
            GameManager.Instance.OnTurnChanged();
        }
        else
        {
            magnetTime = 3f;
            magnetTime -= Time.deltaTime;
            while (magnetTime >= 0f)
            {
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
        }
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0, distance / magnetRange);
    }
}
