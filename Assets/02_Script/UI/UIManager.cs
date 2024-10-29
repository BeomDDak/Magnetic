using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    // �α���
    public GameObject LoginMessageUI;
    public TextMeshProUGUI LoginMessageText;
    public string LOGIN_FAIL_DONTMATCH { get; private set; } = "���̵� Ȥ�� ��й�ȣ�� Ȯ���� �� �����ϴ�";
    public string LOGIN_FAIL_CANTCONFIRM { get; private set; } = "���̵� Ȥ�� ��й�ȣ�� Ȯ���� �� �����ϴ�";
    public string LOGIN_SUCCESS { get; private set; } = "�α����� �����߽��ϴ�";
    public string SIGNUP_SUCCESS { get; private set; } = "ȸ�����Կ� �����߽��ϴ�";
    public string SIGNUP_FAIL_SAMEID { get; private set; } = "�ߺ��� ���̵� �Դϴ�";
    public string SIGNUP_FAIL_DONTMATCHPW { get; private set; } = "�н����尡 ��ġ���� �ʽ��ϴ�";




    public void OpenUI(GameObject _ui)
    {
        _ui.SetActive(true);
    }

    public void CloseUI(GameObject _ui)
    {
        _ui.SetActive(false);
    }
}
