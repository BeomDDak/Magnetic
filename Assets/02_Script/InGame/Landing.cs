using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : BaseStone
{
    [SerializeField]
    private LayerMask layerMask;                // ���̾��ũ
    private RaycastHit hit;                     // ����ĳ��Ʈ
    private Vector3 landingPoint;               // ������
    StoneManager stoneManager;                  // ���� �Ŵ���

    private void Awake()
    {
        stoneManager = GetComponent<StoneManager>();
    }

    private void Start()
    {
        stoneKind = playerStone.Length;
        FirstStoneAssign();
        InitializeStone();
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
                stoneColl.enabled = false;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                stoneColl.enabled = true;
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

        if (stoneManager.isPlayerOneTurn)
        {
            Instantiate(playerStone[stoneIndex[0]], stone.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(playerStone[stoneIndex[1]], stone.transform.position, Quaternion.identity);
        }

        stoneManager.SwitchTurn();
        whoTurn = stoneManager.isPlayerOneTurn ? 0 : 1;
        InitializeStone();
    }
}