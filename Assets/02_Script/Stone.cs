using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{

    private GameObject stone;               // ��

    int stoneKind = 3;                      // �� ����
    List<int> stoneIndex = new List<int>();                   // ���õ� ��
    public GameObject[] playerStone;        // �÷��̾ ����� ��

    [SerializeField]
    private LayerMask layerMask;            // ���̾��ũ
    private RaycastHit hit;                 // ����ĳ��Ʈ
    private Vector3 landingPoint;           // ������
    private Collider col;                   // �ݶ��̴�

    private void Start()
    {
        FirstRandomStone();
        InitializeStone();
        Debug.Log(stoneIndex[0]);
        Debug.Log(stoneIndex[1]);
    }

    void FirstRandomStone()
    {
        // ���� �� �������� ����Ʈ�� 2���� �־����
        for (int i = 0; i < 2; i++)
        {
            int j = Random.Range(0, stoneKind);
            stoneIndex.Add(j);
        }
    }

    void InitializeStone()
    {
        stone = playerStone[stoneIndex[0]];

        col = stone.GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        // �����ý���
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            landingPoint = hit.point;
        }
    }

    void Update()
    {
        // �����ý���
        if (stone != null)
        {
            if (Input.GetMouseButton(0))
            {
                stone.transform.position = landingPoint;
                col.isTrigger = true;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                col.isTrigger = false;
                LandingPointStone();
            }
        }
    }

    void LandingPointStone()
    {
        // ���콺�� ����Ŭ���� ������ �ش��ڸ����� y������ 1��ŭ ������ ������
        Vector3 originlandingPoint;
        originlandingPoint = stone.transform.position;
        originlandingPoint.y += 1f;
        stone.transform.position = originlandingPoint;

        if (GameManager.Instance.isPlayerOneTurn)
        {
            Instantiate(playerStone[stoneIndex[0]], stone.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(playerStone[stoneIndex[1]], stone.transform.position, Quaternion.identity);
        }

        GameManager.Instance.SwitchTurn();
    }
}
