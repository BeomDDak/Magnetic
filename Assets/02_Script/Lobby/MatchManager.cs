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
        // ���� 3. ��ġ���� ���� Ȯ��
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            // ���� 3-1. Ȯ�� �� �Լ� ȣ��
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        // ���� 5. ��ġ �� ����� Ȯ��
        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
            // ���� 5-1. ��ġ ��û �Լ� ȣ��
            RequestMatchMaking();
        };

        // ���� 7. ��ġ ���� Ȯ��
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // ���� 7-1. ��Ī ��û ���� �۾��� ���� ȣ��
            ProcessMatchMakingResponse(args);
        };

        // ���� 10. �ΰ��� ���� ���� Ȯ��
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Debug.Log("OnSessionJoinInServer : " + args.ErrInfo);
            // �ΰ��� ������ �����ϸ� ȣ��
            if (args.ErrInfo != ErrorInfo.Success)
            {
                if (isReconnectProcess)
                {
                    if (args.ErrInfo.Reason.Equals("Fail To Reconnect"))
                    {
                        Debug.Log("������ ����");
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
                Debug.LogError("�ΰ��� ���� ���� ���������� �� ��ū�� �����ϴ�.");
                return;
            }

            Debug.Log("�ΰ��� ���� ���� ����");
            isJoinGameRoom = true;
            // ���� 10-1. �ΰ��� ���� ���� ������, �ΰ��� �� �Լ� ȣ��
            AccessInGameRoom(inGameRoomToken);
        };

        // ���� 12. �ΰ��� �� �����, ������ Ȯ��
        Backend.Match.OnSessionListInServer += (args) =>
        {
            // ���� ����Ʈ ȣ�� �� ���� ä���� ȣ���
            // ���� ���� ����(��)�� �������� �÷��̾�� �� ������ ���� �� �濡 ���� �ִ� �÷��̾��� ���� ������ ����ִ�.
            // ������ �ʰ� ���� �÷��̾���� ������ OnMatchInGameAccess ���� ���ŵ�
            Debug.Log("OnSessionListInServer : " + args.ErrInfo);

            // ���� 12-1. �ΰ��� ������ Ȯ���ϴ� �Լ� ȣ��
            ProcessMatchInGameSessionList(args);
        };

        Backend.Match.OnMatchInGameAccess += (args) =>
        {
            Debug.Log("OnMatchInGameAccess : " + args.ErrInfo);
            // ������ �ΰ��� �뿡 ������ ������ ȣ�� (�� Ŭ���̾�Ʈ�� �ΰ��� �뿡 ������ ������ ȣ���)
            ProcessMatchInGameAccess(args);
        };

        // ���� 14. ��ġ ����
        Backend.Match.OnMatchInGameStart += () =>
        {
            // �������� ���� ���� ��Ŷ�� ������ ȣ��
            SceneLoader.Instance.LoadScene(SceneType.InGame);
        };

    }

    // ��ư�� ����
    public void StartMatchmakingProcess()
    {
        // ���� 1. ��ġ����Ʈ�� ������ ��������
        Backend.Match.GetMatchList();
        Debug.Log(Backend.Match.GetMatchList());
        Debug.Log("��ġ ����Ʈ �ҷ����� ����");
        // ���� 2. ��ġ ����Ʈ�� �ҷ����� ��ġ����ŷ ���� ���� ��û
        JoinMatchServer();
    }

    private void JoinMatchServer()
    {
        // �̹� �������̶�� ����
        if (isConnectMatchServer)
        {
            return;
        }

        ErrorInfo errorInfo;
        isConnectMatchServer = true;

        // 2-1. ��ġ ������ ���� -> �������� Ȯ�� �޾ƾߵ�.
        if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            Debug.Log("��ġ����ŷ ���� ���� ��û ���� : " + errorInfo);
        }

        Debug.Log("��ġ����ŷ ���� ���� ��û ����");
    }

    // ��Ī ���� ���ӿ� ���� ���ϰ�
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // ���� ����
            isConnectMatchServer = false;
            Debug.Log(errInfo);
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format("��ġ ���� ���� ���� : {0}", errInfo.ToString());
            // ���� ����
            Debug.Log(errorLog);
        }
        else
        {
            //���� ����
            Debug.Log("��ġ ���� ���� ����");
            // 4. ��ġ ������ ���� �����ߴٸ� ��ġ �� ����� �Լ� ȣ��
            CreateMatchRoom();
        }
    }

    private bool CreateMatchRoom()
    {
        // ��Ī ������ ����Ǿ� ���� ������ ��Ī ���� ����
        if (!isConnectMatchServer)
        {
            Debug.Log("��ġ ������ ����Ǿ� ���� �ʽ��ϴ�.");
            Debug.Log("��ġ ������ ������ �õ��մϴ�.");
            JoinMatchServer();
            return false;
        }
        Debug.Log("�� ���� ��û�� ������ ����");
        // 4-1. ��ġ �� ����� -> ������ Ȯ�� �޾ƾߵ�.
        Backend.Match.CreateMatchRoom();
        return true;
    }

    public void RequestMatchMaking()
    {
        // ��û ������ ����Ǿ� ���� ������ ��Ī ���� ����
        if (!isConnectMatchServer)
        {
            Debug.Log("��ġ ������ ����Ǿ� ���� �ʽ��ϴ�.");
            Debug.Log("��ġ ������ ������ �õ��մϴ�.");
            JoinMatchServer();
            return;
        }
        // ���� �ʱ�ȭ
        isConnectInGameServer = false;

        // ���� 6. 1������ ���� ��ġ ����Ʈ�� �ϳ��� ������ �̿��Ͽ� ��ġ ��û -> ������ Ȯ�� �޾ƾߵ�.
        Backend.Match.RequestMatchMaking(MatchType.Random, MatchModeType.OneOnOne, "2024-10-16T04:53:20.101Z");
        if (isConnectInGameServer)
        {
            Backend.Match.LeaveGameServer(); //�ΰ��� ���� ���ӵǾ� ���� ��츦 ����� �ΰ��� ���� ���� ȣ��
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
                // ��Ī �������� ��
                debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                // LobbyUI.GetInstance().MatchDoneCallback();

                // ���� 8. ��Ī�� ���� �ϸ� ȣ���ϴ� �Լ�
                ProcessMatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                // ��Ī ��û �������� �� or ��Ī ���� �� ��Ī ��û�� �õ����� ��

                // ��Ī ��û �������� ��
                if (args.Reason == string.Empty)
                {
                    debugLog = SUCCESS_REGIST_MATCHMAKE;
                    // LobbyUI.GetInstance().MatchRequestCallback(true);
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled:
                // ��Ī ��û�� ��ҵǾ��� ��
                debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // ��ġ Ÿ���� �߸� �������� ��
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // ��ġ ��带 �߸� �������� ��
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.InvalidOperation:
                isError = true;
                // �߸��� ��û�� �������� ��
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // �߸��� ��û�� �������� ��
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                // LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Exception:
                isError = true;
                // ��Ī �ǰ�, �������� �� ������ �� ���� �߻� �� exception�� ���ϵ�
                // �� ��� �ٽ� ��Ī ��û�ؾ� ��
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

    // ��Ī �������� ��
    // �ΰ��� ������ �����ؾ� �Ѵ�.
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        if (sessionIdList != null)
        {
            Debug.Log("���� ���� ���� ����");
            sessionIdList.Clear();
        }

        // ���� 9. �ΰ��� ���� ���� -> ������ Ȯ�� �޾ƾߵ�.
        if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
        {
            var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
            Debug.Log(debugLog);
        }
        // ���ڰ����� �ΰ��� ����ū�� �����صξ�� �Ѵ�.
        // �ΰ��� �������� �뿡 ������ �� �ʿ�
        // 1�� ���� ��� ������ �뿡 �������� ������ �ش� ���� �ı�ȴ�.
        isConnectInGameServer = true;
        isJoinGameRoom = false;
        isReconnectProcess = false;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
        isSandBoxGame = args.RoomInfo.m_enableSandbox;
        var info = GetMatchInfo(args.MatchCardIndate);
        if (info == null)
        {
            Debug.LogError("��ġ ������ �ҷ����� �� �����߽��ϴ�.");
            return;
        }

        // nowMatchType = MatchType.Random;
        // nowModeType = MatchModeType.OneOnOne;
        // numOfClient = int.Parse(info.headCount);
    }

    public void ProcessReconnect()
    {
        Debug.Log("������ ���μ��� ����");
        if (roomInfo == null)
        {
            Debug.LogError("������ �� �� ������ �������� �ʽ��ϴ�.");
            return;
        }
        ErrorInfo errorInfo;

        if (sessionIdList != null)
        {
            Debug.Log("���� ���� ���� ���� : " + sessionIdList.Count);
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

    // �ΰ��� �� ����
    private void AccessInGameRoom(string roomToken)
    {
        // ���� 11. �ΰ��� �� ���� -> ������ Ȯ�� �޾ƾߵ�
        Backend.Match.JoinGameRoom(roomToken);
    }

    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        // ���� 13. ���� �뿡 ������ ���ǵ��� ����
        // ���� �뿡 �������� �� 1ȸ ���ŵ�
        // ������ ���� ���� 1ȸ ���ŵ� -> ��ġ ���� ������ Ȯ�� �޾ƾߵ�.
        sessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach (var record in args.GameRecords)
        {
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        sessionIdList.Sort();
    }

    // Ŭ���̾�Ʈ ���� ���� �� ���ӿ� ���� ���ϰ�
    // Ŭ���̾�Ʈ�� ���� �뿡 ������ ������ ȣ���
    // ������ ���� ���� ���ŵ��� ����
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if (isReconnectProcess)
        {
            // ������ ���μ��� �� ���
            // �� �޽����� ���ŵ��� �ʰ�, ���� ���ŵǾ ������
            Debug.Log("������ ���μ��� ������... ������ ���μ��������� ProcessMatchInGameAccess �޽����� ���ŵ��� �ʽ��ϴ�.\n" + args.ErrInfo);
            return;
        }

        Debug.Log(string.Format(SUCCESS_ACCESS_INGAME, args.ErrInfo));

        if (args.ErrInfo != ErrorCode.Success)
        {
            // ���� �� ���� ����
            var errorLog = string.Format(FAIL_ACCESS_INGAME, args.ErrInfo, args.Reason);
            Debug.Log(errorLog);
            LeaveInGameRoom();
            return;
        }

        // ���� �� ���� ����
        // ���ڰ��� ��� ������ Ŭ���̾�Ʈ(����)�� ����ID�� ��Ī ����� ����ִ�.
        // ���� ������ �����Ǿ� ����ֱ� ������ �̹� ������ �����̸� �ǳʶڴ�.

        var record = args.GameRecord;
        Debug.Log(string.Format(string.Format("�ΰ��� ���� ���� ���� [{0}] : {1}", args.GameRecord.m_sessionId, args.GameRecord.m_nickname)));
        if (!sessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            // ���� ����, ���� ��� ���� ����
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);

            Debug.Log(string.Format(NUM_INGAME_SESSION, sessionIdList.Count));
        }
    }

    // �ΰ��� ���� ���� ����
    public void LeaveInGameRoom()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
    }

    // ��Ī ��� �� ������
    public void LeaveMatchLoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
}
