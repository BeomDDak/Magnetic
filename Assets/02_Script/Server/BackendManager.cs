using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using TMPro;
using System.Security.Cryptography;

public class BackendManager : Singleton<BackendManager>
{
    // 로그인
    [SerializeField]
    private TMP_InputField backendLoginID;
    [SerializeField]
    private TMP_InputField backendLoginPass;
    private bool successLogin;

    // 회원가입
    [SerializeField]
    private TMP_InputField backendSignUpID;
    [SerializeField]
    private TMP_InputField backendSignUpPass;
    [SerializeField]
    private TMP_InputField backendSignUpPassChaeck;

    // UI
    [SerializeField]
    private GameObject SignUI;
    private UIManager m_UIManager;

    protected override void Init()
    {
        isDestoryOnLoad = true;
        m_UIManager = UIManager.Instance;
    }

    void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
    }

    public void SignUp()
    {
        string _id = backendSignUpID.text;
        string _pw = backendSignUpPass.text;
        string _pwCheck = backendSignUpPassChaeck.text;

        if (_pw != _pwCheck)
        {
            m_UIManager.LoginMessageText.text = m_UIManager.SIGNUP_FAIL_DONTMATCHPW;
            StartCoroutine(ShowUI());
            return;
        }

        var bro = Backend.BMember.CustomSignUp(_id, _pw);

        if (bro.IsSuccess())
        {
            m_UIManager.LoginMessageText.text = m_UIManager.SIGNUP_SUCCESS;
            UIManager.Instance.CloseUI(SignUI);
        }
        else
        {
            // 에러 처리
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            if (errorCode == "DuplicatedParameterException" && message.Contains("Duplicated customId"))
            {
                m_UIManager.LoginMessageText.text = m_UIManager.SIGNUP_FAIL_SAMEID;
            }
        }
        StartCoroutine(ShowUI());
    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pw = backendLoginPass.text;

        var bro = Backend.BMember.CustomLogin(_id, _pw);

        if (bro.IsSuccess())
        {
            m_UIManager.LoginMessageText.text = m_UIManager.LOGIN_SUCCESS;
            successLogin = true;
        }
        else
        {
            if (bro.StatusCode == 400)
            {
                m_UIManager.LoginMessageText.text = m_UIManager.LOGIN_FAIL_CANTCONFIRM;
            }
            else if (bro.StatusCode == 401)
            {
                m_UIManager.LoginMessageText.text = m_UIManager.LOGIN_FAIL_DONTMATCH;
            }
        }
        StartCoroutine(ShowUI());
    }

    IEnumerator ShowUI()
    {
        m_UIManager.OpenUI(m_UIManager.LoginMessageUI);
        yield return new WaitForSeconds(0.5f);
        m_UIManager.CloseUI(m_UIManager.LoginMessageUI);
        
        if (successLogin)
        {
            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }

    void UpdateNickname()
    {
        BackendLogin.Instance.UpdateNickname("아이폰");
        Debug.Log("테스트를 종료합니다");
    }

    void GameDataInsert()
    {
        BackendGameData.Instance.GameDataGet(); //[추가] 데이터 불러오기 함수

        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }

        BackendGameData.Instance.LevelUp(); // [추가] 로컬에 저장된 데이터를 변경

        BackendGameData.Instance.GameDataUpdate(); //[추가] 서버에 저장된 데이터를 덮어쓰기(변경된 부분만)
    }
}
