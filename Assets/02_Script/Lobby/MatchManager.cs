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
        // 순서 3. 매치서버 접속 확인
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            // 순서 3-1. 확인 후 함수 호출
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        // 순서 5. 매치 룸 만들기 확인
        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
            // 순서 5-1. 매치 요청 함수 호출
            RequestMatchMaking();
        };

        // 순서 7. 매치 접속 확인
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // 순서 7-1. 매칭 신청 관련 작업에 대한 호출
            ProcessMatchMakingResponse(args);
        };

        // 순서 10. 인게임 서버 접속 확인
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Debug.Log("OnSessionJoinInServer : " + args.ErrInfo);
            // 인게임 서버에 접속하면 호출
            if (args.ErrInfo != ErrorInfo.Success)
            {
                if (isReconnectProcess)
                {
                    if (args.ErrInfo.Reason.Equals("Fail To Reconnect"))
                    {
                        Debug.Log("재접속 실패");
                        JoinMatchServer();
                        isConnectInGameServer = false;
                    }
                }
                return;
            }

            if (isJoinGameRoom)
            {
                return;
            }

            if (inGameRoomToken == string.Empty)
            {
                Debug.LogError("인게임 서버 접속 성공했으나 룸 토큰이 없습니다.");
                return;
            }

            Debug.Log("인게임 서버 접속 성공");
            isJoinGameRoom = true;
            // 순서 10-1. 인게임 서버 접속 성공시, 인게임 룸 함수 호출
            AccessInGameRoom(inGameRoomToken);
        };

        // 순서 12. 인게임 룸 입장시, 접속자 확인
        Backend.Match.OnSessionListInServer += (args) =>
        {
            // 세션 리스트 호출 후 조인 채널이 호출됨
            // 현재 같은 게임(방)에 참가중인 플레이어들 중 나보다 먼저 이 방에 들어와 있는 플레이어들과 나의 정보가 들어있다.
            // 나보다 늦게 들어온 플레이어들의 정보는 OnMatchInGameAccess 에서 수신됨
            Debug.Log("OnSessionListInServer : " + args.ErrInfo);

            // 순서 12-1. 인게임 접속자 확인하는 함수 호출
            ProcessMatchInGameSessionList(args);
        };

        Backend.Match.OnMatchInGameAccess += (args) =>
        {
            Debug.Log("OnMatchInGameAccess : " + args.ErrInfo);
            // 세션이 인게임 룸에 접속할 때마다 호출 (각 클라이언트가 인게임 룸에 접속할 때마다 호출됨)
            ProcessMatchInGameAccess(args);
        };

        // 순서 14. 매치 시작
        Backend.Match.OnMatchInGameStart += () =>
        {
            // 서버에서 게임 시작 패킷을 보내면 호출
            SceneLoader.Instance.LoadScene(SceneType.InGame);
        };

    }

    // 버튼에 연결
    public void StartMatchmakingProcess()
    {
        // 순서 1. 매치리스트의 정보를 가져오기
        Backend.Match.GetMatchList();
        Debug.Log(Backend.Match.GetMatchList());
        Debug.Log("매치 리스트 불러오기 성공");
        // 순서 2. 매치 리스트를 불러오면 매치메이킹 서버 접속 요청
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

        // 2-1. 매치 서버에 접속 -> 서버에서 확인 받아야됨.
        if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            Debug.Log("매치메이킹 서버 접속 요청 실패 : " + errorInfo);
        }

        Debug.Log("매치메이킹 서버 접속 요청 성공");
    }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
            Debug.Log(errInfo);
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
            // 4. 매치 서버에 접속 성공했다면 매치 룸 만드는 함수 호출
            CreateMatchRoom();
        }
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
        // 4-1. 매치 룸 만들기 -> 서버에 확인 받아야됨.
        Backend.Match.CreateMatchRoom();
        return true;
    }

    public void RequestMatchMaking()
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

        // 순서 6. 1번에서 받은 매치 리스트중 하나의 정보를 이용하여 매치 요청 -> 서버에 확인 받아야됨.
        Backend.Match.RequestMatchMaking(MatchType.Random, MatchModeType.OneOnOne, "2024-10-16T04:53:20.101Z");
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

                // 순서 8. 매칭이 성공 하면 호출하는 함수
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

        // 순서 9. 인게임 서버 접속 -> 서버에 확인 받아야됨.
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

    // 인게임 룸 접속
    private void AccessInGameRoom(string roomToken)
    {
        // 순서 11. 인게임 룸 입장 -> 서버에 확인 받아야됨
        Backend.Match.JoinGameRoom(roomToken);
    }

    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        // 순서 13. 현재 룸에 접속한 세션들의 정보
        // 최초 룸에 접속했을 때 1회 수신됨
        // 재접속 했을 때도 1회 수신됨 -> 매치 시작 서버에 확인 받아야됨.
        sessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach (var record in args.GameRecords)
        {
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        sessionIdList.Sort();
    }

    // 클라이언트 들의 게임 룸 접속에 대한 리턴값
    // 클라이언트가 게임 룸에 접속할 때마다 호출됨
    // 재접속 했을 때는 수신되지 않음
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if (isReconnectProcess)
        {
            // 재접속 프로세스 인 경우
            // 이 메시지는 수신되지 않고, 만약 수신되어도 무시함
            Debug.Log("재접속 프로세스 진행중... 재접속 프로세스에서는 ProcessMatchInGameAccess 메시지는 수신되지 않습니다.\n" + args.ErrInfo);
            return;
        }

        Debug.Log(string.Format(SUCCESS_ACCESS_INGAME, args.ErrInfo));

        if (args.ErrInfo != ErrorCode.Success)
        {
            // 게임 룸 접속 실패
            var errorLog = string.Format(FAIL_ACCESS_INGAME, args.ErrInfo, args.Reason);
            Debug.Log(errorLog);
            LeaveInGameRoom();
            return;
        }

        // 게임 룸 접속 성공
        // 인자값에 방금 접속한 클라이언트(세션)의 세션ID와 매칭 기록이 들어있다.
        // 세션 정보는 누적되어 들어있기 때문에 이미 저장한 세션이면 건너뛴다.

        var record = args.GameRecord;
        Debug.Log(string.Format(string.Format("인게임 접속 유저 정보 [{0}] : {1}", args.GameRecord.m_sessionId, args.GameRecord.m_nickname)));
        if (!sessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            // 세션 정보, 게임 기록 등을 저장
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);

            Debug.Log(string.Format(NUM_INGAME_SESSION, sessionIdList.Count));
        }
    }

    // 인게임 서버 접속 종료
    public void LeaveInGameRoom()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
    }

    // 매칭 대기 방 나가기
    public void LeaveMatchLoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
}
