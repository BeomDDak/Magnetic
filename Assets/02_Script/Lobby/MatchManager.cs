using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public partial class BackendMatchManager : MonoBehaviour
{
    private string gameRoomToken = string.Empty;
    private List<MatchInGameRoomInfo> matchList;

    private void Start()
    {
        MatchMakingHandler();
    }

    private void MatchMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            // 매칭 서버에 접속하면 호출
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
        };

        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // 매칭 신청 관련 작업에 대한 호출
            ProcessMatchMakingResponse(args);
        };
    }

    public void StartMatchmakingProcess()
    {
        // 1. GetMatchList 매치 리스트 불러오기
        /*var callback = Backend.Match.GetMatchList();

        Backend.Match.GetMatchList();
        if (!callback.IsSuccess())
        {
            Debug.LogError("Backend.Match.GetMatchList Error : " + callback);
            return;
        }

        Debug.Log("매치 리스트 불러오기 성공");*/
        JoinMatchServer();
    }

    private void JoinMatchServer()
    {
        // 이미 연결중이라면 리턴
        if (isConnectMatchServer)
        {
            return;
        }

        ErrorInfo errorInfo;
        isConnectMatchServer = true;

        if(!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            Debug.Log("매치메이킹 서버 접속 요청 실패 : " + errorInfo);
        }

        Debug.Log("매치메이킹 서버 접속 요청 성공");
    }

    private bool CreateMatchRoom()
    {
        // 매칭 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log("매치 서버에 연결되어 있지 않습니다.");
            Debug.Log("매치 서버에 접속을 시도합니다.");
            JoinMatchServer();
            return false;
        }
        Debug.Log("방 생성 요청을 서버로 보냄");
        Backend.Match.CreateMatchRoom();
        return true;
    }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format("매치 서버 접속 실패 : {0}", errInfo.ToString());
            // 접속 실패
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            Debug.Log("매치 서버 접속 성공");
            CreateMatchRoom();
        }
    }

    public void RequestMatchMaking(int index)
    {
        // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log("매치 서버에 연결되어 있지 않습니다.");
            Debug.Log("매치 서버에 접속을 시도합니다.");
            JoinMatchServer();
            return;
        }
        // 변수 초기화
        isConnectInGameServer = false;

        Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
        if (isConnectInGameServer)
        {
            Backend.Match.LeaveGameServer(); //인게임 서버 접속되어 있을 경우를 대비해 인게임 서버 리브 호출
        }

        //nowMatchType = matchInfos[index].matchType;
        //nowModeType = matchInfos[index].matchModeType;
        //numOfClient = int.Parse(matchInfos[index].headCount);
    }

    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        string debugLog = string.Empty;
        bool isError = false;
        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                // 매칭 성공했을 때
                debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                // LobbyUI.GetInstance().MatchDoneCallback();
                ProcessMatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때

                // 매칭 신청 성공했을 때
                if (args.Reason == string.Empty)
                {
                    debugLog = SUCCESS_REGIST_MATCHMAKE;

                    // LobbyUI.GetInstance().MatchRequestCallback(true);
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled:
                // 매칭 신청이 취소되었을 때
                debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // 매치 타입을 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // 매치 모드를 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.InvalidOperation:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Exception:
                isError = true;
                // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                // 이 경우 다시 매칭 신청해야 됨
                debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);

                // LobbyUI.GetInstance().RequestMatch();
                break;
        }

        if (!debugLog.Equals(string.Empty))
        {
            Debug.Log(debugLog);
            if (isError == true)
            {
                // LobbyUI.GetInstance().SetErrorObject(debugLog);
            }
        }
    }

    // 매칭 성공했을 때
    // 인게임 서버로 접속해야 한다.
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        if (sessionIdList != null)
        {
            Debug.Log("이전 세션 저장 정보");
            sessionIdList.Clear();
        }

        if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
        {
            var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
            Debug.Log(debugLog);
        }
        // 인자값에서 인게임 룸토큰을 저장해두어야 한다.
        // 인게임 서버에서 룸에 접속할 때 필요
        // 1분 내에 모든 유저가 룸에 접속하지 않으면 해당 룸은 파기된다.
        isConnectInGameServer = true;
        isJoinGameRoom = false;
        isReconnectProcess = false;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
        isSandBoxGame = args.RoomInfo.m_enableSandbox;
        var info = GetMatchInfo(args.MatchCardIndate);
        if (info == null)
        {
            Debug.LogError("매치 정보를 불러오는 데 실패했습니다.");
            return;
        }

        // nowMatchType = MatchType.Random;
        // nowModeType = MatchModeType.OneOnOne;
        // numOfClient = int.Parse(info.headCount);
    }

    public void ProcessReconnect()
    {
        Debug.Log("재접속 프로세스 진입");
        if (roomInfo == null)
        {
            Debug.LogError("재접속 할 룸 정보가 존재하지 않습니다.");
            return;
        }
        ErrorInfo errorInfo;

        if (sessionIdList != null)
        {
            Debug.Log("이전 세션 저장 정보 : " + sessionIdList.Count);
            sessionIdList.Clear();
        }

        if (!Backend.Match.JoinGameServer(roomInfo.host, roomInfo.port, true, out errorInfo))
        {
            var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
            Debug.Log(debugLog);
        }

        isConnectInGameServer = true;
        isJoinGameRoom = false;
        isReconnectProcess = true;
    }

    // 매칭 대기 방 나가기
    public void LeaveMatchLoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
}
