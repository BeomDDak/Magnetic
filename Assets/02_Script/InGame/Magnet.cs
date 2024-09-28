using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private float magnetRange = 2f;
    private float magnetMaxForce = 1f;
    private float magnetTime = 3f;

    public void ApplyMagneticEffect(Vector3 center, List<Stone> stones)
    {
        GameManager.Instance.StartCoroutine(PullStones(center, stones));
    }

    private IEnumerator PullStones(Vector3 center, List<Stone> stones)
    {
        float elapsedTime = 0f;
        while (elapsedTime < magnetTime)
        {
            foreach (var stone in stones)
            {
                float distance = Vector3.Distance(center, stone.transform.position);
                if (distance <= magnetRange)
                {
                    float magnetForce = CalculateMagnetForce(distance);
                    Vector3 pullDirection = (center - stone.transform.position).normalized;
                    stone.ApplyForce(pullDirection * magnetForce);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private float CalculateMagnetForce(float distance)
    {
        return Mathf.Lerp(magnetMaxForce, 0, distance / magnetRange);
    }
}
