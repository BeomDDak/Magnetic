using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class GameManager : Singleton<GameManager>
{
    // ���� 
    public GameState CurrentState;
    public Player CurrentPlayer;
    public PlayerState CurrentPlayerState;

    // �ٸ� ��ũ��Ʈ ����
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    public Landing landing;

    // �׼�
    public Action<GameState> OnGameStateChanged;      // ���� ���� ����
    public Action<Player> OnTurnChanged;              // �� ����
    public Action OnStartGame;
    public Action OnGame;

    
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
