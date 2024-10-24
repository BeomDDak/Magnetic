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
        string _pass = backendSignUpPass.text;
        string _passCheck = backendSignUpPassChaeck.text;

        BackendLogin.Instance.CustomSignUp(_id, _pass,_passCheck);

    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pass = backendLoginPass.text;
        BackendLogin.Instance.CustomLogin(_id, _pass); // �ڳ� �α��� �Լ�
        Debug.Log("�׽�Ʈ�� �����մϴ�.");
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
