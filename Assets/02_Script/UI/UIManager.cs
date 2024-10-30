using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    // 로그인
    public GameObject LoginMessageUI;
    public TextMeshProUGUI LoginMessageText;
    public string LOGIN_FAIL_DONTMATCH = "아이디 혹은 비밀번호가 틀렸습니다";
    public string LOGIN_FAIL_CANTCONFIRM = "아이디 혹은 비밀번호를 확인할 수 없습니다";
    public string LOGIN_SUCCESS = "로그인이 성공했습니다";
    public string SIGNUP_SUCCESS = "회원가입에 성공했습니다";
    public string SIGNUP_FAIL_SAMEID = "중복된 아이디 입니다";
    public string SIGNUP_FAIL_DONTMATCHPW = "패스워드가 일치하지 않습니다";




    public void OpenUI(GameObject _ui)
    {
        _ui.SetActive(true);
    }

    public void CloseUI(GameObject _ui)
    {
        _ui.SetActive(false);
    }
}
