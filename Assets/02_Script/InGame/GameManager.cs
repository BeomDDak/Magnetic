using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class GameManager : Singleton<GameManager>
{
    // 상태 
    public GameState CurrentState;
    public Player CurrentPlayer;
    public PlayerState CurrentPlayerState;

    // 다른 스크립트 연결
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    public Landing landing;

    // 액션
    public Action<GameState> OnGameStateChanged;      // 게임 상태 변경
    public Action<Player> OnTurnChanged;              // 턴 변경
    public Action OnStartGame;
    public Action OnGame;

    
    protected override void Init()
    {
        base.Init();
        InitializeManagers();
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
        Debug.Log("SwitchTurn called. Current player before switch: " + CurrentPlayer);
        CurrentPlayer = (CurrentPlayer == Player.One) ? Player.Two : Player.One;
        Debug.Log("Turn switched. New current player: " + CurrentPlayer);
        CurrentPlayerState = PlayerState.PlayTime;
        //OnTurnChanged?.Invoke(CurrentPlayer);
    }

    private void Update()
    {
        Debug.Log(CurrentPlayer);
        Debug.Log(CurrentPlayerState);
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
