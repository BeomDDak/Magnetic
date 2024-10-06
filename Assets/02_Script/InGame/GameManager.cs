using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class GameManager : Singleton<GameManager>
{
    // ���� 
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public PlayerState CurrentPlayerState { get; set; }

    // �ٸ� ��ũ��Ʈ ����
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    public Landing landing {get; private set;}

    // �׼�
    public Action<GameState> OnGameStateChanged;      // ���� ���� ����
    public Action<Player> OnTurnChanged;              // �� ����
    public Action OnStartGame;
    //
    protected override void Init()
    {
        base.Init();
        InitializeManagers();
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
        OnStartGame?.Invoke();
        SwitchTurn();
    }

    // ���� ���۽� ���� �۾�
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1�÷��̾� ���� �����ϱ� ���ؼ�
        CurrentPlayerState = PlayerState.PlayTime;
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
        CurrentPlayer = CurrentPlayer == Player.One ? Player.Two : Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        OnTurnChanged?.Invoke(CurrentPlayer);
    }
}
