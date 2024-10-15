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
                Debug.Log("기존 방 참가 성공");
                // 방에 참가했으므로 게임 시작 준비
                PrepareGame();
            }
            else
            {
                Debug.Log("빈 방이 없음. 새 방 생성");
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
                Debug.Log("새 방 생성 성공");
                // 방을 생성했으므로 상대방 기다리기
                WaitForOpponent();
            }
            else
            {
                Debug.LogError("방 생성 실패: " + callback.GetMessage());
            }
        });
    }

    private void WaitForOpponent()
    {
        Debug.Log("상대방을 기다리는 중...");
        // 여기서 상대방이 들어오는 이벤트를 처리
        Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
        {
            if (args.GameRecords.Count == 2) // 1:1 매치이므로 2명이 되면 시작
            {
                Debug.Log("상대방 입장. 게임 시작!");
                PrepareGame();
            }
        };
    }

    private void PrepareGame()
    {
        // 게임 시작 전 필요한 준비 작업
        Debug.Log("게임 준비 중...");
        // 예: 게임 씬 로드, 초기 게임 상태 설정 등
        // SceneManager.LoadScene("GameScene");
    }

    private void OnEnable()
    {
        // 게임 서버 접속
        Backend.Match.JoinGameServer(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("게임 서버 접속 성공");
            }
            else
            {
                Debug.LogError("게임 서버 접속 실패: " + callback.GetMessage());
            }
        });
    }

    private void OnDisable()
    {
        // 게임 서버에서 나가기
        Backend.Match.LeaveGameServer();
    }
}
