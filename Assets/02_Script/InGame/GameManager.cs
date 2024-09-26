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
    // ���� 
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }

    // �ٸ� ��ũ��Ʈ ����
    public StoneManager stoneManager { get; private set; }
    private PlayerManager playerManager;
    private Landing landing;
    private Magnet magnet;

    // �׼�
    public Action<GameState> OnGameStateChanged;           // ���� ���� ����
    public Action OnTurnChanged;                           // �� ����
    public Action OnGameStarted;
    public Action OnGameEnded;

    // �� ����
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

        // ������ ����
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
        // �κ�� ������ �����ߴ�
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
