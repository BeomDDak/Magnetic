using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    SceneLoader loader;

    public float[] moveSpeed;
    float m_time = 0.3f;
    public GameObject[] productObj;

    private void Awake()
    {
        loader = GameObject.FindGameObjectWithTag("Loader").GetComponent<SceneLoader>();
    }

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

    public void OnClickGameStart()
    {
        loader.LoadScene(SceneType.Lobby);
    }
}
