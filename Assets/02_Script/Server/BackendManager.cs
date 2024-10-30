using System.Collections;
using UnityEngine;
using BackEnd;
using TMPro;

public class BackendManager : Singleton<BackendManager>
{
    // �α���
    [SerializeField]
    private TMP_InputField backendLoginID;
    [SerializeField]
    private TMP_InputField backendLoginPass;
    private bool successLogin;

    // ȸ������
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
    string UImessage;

    protected override void Init()
    {
        isDestoryOnLoad = true; 
    }

    void Start()
    {
        m_UIManager = UIManager.Instance;
        var bro = Backend.Initialize(); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 400�� ���� �߻�
        }
    }

    public void SignUp()
    {
        string _id = backendSignUpID.text;
        string _pw = backendSignUpPass.text;
        string _pwCheck = backendSignUpPassChaeck.text;

        if (_pw != _pwCheck)
        {
            UImessage = m_UIManager.SIGNUP_FAIL_DONTMATCHPW;
            StartCoroutine(ShowUI(UImessage));
            return;
        }

        var bro = Backend.BMember.CustomSignUp(_id, _pw);

        if (bro.IsSuccess())
        {
            UImessage = m_UIManager.SIGNUP_SUCCESS;
            UIManager.Instance.CloseUI(SignUI);
        }
        else
        {
            // ���� ó��
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            if (errorCode == "DuplicatedParameterException" && message.Contains("Duplicated customId"))
            {
                UImessage = m_UIManager.SIGNUP_FAIL_SAMEID;
            }
        }
        StartCoroutine(ShowUI(UImessage));
    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pw = backendLoginPass.text;

        var bro = Backend.BMember.CustomLogin(_id, _pw);

        if (bro.IsSuccess())
        {
            UImessage = m_UIManager.LOGIN_SUCCESS;
            successLogin = true;
        }
        else
        {
            if (bro.StatusCode == 400)
            {
                UImessage = m_UIManager.LOGIN_FAIL_CANTCONFIRM;
            }
            else if (bro.StatusCode == 401)
            {
                UImessage = m_UIManager.LOGIN_FAIL_DONTMATCH;
            }
        }
        StartCoroutine(ShowUI(UImessage));
    }

    IEnumerator ShowUI(string message)
    {
        m_UIManager.OpenUI(m_UIManager.LoginMessageUI);
        m_UIManager.LoginMessageText.text = message;
        yield return new WaitForSeconds(1f);
        m_UIManager.CloseUI(m_UIManager.LoginMessageUI);
        
        if (successLogin)
        {
            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }
}
