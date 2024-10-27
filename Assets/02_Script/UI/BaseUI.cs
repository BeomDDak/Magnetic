using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUIData
{
    public Action OnShow;       // UI 화면을 열었을 때 해주고 싶은 행위
    public Action OnClose;      // UI 화면을 닫으면서 실행해야 되는 기능
}

public class BaseUI : MonoBehaviour
{
    private Action m_OnShow;
    private Action m_OnClose;

    public virtual void Init(Transform anchor)
    {
        m_OnShow = null;
        m_OnClose = null;

        transform.SetParent(anchor); // anchor : UI캔버스 컴포넌트의 트랜스폼

        var rectTransform = GetComponent<RectTransform>();
        if (!rectTransform)
        {
            return;
        }

        // 기본 값으로 전부 초기화
        rectTransform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }

    // UI 화면에 UI 요소를 세팅해주는 함수
    public virtual void SetInfo(BaseUIData uiData)
    {
        m_OnShow = uiData.OnShow;
        m_OnClose = uiData.OnClose;
    }

    // UI 화면을 실제로 열어서 화면에 표시해 주는 함수
    public virtual void ShowUI()
    {
        m_OnShow?.Invoke(); // m_OnShow가 null이 아니라면 m_OnShow 실행

        m_OnShow = null; // 실행 후 null값으로 초기화
    }

    public virtual void CloseUI(bool isCloseAll = false)
    {
        // isCloseAll : 씬을 전환하거나 할 때 열려있는 화면을
        // 전부 다 닫아줄 필요가 있을 때
        // true 를 넘겨줘서 화면을 닫을 때 필요한 처리들을
        // 다 무시하고 화면만 닫아주기 위해서 사용할 것임

        if (!isCloseAll)
        {
            m_OnClose?.Invoke();
        }
        m_OnClose = null;

        UIManager.Instance.CloseUI(this); //CloseUI에 이 인스턴스를 매개변수로 넣어준다
    }
}
