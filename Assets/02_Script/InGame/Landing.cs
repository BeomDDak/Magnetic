using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;

public class Landing : MonoBehaviour
{
    // �ʿ��� �Ŵ���
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    GameManager gameManager;

    [SerializeField] private LayerMask boardLayer;      // ������
    public Vector3 landingPoint;   // ������ ��ġ

    private void Awake()
    {
        InitManager();
    }

    private void Start()
    {
        gameManager.OnGame += StartLanding;
    }

    private void InitManager()
    {
        playerManager = GetComponent<PlayerManager>();
        stoneManager = GetComponent<StoneManager>();
        gameManager = GetComponent<GameManager>();
    }

    private void StartLanding()
    {
        // ���� �÷��̾��� ���� ����
        if(playerManager.stoneCount.Any(pair => pair.Value == 0))
        {
            gameManager.CurrentState = GameState.GameOver;
        }

        if (Input.GetMouseButtonDown(0))
        {
            stoneManager.InitializeStone();
        }

        if (Input.GetMouseButton(0))
        {
            stoneManager.landingStone.transform.position = landingPoint;
            stoneManager.stoneColl.enabled = false;
            PreviewLandingPoint();
        }

        if (Input.GetMouseButtonUp(0))
        {
            stoneManager.stoneColl.enabled = true;
            Destroy(stoneManager.landingStone);
            LandingPointStone();
        }
    }

    private void PreviewLandingPoint()
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

        GameObject pullStone;       // ���� ������ ���� ������ ��

        if (gameManager.CurrentPlayer == Player.One)
        {
            pullStone = Instantiate(stoneManager._stoneTypes[stoneManager.stoneIndex[0]], 
                stoneManager.landingStone.transform.position, Quaternion.identity);
        }
        else
        {
            pullStone = Instantiate(stoneManager._stoneTypes[stoneManager.stoneIndex[1]], 
                stoneManager.landingStone.transform.position, Quaternion.identity);
        }

        playerManager.DecrementStoneCount(gameManager.CurrentPlayer);

        Debug.Log($"�÷��̾� : {gameManager.CurrentPlayer} ������ : {playerManager.stoneCount[gameManager.CurrentPlayer]}");

        pullStone.GetComponent<Magnet>().enabled = true;
        pullStone.GetComponent<Stone>().m_CurrentPlayer = gameManager.CurrentPlayer;
        
        gameManager.CurrentPlayerState = PlayerState.Wait;

    }
}