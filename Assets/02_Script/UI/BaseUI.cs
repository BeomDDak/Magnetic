using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUIData
{
    public Action OnShow;       // UI ȭ���� ������ �� ���ְ� ���� ����
    public Action OnClose;      // UI ȭ���� �����鼭 �����ؾ� �Ǵ� ���
}

public class BaseUI : MonoBehaviour
{
    private Action m_OnShow;
    private Action m_OnClose;

    public virtual void Init(Transform anchor)
    {
        m_OnShow = null;
        m_OnClose = null;

        transform.SetParent(anchor); // anchor : UIĵ���� ������Ʈ�� Ʈ������

        var rectTransform = GetComponent<RectTransform>();
        if (!rectTransform)
        {
            return;
        }

        // �⺻ ������ ���� �ʱ�ȭ
        rectTransform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }

    // UI ȭ�鿡 UI ��Ҹ� �������ִ� �Լ�
    public virtual void SetInfo(BaseUIData uiData)
    {
        m_OnShow = uiData.OnShow;
        m_OnClose = uiData.OnClose;
    }

    // UI ȭ���� ������ ��� ȭ�鿡 ǥ���� �ִ� �Լ�
    public virtual void ShowUI()
    {
        m_OnShow?.Invoke(); // m_OnShow�� null�� �ƴ϶�� m_OnShow ����

        m_OnShow = null; // ���� �� null������ �ʱ�ȭ
    }

    public virtual void CloseUI(bool isCloseAll = false)
    {
        // isCloseAll : ���� ��ȯ�ϰų� �� �� �����ִ� ȭ����
        // ���� �� �ݾ��� �ʿ䰡 ���� ��
        // true �� �Ѱ��༭ ȭ���� ���� �� �ʿ��� ó������
        // �� �����ϰ� ȭ�鸸 �ݾ��ֱ� ���ؼ� ����� ����

        if (!isCloseAll)
        {
            m_OnClose?.Invoke();
        }
        m_OnClose = null;

        UIManager.Instance.CloseUI(this); //CloseUI�� �� �ν��Ͻ��� �Ű������� �־��ش�
    }
}
