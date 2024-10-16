using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using System.Linq;

public partial class BackendMatchManager : MonoBehaviour
{
    public class MatchInfo
    {
        public string title;                // ��Ī ��
        public string inDate;               // ��Ī inDate (UUID)
        public MatchType matchType;         // ��ġ Ÿ��
        public MatchModeType matchModeType; // ��ġ ��� Ÿ��
        public string headCount;            // ��Ī �ο�
        public bool isSandBoxEnable;        // ����ڽ� ��� (AI��Ī)
    }

    public class ServerInfo
    {
        public string host;
        public ushort port;
        public string roomToken;
    }

    public class MatchRecord
    {
        public MatchType matchType;
        public MatchModeType modeType;
        public string matchTitle;
        public string score = "-";
        public int win = -1;
        public int numOfMatch = 0;
        public double winRate = 0;
    }

    private static BackendMatchManager instance = null; // �ν��Ͻ�

    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();  // �ֿܼ��� ������ ��Ī ī����� ����Ʈ

    public List<SessionId> sessionIdList { get; private set; }              // ��ġ�� �������� �������� ���� ���
    // public Dictionary<SessionId, int> teamInfo { get; private set; }     // ��ġ�� �������� �������� �� ���� (MatchModeType�� team�� ��쿡�� ���)
    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // ��ġ�� �������� �������� ��Ī ���
    private string inGameRoomToken = string.Empty;  // ���� �� ��ū (�ΰ��� ���� ��ū)
    public SessionId hostSession { get; private set; }  // ȣ��Ʈ ����
    private ServerInfo roomInfo = null;             // ���� �� ����
    public bool isReconnectEnable { get; private set; } = false;

    public bool isConnectMatchServer { get; private set; } = false;
    private bool isConnectInGameServer = false;
    private bool isJoinGameRoom = false;
    public bool isReconnectProcess { get; private set; } = false;
    public bool isSandBoxGame { get; private set; } = false;

    private int numOfClient = 2;                    // ��ġ�� ������ ������ �� ��

    public MatchType nowMatchType { get; private set; } = MatchType.None;     // ���� ���õ� ��ġ Ÿ��
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None; // ���� ���õ� ��ġ ��� Ÿ��

    // ����� �α�
    private string NOTCONNECT_MATCHSERVER = "��ġ ������ ����Ǿ� ���� �ʽ��ϴ�.";
    private string RECONNECT_MATCHSERVER = "��ġ ������ ������ �õ��մϴ�.";
    private string FAIL_CONNECT_MATCHSERVER = "��ġ ���� ���� ���� : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "��ġ ���� ���� ����";
    private string SUCCESS_MATCHMAKE = "��Ī ���� : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "��Ī ��⿭�� ��ϵǾ����ϴ�.";
    private string FAIL_REGIST_MATCHMAKE = "��Ī ���� : {0}";
    private string CANCEL_MATCHMAKE = "��Ī ��û ��� : {0}";
    private string INVAILD_MATCHTYPE = "�߸��� ��ġ Ÿ���Դϴ�.";
    private string INVALID_MODETYPE = "�߸��� ��� Ÿ���Դϴ�.";
    private string INVALID_OPERATION = "�߸��� ��û�Դϴ�\n{0}";
    private string EXCEPTION_OCCUR = "���� �߻� : {0}\n�ٽ� ��Ī�� �õ��մϴ�.";
    private string FAIL_ACCESS_INGAME = "�ΰ��� ���� ���� : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "���� �ΰ��� ���� ���� : {0}";
    private string NUM_INGAME_SESSION = "�ΰ��� �� ���� ���� : {0}";

    // ��ġ ���� ��������
    public MatchInfo GetMatchInfo(string indate)
    {
        var result = matchInfos.FirstOrDefault(x => x.inDate == indate);
        if (result.Equals(default(MatchInfo)) == true)
        {
            return null;
        }
        return result;
    }

    void Update()
    {
        if (isConnectInGameServer || isConnectMatchServer)
        {
            Backend.Match.Poll();
        }
    }
}
