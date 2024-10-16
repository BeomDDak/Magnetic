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
            // ��Ī ������ �����ϸ� ȣ��
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
        };

        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            // ��Ī ��û ���� �۾��� ���� ȣ��
            ProcessMatchMakingResponse(args);
        };
    }

    public void StartMatchmakingProcess()
    {
        // 1. GetMatchList ��ġ ����Ʈ �ҷ�����
        /*var callback = Backend.Match.GetMatchList();

        Backend.Match.GetMatchList();
        if (!callback.IsSuccess())
        {
            Debug.LogError("Backend.Match.GetMatchList Error : " + callback);
            return;
        }

        Debug.Log("��ġ ����Ʈ �ҷ����� ����");*/
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

        if(!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            Debug.Log("��ġ����ŷ ���� ���� ��û ���� : " + errorInfo);
        }

        Debug.Log("��ġ����ŷ ���� ���� ��û ����");
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
        Backend.Match.CreateMatchRoom();
        return true;
    }

    // ��Ī ���� ���ӿ� ���� ���ϰ�
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // ���� ����
            isConnectMatchServer = false;
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
            CreateMatchRoom();
        }
    }

    public void RequestMatchMaking(int index)
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

        Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
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

    // ��Ī ��� �� ������
    public void LeaveMatchLoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
}
