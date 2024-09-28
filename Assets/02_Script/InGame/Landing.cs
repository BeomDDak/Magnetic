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
        // ���콺 ��ġ�� ��������Ʈ ����
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
            // ���콺 ���� Ŭ���ϸ� ������ ��ġ�� ���� ��� ���������� �̸�����
            // ���Ŀ� ��ġ�� ����
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
        // ���콺�� ����Ŭ���� ������ �ش��ڸ����� y������ 1��ŭ ������ ������
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