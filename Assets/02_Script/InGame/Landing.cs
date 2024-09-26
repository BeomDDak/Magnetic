using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : BaseStone
{
    [SerializeField]
    private LayerMask layerMask;                // 레이어마스크
    private RaycastHit hit;                     // 레이캐스트
    private Vector3 landingPoint;               // 착지점
    StoneManager stoneManager;                  // 스톤 매니저

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
        // 빌딩시스템
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            landingPoint = hit.point;
        }
    }

    void Update()
    {
        // 빌딩시스템
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
        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
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