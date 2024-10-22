using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;
using BackEnd;
using BackEnd.Tcp;

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
        //CameraPosition();
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
        //OnTurnChanged?.Invoke(CurrentPlayer);
    }

    // 카메라
    private void CameraPosition()
    {
        var player = BackendMatchManager.Instance.sessionIdList[0];
        if(player == BackendMatchManager.Instance.sessionIdList[0])
        {
            cam.transform.parent = camPosition[0].transform;
        }
        else
        {
            cam.transform.parent = camPosition[1].transform;
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
