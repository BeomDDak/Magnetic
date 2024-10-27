using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Transform UICanvasTrs;  // UI 화면을 렌더링 할 캔버스 컴포넌트 트랜스폼
    public Transform ClosedUITrs;  // UI 화면을 닫을 때 비활성화 시킨 UI 화면들을 위치시켜줄 트랜스폼
    private BaseUI m_FrontUI;      // UI 화면이 열려있을 때 가장 상단에 열려있는 UI

    // 현재 열려있는, 즉 활성화 되어있는 UI 화면을 담고 있는 변수(풀)
    private Dictionary<System.Type, GameObject> m_OpenUIPool = new Dictionary<System.Type, GameObject>();

    // 닫혀있는, 즉 비활성화 되어있는 UI 화면을 담고 있는 변수(풀)
    private Dictionary<System.Type, GameObject> m_ClosedUIPool = new Dictionary<System.Type, GameObject>();


    protected override void Init()
    {
        base.Init();
    }

    private BaseUI GetUI<T>(out bool isAlreadyOpen)  // T는 열고자 하는 화면 UI 클래스 타입. 이것을 uiType으로 받아온다
    {
        System.Type uiType = typeof(T);

        BaseUI ui = null;
        isAlreadyOpen = false;

        // 만약에 m_OpenUIPool에 열고자 하는 UI 존재한다면
        if (m_OpenUIPool.ContainsKey(uiType))
        {
            // 풀에 있는 해당 오브젝트의 BaseUI 컴포넌트를 ui 변수에 대입
            ui = m_OpenUIPool[uiType].GetComponent<BaseUI>();
            isAlreadyOpen = true;
        }
        // 열고자 하는 UI가 존재만 한다면 (열리진 않음)
        else if (m_ClosedUIPool.ContainsKey(uiType))
        {
            ui = m_ClosedUIPool[uiType].GetComponent<BaseUI>();
            m_ClosedUIPool.Remove(uiType);
        }
        // 그렇지 않고 아예 한번도 생성된 적이 없는 인스턴스 라면
        else
        {
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            // 프리팹의 이름을 반드시 UI 클래스의 이름과 동일해야함
            // 그래야지 경로를 만들어서 리소스 폴더에서 로드할 수 있음
            ui = uiObj.GetComponent<BaseUI>();
        }

        return ui;
    }

    public void OpenUI<T>(BaseUIData uiData)
    {

        System.Type uiType = typeof(T);

        Debug.Log($"{GetType()}::OpenUI({uiType})"); // 어떤 UI 화면을 열고자 하는지 로그를 찍는다

        bool isAlreadyOpen = false;             // 이미 열려있는지 알 수 있는 변수 선언

        var ui = GetUI<T>(out isAlreadyOpen);

        // 없으면 에러 로그
        if (!ui)
        {
            return;
        }

        // 이미 열려있으면 이것 또한 비정상적인 요청이라고 간주하고 로그
        if (isAlreadyOpen)
        {
            return;
        }

        var siblingIdx = UICanvasTrs.childCount - 2;    // childCount 하위에 있는 게임 오브젝트 갯수
        ui.Init(UICanvasTrs);                           // UI 화면 초기화 ( 위치시켜야 할 상단 트랜스폼 )

        // 하이어라키 순위 변경 SetSiblingIndex : 매개변수를 넣어서 순위를 지정
        ui.transform.SetSiblingIndex(siblingIdx);

        ui.gameObject.SetActive(true);  // 컴포넌트가 연동된 게임오브젝트 활성화
        ui.SetInfo(uiData);             // UI 화면에 보이는 UI요소의 데이터를 세팅
        ui.ShowUI();

        m_FrontUI = ui;     // 현재 열고자하는 화면 UI가 가장 상단에 있는 UI가 될것이기 때문에 이렇게 설정
        m_OpenUIPool[uiType] = ui.gameObject;   // m_OpenUIPool에 생성한 UI 인스턴스를 넣어 준다
    }

    public void CloseUI(BaseUI ui)
    {
        System.Type uiType = ui.GetType();

        Debug.Log($"CloseUI UI:{uiType}"); // 어떤 UI를 닫아주는지 로그

        ui.gameObject.SetActive(false);

        m_OpenUIPool.Remove(uiType);            // 오픈 풀에서 제거
        m_ClosedUIPool[uiType] = ui.gameObject; // 클로즈풀에 추가
        ui.transform.SetParent(ClosedUITrs);    // ClosedUITrs 하위로 위치

        m_FrontUI = null;       // 최상단 UI null로 초기화

        //현재 최상단에 있는 UI화면 오브젝트를 가져온다
        var lastChild = UICanvasTrs.GetChild(UICanvasTrs.childCount - 1);

        // 만약에 UI가 존재한다면 이 UI화면 인스턴스를 최상단 UI로 대입
        if (lastChild)
        {
            m_FrontUI = lastChild.gameObject.GetComponent<BaseUI>();
        }
    }
}
