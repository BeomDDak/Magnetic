using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;
using BackEnd;
using BackEnd.Tcp;

public class GameManager : Singleton<GameManager>
{
    // ���� 
    public GameState CurrentState;
    public Player CurrentPlayer;
    public PlayerState CurrentPlayerState;

    // �ٸ� ��ũ��Ʈ ����
    private StoneManager stoneManager;
    public PlayerManager playerManager;
    public Landing landing;

    // �׼�
    public Action<GameState> OnGameStateChanged;      // ���� ���� ����
    public Action<Player> OnTurnChanged;              // �� ����
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
        //CameraPosition();
    }

    // ���� ���۽� ���� �۾�
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        CurrentPlayer = Player.One;         // 1�÷��̾� ���� �����ϱ� ���ؼ�
        CurrentPlayerState = PlayerState.PlayTime;
        Debug.Log("���� ���� : " + CurrentPlayer);
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
        CurrentPlayer = (CurrentPlayer == Player.One) ? Player.Two : Player.One;
        CurrentPlayerState = PlayerState.PlayTime;
        Debug.Log("���� ���� : " + CurrentPlayer);
        //OnTurnChanged?.Invoke(CurrentPlayer);
    }

    // ī�޶�
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
