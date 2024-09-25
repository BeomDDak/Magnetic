using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private GameObject stone;                   // 돌

    int stoneKind;                              // 돌 종류
    List<int> stoneIndex = new List<int>();     // 선택될 돌
    public GameObject[] playerStone;            // 플레이어가 사용할 돌
    private int turn = 0;                       // 턴

    [SerializeField]
    private LayerMask layerMask;                // 레이어마스크
    private RaycastHit hit;                     // 레이캐스트
    private Vector3 landingPoint;               // 착지점
    private Collider col;                       // 콜라이더

    private void Start()
    {
        stoneKind = playerStone.Length;
        Debug.Log(stoneKind);
        FirstRandomStone();
        InitializeStone();
        Debug.Log(stoneIndex[0]);
        Debug.Log(stoneIndex[1]);
    }

    void FirstRandomStone()
    {
        // 최초 돌 랜덤으로 리스트에 2가지 넣어놓음
        for (int i = 0; i < 2; i++)
        {
            int j = Random.Range(0, stoneKind);
            stoneIndex.Add(j);
        }
    }

    void InitializeStone()
    {
        stone = Instantiate(playerStone[stoneIndex[turn]], Vector3.zero, Quaternion.identity);
        col = stone.GetComponent<Collider>();
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
        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
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
        turn = GameManager.Instance.isPlayerOneTurn ? 0 : 1;
        InitializeStone();
    }
}