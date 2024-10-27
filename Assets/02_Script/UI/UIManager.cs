using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Transform UICanvasTrs;  // UI ȭ���� ������ �� ĵ���� ������Ʈ Ʈ������
    public Transform ClosedUITrs;  // UI ȭ���� ���� �� ��Ȱ��ȭ ��Ų UI ȭ����� ��ġ������ Ʈ������
    private BaseUI m_FrontUI;      // UI ȭ���� �������� �� ���� ��ܿ� �����ִ� UI

    // ���� �����ִ�, �� Ȱ��ȭ �Ǿ��ִ� UI ȭ���� ��� �ִ� ����(Ǯ)
    private Dictionary<System.Type, GameObject> m_OpenUIPool = new Dictionary<System.Type, GameObject>();

    // �����ִ�, �� ��Ȱ��ȭ �Ǿ��ִ� UI ȭ���� ��� �ִ� ����(Ǯ)
    private Dictionary<System.Type, GameObject> m_ClosedUIPool = new Dictionary<System.Type, GameObject>();


    protected override void Init()
    {
        base.Init();
    }

    private BaseUI GetUI<T>(out bool isAlreadyOpen)  // T�� ������ �ϴ� ȭ�� UI Ŭ���� Ÿ��. �̰��� uiType���� �޾ƿ´�
    {
        System.Type uiType = typeof(T);

        BaseUI ui = null;
        isAlreadyOpen = false;

        // ���࿡ m_OpenUIPool�� ������ �ϴ� UI �����Ѵٸ�
        if (m_OpenUIPool.ContainsKey(uiType))
        {
            // Ǯ�� �ִ� �ش� ������Ʈ�� BaseUI ������Ʈ�� ui ������ ����
            ui = m_OpenUIPool[uiType].GetComponent<BaseUI>();
            isAlreadyOpen = true;
        }
        // ������ �ϴ� UI�� ���縸 �Ѵٸ� (������ ����)
        else if (m_ClosedUIPool.ContainsKey(uiType))
        {
            ui = m_ClosedUIPool[uiType].GetComponent<BaseUI>();
            m_ClosedUIPool.Remove(uiType);
        }
        // �׷��� �ʰ� �ƿ� �ѹ��� ������ ���� ���� �ν��Ͻ� ���
        else
        {
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            // �������� �̸��� �ݵ�� UI Ŭ������ �̸��� �����ؾ���
            // �׷����� ��θ� ���� ���ҽ� �������� �ε��� �� ����
            ui = uiObj.GetComponent<BaseUI>();
        }

        return ui;
    }

    public void OpenUI<T>(BaseUIData uiData)
    {

        System.Type uiType = typeof(T);

        Debug.Log($"{GetType()}::OpenUI({uiType})"); // � UI ȭ���� ������ �ϴ��� �α׸� ��´�

        bool isAlreadyOpen = false;             // �̹� �����ִ��� �� �� �ִ� ���� ����

        var ui = GetUI<T>(out isAlreadyOpen);

        // ������ ���� �α�
        if (!ui)
        {
            return;
        }

        // �̹� ���������� �̰� ���� ���������� ��û�̶�� �����ϰ� �α�
        if (isAlreadyOpen)
        {
            return;
        }

        var siblingIdx = UICanvasTrs.childCount - 2;    // childCount ������ �ִ� ���� ������Ʈ ����
        ui.Init(UICanvasTrs);                           // UI ȭ�� �ʱ�ȭ ( ��ġ���Ѿ� �� ��� Ʈ������ )

        // ���̾��Ű ���� ���� SetSiblingIndex : �Ű������� �־ ������ ����
        ui.transform.SetSiblingIndex(siblingIdx);

        ui.gameObject.SetActive(true);  // ������Ʈ�� ������ ���ӿ�����Ʈ Ȱ��ȭ
        ui.SetInfo(uiData);             // UI ȭ�鿡 ���̴� UI����� �����͸� ����
        ui.ShowUI();

        m_FrontUI = ui;     // ���� �������ϴ� ȭ�� UI�� ���� ��ܿ� �ִ� UI�� �ɰ��̱� ������ �̷��� ����
        m_OpenUIPool[uiType] = ui.gameObject;   // m_OpenUIPool�� ������ UI �ν��Ͻ��� �־� �ش�
    }

    public void CloseUI(BaseUI ui)
    {
        System.Type uiType = ui.GetType();

        Debug.Log($"CloseUI UI:{uiType}"); // � UI�� �ݾ��ִ��� �α�

        ui.gameObject.SetActive(false);

        m_OpenUIPool.Remove(uiType);            // ���� Ǯ���� ����
        m_ClosedUIPool[uiType] = ui.gameObject; // Ŭ����Ǯ�� �߰�
        ui.transform.SetParent(ClosedUITrs);    // ClosedUITrs ������ ��ġ

        m_FrontUI = null;       // �ֻ�� UI null�� �ʱ�ȭ

        //���� �ֻ�ܿ� �ִ� UIȭ�� ������Ʈ�� �����´�
        var lastChild = UICanvasTrs.GetChild(UICanvasTrs.childCount - 1);

        // ���࿡ UI�� �����Ѵٸ� �� UIȭ�� �ν��Ͻ��� �ֻ�� UI�� ����
        if (lastChild)
        {
            m_FrontUI = lastChild.gameObject.GetComponent<BaseUI>();
        }
    }
}
