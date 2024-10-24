using UnityEngine;
using System.Linq;
using static Define;
using System;
using Protocol;

public class Landing : MonoBehaviour
{
    // 메시지 타입 정의
    private enum GameMessageType
    {
        StonePlacement = 0,
        GameState = 1
    }

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

    #region 매니저 초기화
    private void InitManager()
    {
        playerManager = GetComponent<PlayerManager>();
        stoneManager = GetComponent<StoneManager>();
        gameManager = GetComponent<GameManager>();
    }
    #endregion

    private void Start()
    {
        gameManager.OnGame += StartLanding;
    }

    private void StartLanding()
    {
        // 현재 턴이 아니면 리턴
        if (!BackendMatchManager.Instance.IsMyPlayer(GameManager.Instance.CurrentPlayer))
        {
            return; // 내 턴이 아니면 리턴
        }


        if (playerManager.stoneCount.Any(pair => pair.Value < -1))
        {
            gameManager.CurrentState = GameState.GameOver;
            return;
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

    public void ProcessStonePlacement(StonePlacementMessage msg)
    {
        Vector3 position = new Vector3(msg.posX, msg.posY, msg.posZ);

        // 돌 생성 및 설정
        GameObject pullStone = Instantiate(
            stoneManager._stoneTypes[msg.stoneIndex],
            position,
            Quaternion.identity
        );

        Player currentPlayer = (Player)Enum.Parse(typeof(Player), msg.player);
        playerManager.DecrementStoneCount(currentPlayer);
        pullStone.GetComponent<Magnet>().enabled = true;
        pullStone.GetComponent<Stone>().m_CurrentPlayer = currentPlayer;

        Debug.Log($"플레이어 : {currentPlayer} 남은돌 : {playerManager.stoneCount[currentPlayer]}");
        gameManager.CurrentPlayerState = PlayerState.Wait;
    }

    private void LandingPointStone()
    {
        // 마우스를 왼쪽클릭을 놓으면 해당자리에서 y축으로 1만큼 위에서 떨어짐
        Vector3 originlandingPoint = stoneManager.landingStone.transform.position;
        originlandingPoint.y += 1f;

        StonePlacementMessage placementMsg = new StonePlacementMessage(
            originlandingPoint,
            gameManager.CurrentPlayer == Player.One ? stoneManager.stoneIndex[0] : stoneManager.stoneIndex[1],
            gameManager.CurrentPlayer.ToString()
        );

        BackendMatchManager.Instance.SendDataToInGame(placementMsg);
    }
}