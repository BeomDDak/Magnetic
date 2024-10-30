using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    // �α���
    public GameObject LoginMessageUI;
    public TextMeshProUGUI LoginMessageText;
    public string LOGIN_FAIL_DONTMATCH = "���̵� Ȥ�� ��й�ȣ�� Ʋ�Ƚ��ϴ�";
    public string LOGIN_FAIL_CANTCONFIRM = "���̵� Ȥ�� ��й�ȣ�� Ȯ���� �� �����ϴ�";
    public string LOGIN_SUCCESS = "�α����� �����߽��ϴ�";
    public string SIGNUP_SUCCESS = "ȸ�����Կ� �����߽��ϴ�";
    public string SIGNUP_FAIL_SAMEID = "�ߺ��� ���̵� �Դϴ�";
    public string SIGNUP_FAIL_DONTMATCHPW = "�н����尡 ��ġ���� �ʽ��ϴ�";




    public void OpenUI(GameObject _ui)
    {
        _ui.SetActive(true);
    }

    public void CloseUI(GameObject _ui)
    {
        _ui.SetActive(false);
    }
}
