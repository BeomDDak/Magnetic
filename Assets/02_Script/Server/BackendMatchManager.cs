using System;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using System.Linq;

public partial class BackendMatchManager : Singleton<BackendMatchManager>
{
    public class MatchInfo
    {
        public string title;                // 매칭 명
        public string inDate;               // 매칭 inDate (UUID)
        public MatchType matchType;         // 매치 타입
        public MatchModeType matchModeType; // 매치 모드 타입
        public string headCount;            // 매칭 인원
        public bool isSandBoxEnable;        // 샌드박스 모드 (AI매칭)
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

    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();  // 콘솔에서 생성한 매칭 카드들의 리스트

    public List<SessionId> sessionIdList { get; private set; }              // 매치에 참가중인 유저들의 세션 목록
    // public Dictionary<SessionId, int> teamInfo { get; private set; }     // 매치에 참가중인 유저들의 팀 정보 (MatchModeType이 team인 경우에만 사용)
    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // 매치에 참가중인 유저들의 매칭 기록
    private string inGameRoomToken = string.Empty;  // 게임 룸 토큰 (인게임 접속 토큰)
    public SessionId hostSession { get; private set; }  // 호스트 세션
    private ServerInfo roomInfo = null;             // 게임 룸 정보
    public bool isReconnectEnable { get; private set; } = false;

    public bool isConnectMatchServer { get; private set; } = false;
    private bool isConnectInGameServer = false;
    private bool isJoinGameRoom = false;
    public bool isReconnectProcess { get; private set; } = false;
    public bool isSandBoxGame { get; private set; } = false;

    public MatchType nowMatchType { get; private set; } = MatchType.None;     // 현재 선택된 매치 타입
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None; // 현재 선택된 매치 모드 타입

    private bool isHost = false;       // 호스트 여부 (서버에서 설정한 SuperGamer 정보를 가져옴)
    private bool isSetHost = false;    // 호스트 세션 결정했는지 여부

    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";
    private string FAIL_ACCESS_INGAME = "인게임 접속 실패 : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "유저 인게임 접속 성공 : {0}";
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";

    // 매치 정보 가져오기
    public MatchInfo GetMatchInfo(string indate)
    {
        var result = matchInfos.FirstOrDefault(x => x.inDate == indate);
        if (result.Equals(default(MatchInfo)) == true)
        {
            return null;
        }
        return result;
    }

    private bool SetHostSession()
    {
        // 호스트 세션 정하기
        // 각 클라이언트가 모두 수행 (호스트 세션 정하는 로직은 모두 같으므로 각각의 클라이언트가 모두 로직을 수행하지만 결과값은 같다.)

        Debug.Log("호스트 세션 설정 진입");
        // 호스트 세션 정렬 (각 클라이언트마다 입장 순서가 다를 수 있기 때문에 정렬)
        sessionIdList.Sort();
        isHost = false;
        // 내가 호스트 세션인지
        foreach (var record in gameRecords)
        {
            if (record.Value.m_isSuperGamer == true)
            {
                if (record.Value.m_sessionId.Equals(Backend.Match.GetMySessionId()))
                {
                    isHost = true;
                }
                hostSession = record.Value.m_sessionId;
                break;
            }
        }

        Debug.Log("호스트 여부 : " + isHost);

        // 호스트 설정까지 끝나면 매치서버와 접속 끊음
        LeaveMatchServer();
        return true;
    }

    void Update()
    {
        if (isConnectInGameServer || isConnectMatchServer)
        {
            Backend.Match.Poll();
        }
    }
}
