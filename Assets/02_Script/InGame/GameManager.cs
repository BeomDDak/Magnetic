using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
    Playing,
    GameOver,
}

public enum Player
{
    One,
    Two,
}

public class GameManager : Singleton<GameManager>
{
    /*
    // 상태 
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }

    // 다른 스크립트 연결
    public StoneManager stoneManager { get; private set; }
    private PlayerManager playerManager;
    private Landing landing;
    private Magnet magnet;

    // 액션
    public Action<GameState> OnGameStateChanged;           // 게임 상태 변경
    public Action OnTurnChanged;                           // 턴 변경
    public Action OnGameStarted;
    public Action OnGameEnded;

    // 돌 종류
    [SerializeField] 
    private GameObject[] stonePrefabs;

    protected override void Init()
    {
        base.Init();
        InitializeManagers();
    }

    private void InitializeManagers()
    {
        magnet = new Magnet();
        stoneManager = new StoneManager(stonePrefabs, magnet);
        playerManager = new PlayerManager();
        landing = GetComponent<Landing>();

        // 의존성 주입
        landing.Initialize(stoneManager, playerManager);
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;
        OnGameStarted?.Invoke();
        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        OnGameEnded?.Invoke();
        // 로비로 나가기 만들어야댐
    }

    public void SwitchTurn()
    {
        if (CurrentPlayer == Player.One)
        {
            CurrentPlayer = Player.Two;
        }
        else
        {
            CurrentPlayer = Player.One;
        }

        OnTurnChanged?.Invoke();
    }
    */
}
