using UnityEngine;
using System;
using static Define;
using BackEnd.Tcp;
using Protocol;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    // ���� 
    public GameState CurrentState;
    public Player CurrentPlayer;
    public PlayerState CurrentPlayerState;

    // �ٸ� ��ũ��Ʈ ����
    private StoneManager stoneManager;
    public PlayerManager playerManager;
    public Landing landing;

    // �׼�
    public Action<GameState> OnGameStateChanged;      // ���� ���� ����
    public Action<Player> OnTurnChanged;              // �� ����
    public Action OnStartGame;
    public Action OnGame;

    private Camera cam;
    [SerializeField]
    private GameObject[] camPosition;

    protected override void Init()
    {
        base.Init();
        isDestoryOnLoad = true;
        InitializeManagers();
        cam = Camera.main;
    }

    // �Ŵ��� �ʱ⼳��
    private void InitializeManagers()
    {
        playerManager = GetComponent<PlayerManager>();
        stoneManager = GetComponent<StoneManager>();
        landing = GetComponent<Landing>();
    }

    // ���� ���۽� ���� �۾�
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1�÷��̾� ���� �����ϱ� ���ؼ�
        CurrentPlayerState = PlayerState.PlayTime;
        Debug.Log("���� ���� : " + CurrentPlayer);
    }
    
    // ���� ������ ���� �۾�
    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        // ���⿡ �κ�� ������ ��� �߰��ؾ���
    }

    // �� ����
    public void SwitchTurn()
    {
        CurrentPlayer = (CurrentPlayer == Player.One) ? Player.Two : Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        Debug.Log("���� ���� : " + CurrentPlayer);
    }

    // ī�޶�
    private void SetCameraPosition()
    {
        if(BackendMatchManager.Instance.IsMyPlayer(Player.One))
        {
            cam.transform.parent = camPosition[0].transform;
        }
        else
        {
            cam.transform.parent = camPosition[1].transform;
        }
        Debug.Log(cam.transform);

        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
    }

    public void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Debug.LogWarning(string.Format("�� �����Ͱ� ��ε�ĳ���� �Ǿ����ϴ�.\n{0} - {1}", args.From, args.ErrInfo));
            return;
        }

        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }

        if (BackendMatchManager.Instance.players == null)
        {
            Debug.LogError("Players ������ �������� �ʽ��ϴ�.");
            return;
        }

        // �� ���� ���� ������ ���� �Է� ��, �޼����� �Ѱ��ֱ�
        switch (msg.type)
        {
            case Protocol.Type.GameStart:       // ���� ���� ( ī�޶� )
                GameStartMessage gameStart = DataParser.ReadJsonData<GameStartMessage>(args.BinaryUserData);
                SetCameraPosition();
                StartGame();
                break;
            case Protocol.Type.StonePlacement:      // ����
                StonePlacementMessage stoneMsg = DataParser.ReadJsonData<StonePlacementMessage>(args.BinaryUserData);
                landing.ProcessStonePlacement(stoneMsg);
                break;
            case Protocol.Type.StoneSync:
                StoneSyncMessage syncMsg = DataParser.ReadJsonData<StoneSyncMessage>(args.BinaryUserData);
                ProcessStoneSync(syncMsg);
                break;
            case Protocol.Type.PlayerSync:
                PlayerSyncMessage playerMsg = DataParser.ReadJsonData<PlayerSyncMessage>(args.BinaryUserData);
                ProcessPlayerSync(playerMsg);
                break;
            case Protocol.Type.GameEnd:
                EndGame();
                break;
            default:
                Debug.Log("ã�� �� ���� Ÿ���Դϴ�.");
                return;
        }
    }

    private void ProcessStoneSync(StoneSyncMessage msg)
    {
        Stone stone = FindStoneById(msg.stoneId);
        if (stone != null)
        {
            Rigidbody rb = stone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���� ���� ����ȭ
                rb.position = new Vector3(msg.posX, msg.posY, msg.posZ);
                rb.rotation = new Quaternion(msg.rotX, msg.rotY, msg.rotZ, msg.rotW);
                rb.velocity = new Vector3(msg.velX, msg.velY, msg.velZ);
                rb.angularVelocity = new Vector3(msg.angVelX, msg.angVelY, msg.angVelZ);

                // �پ��ִ� ���� ó��
                foreach (string attachedId in msg.attachedStoneIds)
                {
                    Stone attachedStone = FindStoneById(attachedId);
                    if (attachedStone != null && !stone.IsConnectedTo(attachedStone.gameObject))
                    {
                        stone.AttachObject(attachedStone.gameObject);
                    }
                }
            }
        }
    }

    private void ProcessPlayerSync(PlayerSyncMessage msg)
    {
        foreach (var pair in msg.playerStones)
        {
            Player player = (Player)Enum.Parse(typeof(Player), pair.Key);
            playerManager.stoneCount[player] = pair.Value;
        }
    }

    private Stone FindStoneById(string id)
    {
        // Scene���� ��� Stone ������Ʈ�� ã�� ID�� ��Ī
        Stone[] allStones = FindObjectsOfType<Stone>();
        return allStones.FirstOrDefault(s => s.stoneId == id);
    }

    private void Update()
    {
        if(CurrentPlayerState == PlayerState.PlayTime)
        {
            OnGame?.Invoke();
        }
        else
        {
            return;
        }
    }
}