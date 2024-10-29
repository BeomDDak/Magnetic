using System.Collections;
using System.Collections.Generic;
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

    protected override void Init()
    {
        isDestoryOnLoad = true;
    }

    void Start()
    {
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
            Debug.Log("�н����尡 ��ġ���� �ʽ��ϴ�");
            return;
        }

        var bro = Backend.BMember.CustomSignUp(_id, _pw);

        if (bro.IsSuccess())
        {
            Debug.Log("ȸ�����Կ� �����߽��ϴ�. : " + bro);
            UIManager.Instance.CloseUI(SignUI);
        }
        else
        {
            // ���� ó��
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            if (errorCode == "DuplicatedParameterException" && message.Contains("Duplicated customId"))
            {
                Debug.Log("�ߺ��� ���̵� �Դϴ�");
            }

            Debug.LogError("ȸ�����Կ� �����߽��ϴ�. : " + bro);
        }

    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pw = backendLoginPass.text;

        var bro = Backend.BMember.CustomLogin(_id, _pw);

        if (bro.IsSuccess())
        {
            Debug.Log("�α����� �����߽��ϴ�. : " + bro);

            Backend.BMember.UpdateNickname("user" + Time.time);

            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
        else
        {
            if (bro.StatusCode == 400)
            {
                Debug.Log("���̵� Ȥ�� ��й�ȣ�� Ȯ���� �� �����ϴ�");
            }
            else if (bro.StatusCode == 401)
            {
                Debug.Log("���̵� Ȥ�� ��й�ȣ�� �߸��Ǿ����ϴ�");
            }
            Debug.LogError("�α����� �����߽��ϴ�. : " + bro);
        }
    }

    void UpdateNickname()
    {
        BackendLogin.Instance.UpdateNickname("������");
        Debug.Log("�׽�Ʈ�� �����մϴ�");
    }

    void GameDataInsert()
    {
        BackendGameData.Instance.GameDataGet(); //[�߰�] ������ �ҷ����� �Լ�

        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }

        BackendGameData.Instance.LevelUp(); // [�߰�] ���ÿ� ����� �����͸� ����

        BackendGameData.Instance.GameDataUpdate(); //[�߰�] ������ ����� �����͸� �����(����� �κи�)
    }
}
