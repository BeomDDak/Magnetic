using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : Singleton<BackendManager>
{
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

        //SignUp();
        //Login();
        //UpdateNickname();
        //GameDataInsert();
    }

    void SignUp()
    {
        BackendLogin.Instance.CustomSignUp("user1", "1234"); // [�߰�] �ڳ� ȸ������ �Լ�
        Debug.Log("�׽�Ʈ�� �����մϴ�.");
        
    }

    void Login()
    {
        BackendLogin.Instance.CustomLogin("user1", "1234"); // [�߰�] �ڳ� ȸ������ �Լ�
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
