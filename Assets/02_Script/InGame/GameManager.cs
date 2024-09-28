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
    private Landing landing;
    private Magnet magnet;

    // �׼�
    public Action<GameState> OnGameStateChanged;      // ���� ���� ����
    public Action OnTurnChanged;                             // �� ����

    protected override void Init()
    {
        base.Init();
        InitializeManagers();
    }

    // �Ŵ��� �ʱ⼳��
    private void InitializeManagers()
    {
        playerManager = new PlayerManager();
        stoneManager = new StoneManager();
        magnet = new Magnet();
        landing = GetComponent<Landing>();
    }

    // ���� ���۽� ���� �۾�
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        OnGameStateChanged?.Invoke(CurrentState);
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
        OnTurnChanged?.Invoke();
    }
}
