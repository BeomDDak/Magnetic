using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Landing : StoneManager
{
    [SerializeField]
    private LayerMask boardLayer;
    private Vector3 landingPoint;

    private void FixedUpdate()
    {
        if (GameManager.CurrentPlayerState != PlayerState.PlayTime)
            return;
        // 마우스 위치로 랜딩포인트 변경
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, boardLayer))
        {
            landingPoint = hit.point;
        }
    }

    private void Update()
    {
        if (landingStone != null)
        {
            // 마우스 왼쪽 클릭하면 착수될 위치에 돌이 어떻게 떨어지는지 미리보기
            // 추후에 터치로 변경
            if (Input.GetMouseButton(0))
            {
                landingStone.transform.position = landingPoint;
                stoneColl.enabled = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                stoneColl.enabled = true;
                Destroy(landingStone);
                LandingPointStone();
            }
        }
    }
    private void LandingPointStone()
    {
        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
        Vector3 originlandingPoint;
        originlandingPoint = landingStone.transform.position;
        originlandingPoint.y += 1f;
        landingStone.transform.position = originlandingPoint;

        GameObject pullStone;

        if (GameManager.CurrentPlayer == Player.One)
        {
            pullStone = Instantiate(stoneTypes[stoneIndex[0]], landingStone.transform.position, Quaternion.identity);
        }
        else
        {
            pullStone = Instantiate(stoneTypes[stoneIndex[1]], landingStone.transform.position, Quaternion.identity);
        }

        pullStone.AddComponent<Magnet>();

        InitializeStone();
        GameManager.CurrentPlayerState = PlayerState.Wait;
    }
}