using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Landing : MonoBehaviour
{

    // �ʿ��� �Ŵ���
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    GameManager gameManager;

    [SerializeField] private LayerMask boardLayer;      // ������
    public Vector3 landingPoint { get; private set; }   // ������ ��ġ

    private void Awake()
    {
        InitManager();
    }

    private void Start()
    {
        gameManager.OnTurnChanged += StartLanding;
    }

    private void InitManager()
    {
        playerManager = GetComponent<PlayerManager>();
        stoneManager = GetComponent<StoneManager>();
        gameManager = GetComponent<GameManager>();
    }

    private void StartLanding(Player player)
    {
        // ���� �÷��̾��� ���� ����
        Debug.Log($"���� ���� {player}");

        if (Input.GetMouseButtonDown(0))
        {
            stoneManager.InitializeStone();
        }

        if (Input.GetMouseButton(0))
        {
            stoneManager.landingStone.transform.position = landingPoint;
            stoneManager.stoneColl.enabled = false;
            PreviewStone();
        }

        if (Input.GetMouseButtonUp(0))
        {
            stoneManager.stoneColl.enabled = true;
            Destroy(stoneManager.landingStone);
            LandingPointStone();
        }
    }

    private void PreviewStone()
    {
        // ���콺 ��ġ�� ��������Ʈ ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, boardLayer))
        {
            landingPoint = hit.point;
        }
    }

    private void LandingPointStone()
    {
        // ���콺�� ����Ŭ���� ������ �ش��ڸ����� y������ 1��ŭ ������ ������
        Vector3 originlandingPoint;
        originlandingPoint = stoneManager.landingStone.transform.position;
        originlandingPoint.y += 1f;
        stoneManager.landingStone.transform.position = originlandingPoint;

        GameObject pullStone;

        if (GameManager.Instance.CurrentPlayer == Player.One)
        {
            pullStone = Instantiate(stoneManager._stoneTypes[stoneManager.stoneIndex[0]], 
                stoneManager.landingStone.transform.position, Quaternion.identity);
        }
        else
        {
            pullStone = Instantiate(stoneManager._stoneTypes[stoneManager.stoneIndex[1]], 
                stoneManager.landingStone.transform.position, Quaternion.identity);
        }

        playerManager.DecrementStoneCount(GameManager.Instance.CurrentPlayer);
        pullStone.AddComponent<Magnet>();

        GameManager.Instance.CurrentPlayerState = PlayerState.Wait;
    }
}