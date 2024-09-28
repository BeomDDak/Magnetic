using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class GameManager : Singleton<GameManager>
{
    // 상태 
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public PlayerState CurrentPlayerState { get; set; }

    // 다른 스크립트 연결
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    private Landing landing;
    private Magnet magnet;

    // 액션
    public Action<GameState> OnGameStateChanged;      // 게임 상태 변경
    public Action OnTurnChanged;                             // 턴 변경

    protected override void Init()
    {
        base.Init();
        InitializeManagers();
    }

    // 매니저 초기설정
    private void InitializeManagers()
    {
        playerManager = new PlayerManager();
        stoneManager = new StoneManager();
        magnet = new Magnet();
        landing = GetComponent<Landing>();
    }

    // 게임 시작시 해줄 작업
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        OnGameStateChanged?.Invoke(CurrentState);
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
        CurrentPlayer = CurrentPlayer == Player.One ? Player.Two : Player.One;
        OnTurnChanged?.Invoke();
    }
}
