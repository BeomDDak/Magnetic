using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Landing : MonoBehaviour
{

    // 필요한 매니저
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    GameManager gameManager;

    [SerializeField] private LayerMask boardLayer;      // 보드판
    public Vector3 landingPoint { get; private set; }   // 착수될 위치

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
        // 현재 플레이어의 랜딩 시작
        Debug.Log($"랜딩 시작 {player}");

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