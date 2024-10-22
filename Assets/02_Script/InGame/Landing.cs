using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Define;

public class Landing : MonoBehaviour
{
    // 필요한 매니저
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    GameManager gameManager;

    [SerializeField] private LayerMask boardLayer;      // 보드판
    public Vector3 landingPoint;   // 착수될 위치

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
        // 현재 플레이어의 랜딩 시작
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
        // 마우스 위치로 랜딩포인트 변경
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, boardLayer))
        {
            landingPoint = hit.point;
        }
    }

    private void LandingPointStone()
    {
        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
        Vector3 originlandingPoint;
        originlandingPoint = stoneManager.landingStone.transform.position;
        originlandingPoint.y += 1f;
        stoneManager.landingStone.transform.position = originlandingPoint;

        GameObject pullStone;       // 실제 보드판 위에 생성될 돌

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

        Debug.Log($"플레이어 : {gameManager.CurrentPlayer} 남은돌 : {playerManager.stoneCount[gameManager.CurrentPlayer]}");

        pullStone.GetComponent<Magnet>().enabled = true;
        pullStone.GetComponent<Stone>().m_CurrentPlayer = gameManager.CurrentPlayer;
        
        gameManager.CurrentPlayerState = PlayerState.Wait;

    }
}