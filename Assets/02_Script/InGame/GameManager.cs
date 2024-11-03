using UnityEngine;
using System;
using static Define;
using BackEnd.Tcp;
using Protocol;
using System.Linq;

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
        isDestoryOnLoad = true;
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

    // 게임 시작시 해줄 작업
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1플레이어 부터 시작하기 위해서
        CurrentPlayerState = PlayerState.PlayTime;
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
    }

    // 카메라
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
                SetCameraPosition();
                StartGame();
                break;
            case Protocol.Type.StonePlacement:      // 랜딩
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
                Debug.Log("찾을 수 없는 타입입니다.");
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
                // 물리 상태 동기화
                rb.position = new Vector3(msg.posX, msg.posY, msg.posZ);
                rb.rotation = new Quaternion(msg.rotX, msg.rotY, msg.rotZ, msg.rotW);
                rb.velocity = new Vector3(msg.velX, msg.velY, msg.velZ);
                rb.angularVelocity = new Vector3(msg.angVelX, msg.angVelY, msg.angVelZ);

                // 붙어있는 돌들 처리
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
        // Scene에서 모든 Stone 컴포넌트를 찾아 ID로 매칭
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