using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing 
{
    /*
    private StoneManager stoneManager;
    private PlayerManager playerManager;
    private Camera mainCamera;

    public void Initialize(StoneManager stoneManager, PlayerManager playerManager)
    {
        this.stoneManager = stoneManager;
        this.playerManager = playerManager;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 position = hit.point;
            Player currentPlayer = GameManager.Instance.CurrentPlayer;
            int stoneType = playerManager.GetStoneCount(currentPlayer) % 2; // 예시 로직

            stoneManager.CreateStone(stoneType, position);
            playerManager.DecrementStoneCount(currentPlayer);
            GameManager.Instance.SwitchTurn();
        }
    }
    */
}