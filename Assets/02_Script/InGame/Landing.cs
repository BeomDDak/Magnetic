using UnityEngine;
using System.Linq;
using static Define;
using System;
using Protocol;

public class Landing : MonoBehaviour
{
    // �޽��� Ÿ�� ����
    private enum GameMessageType
    {
        StonePlacement = 0,
        GameState = 1
    }

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

    #region �Ŵ��� �ʱ�ȭ
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
        // ���� ���� �ƴϸ� ����
        if (!BackendMatchManager.Instance.IsMyPlayer(GameManager.Instance.CurrentPlayer))
        {
            return; // �� ���� �ƴϸ� ����
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
        // ���콺 ��ġ�� ��������Ʈ ����
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

        // �� ���� �� ����
        GameObject pullStone = Instantiate(
            stoneManager._stoneTypes[msg.stoneIndex],
            position,
            Quaternion.identity
        );

        Player currentPlayer = (Player)Enum.Parse(typeof(Player), msg.player);
        playerManager.DecrementStoneCount(currentPlayer);
        pullStone.GetComponent<Magnet>().enabled = true;
        pullStone.GetComponent<Stone>().m_CurrentPlayer = currentPlayer;

        Debug.Log($"�÷��̾� : {currentPlayer} ������ : {playerManager.stoneCount[currentPlayer]}");
        gameManager.CurrentPlayerState = PlayerState.Wait;
    }

    private void LandingPointStone()
    {
        // ���콺�� ����Ŭ���� ������ �ش��ڸ����� y������ 1��ŭ ������ ������
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