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
    // 상태 
    public GameState CurrentState;
    public Player CurrentPlayer;
    public PlayerState CurrentPlayerState;

    // 다른 스크립트 연결
    private StoneManager stoneManager;
    public PlayerManager playerManager;
    public Landing landing;

    // 액션
    public Action<GameState> OnGameStateChanged;      // 게임 상태 변경
    public Action<Player> OnTurnChanged;              // 턴 변경
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

    // 매니저 초기설정
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

    // 게임 시작시 해줄 작업
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1플레이어 부터 시작하기 위해서
        CurrentPlayerState = PlayerState.PlayTime;
        GameStartMessage msg = new GameStartMessage();
        BackendMatchManager.Instance.SendDataToInGame(msg);

        Debug.Log("현재 차례 : " + CurrentPlayer);
    }
    
    // 게임 끝날시 해줄 작업
    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        // 여기에 로비로 나가는 기능 추가해야함
    }

    // 턴 변경
    public void SwitchTurn()
    {
        CurrentPlayer = (CurrentPlayer == Player.One) ? Player.Two : Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        Debug.Log("현재 차례 : " + CurrentPlayer);
        //OnTurnChanged?.Invoke(CurrentPlayer);
    }

    // 카메라
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
            Debug.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            return;
        }

        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }

        if (BackendMatchManager.Instance.players == null)
        {
            Debug.LogError("Players 정보가 존재하지 않습니다.");
            return;
        }

        // 각 게임 로직 수신할 정보 입력 후, 메세지로 넘겨주기
        switch (msg.type)
        {
            case Protocol.Type.GameStart:       // 게임 시작 ( 카메라 )
                GameStartMessage gameStart = DataParser.ReadJsonData<GameStartMessage>(args.BinaryUserData);
                StartGame();
                SetCameraPosition();
                break;
            case Protocol.Type.StonePlacement:      // 랜딩
                StonePlacementMessage stoneMsg = DataParser.ReadJsonData<StonePlacementMessage>(args.BinaryUserData);
                landing.ProcessStonePlacement(stoneMsg);
                break;
            case Protocol.Type.GameEnd:
                EndGame();
                break;
            default:
                Debug.Log("찾을 수 없는 타입입니다.");
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
