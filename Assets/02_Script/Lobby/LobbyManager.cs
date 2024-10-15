using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public class LobbyManager : MonoBehaviour
{
    public void OnClickQuickGame()
    {
        Backend.Match.JoinGameRoom(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("���� �� ���� ����");
                // �濡 ���������Ƿ� ���� ���� �غ�
                PrepareGame();
            }
            else
            {
                Debug.Log("�� ���� ����. �� �� ����");
                CreateNewRoom();
            }
        });
    }

    private void CreateNewRoom()
    {
        Backend.Match.CreateGameRoom(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("�� �� ���� ����");
                // ���� ���������Ƿ� ���� ��ٸ���
                WaitForOpponent();
            }
            else
            {
                Debug.LogError("�� ���� ����: " + callback.GetMessage());
            }
        });
    }

    private void WaitForOpponent()
    {
        Debug.Log("������ ��ٸ��� ��...");
        // ���⼭ ������ ������ �̺�Ʈ�� ó��
        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
        {
            if (args.GameRecords.Count == 2) // 1:1 ��ġ�̹Ƿ� 2���� �Ǹ� ����
            {
                Debug.Log("���� ����. ���� ����!");
                PrepareGame();
            }
        };
    }

    private void PrepareGame()
    {
        // ���� ���� �� �ʿ��� �غ� �۾�
        Debug.Log("���� �غ� ��...");
        // ��: ���� �� �ε�, �ʱ� ���� ���� ���� ��
        // SceneManager.LoadScene("GameScene");
    }

    private void OnEnable()
    {
        // ���� ���� ����
        Backend.Match.JoinGameServer(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("���� ���� ���� ����");
            }
            else
            {
                Debug.LogError("���� ���� ���� ����: " + callback.GetMessage());
            }
        });
    }

    private void OnDisable()
    {
        // ���� �������� ������
        Backend.Match.LeaveGameServer();
    }
}
