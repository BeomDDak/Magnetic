using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour
{
    private GameObject stone;                   // ��

    int stoneKind;                              // �� ����
    List<int> stoneIndex = new List<int>();     // ���õ� ��
    public GameObject[] playerStone;            // �÷��̾ ����� ��
    private int whoTurn = 0;                    // ��

    [SerializeField]
    private LayerMask layerMask;                // ���̾��ũ
    private RaycastHit hit;                     // ����ĳ��Ʈ
    private Vector3 landingPoint;               // ������
    private Collider col;                       // �ݶ��̴�

    private void Start()
    {
        stoneKind = playerStone.Length;
        Debug.Log(stoneKind);
        FirstStoneAssign();
        InitializeStone();
        Debug.Log(stoneIndex[0]);
        Debug.Log(stoneIndex[1]);
    }

    void FirstStoneAssign()
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
        stone = Instantiate(playerStone[stoneIndex[whoTurn]], Vector3.zero, Quaternion.identity);
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
                col.enabled = false;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                col.enabled = true;
                Destroy(stone);
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
        whoTurn = GameManager.Instance.isPlayerOneTurn ? 0 : 1;
        InitializeStone();
    }
}