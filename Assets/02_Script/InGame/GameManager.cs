using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;
using BackEnd;
using BackEnd.Tcp;
using Protocol;

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

    private void Start()
    {
        StartGame();
        
    }

    // ���� ���۽� ���� �۾�
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1�÷��̾� ���� �����ϱ� ���ؼ�
        CurrentPlayerState = PlayerState.PlayTime;
        GameStartMessage msg = new GameStartMessage();
        BackendMatchManager.Instance.SendDataToInGame(msg);

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
        //OnTurnChanged?.Invoke(CurrentPlayer);
    }

    // ī�޶�
    private void SetCameraPosition()
    {
        GameStartMessage msg = new GameStartMessage();

        if(BackendMatchManager.Instance.players.ContainsValue(Player.One))
        {
            cam.transform.parent = camPosition[0].transform;
        }
        else
        {
            cam.transform.parent = camPosition[1].transform;
        }

        cam.transform.position = Vector3.zero;
        cam.transform.rotation = Quaternion.identity;
        BackendMatchManager.Instance.SendDataToInGame<GameStartMessage>(msg);
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
                StartGame();
                SetCameraPosition();
                break;
            case Protocol.Type.StonePlacement:      // ����
                StonePlacementMessage stoneMsg = DataParser.ReadJsonData<StonePlacementMessage>(args.BinaryUserData);
                landing.ProcessStonePlacement(stoneMsg);
                break;
            case Protocol.Type.GameEnd:
                EndGame();
                break;
            default:
                Debug.Log("ã�� �� ���� Ÿ���Դϴ�.");
                return;
        }
    }

    private void Update()
    {
        if(CurrentState == GameState.GameOver) 
        {
            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }

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
