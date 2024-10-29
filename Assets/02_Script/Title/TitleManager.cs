using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject LoginUI;

    public float[] moveSpeed;
    float m_time = 0.3f;
    public GameObject[] productObj;

    private void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(1f);

        while (m_time > 0f)
        {
            m_time -= Time.deltaTime;
            for(int i = 0; i < productObj.Length; i++)
            {
                productObj[i].transform.Translate(Vector3.right * moveSpeed[i] * Time.deltaTime);
            }
            yield return null;
        }
    }
}
