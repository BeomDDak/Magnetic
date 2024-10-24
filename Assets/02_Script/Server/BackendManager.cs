using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using TMPro;

public class BackendManager : Singleton<BackendManager>
{
    // 로그인
    [SerializeField]
    private TMP_InputField backendLoginID;
    [SerializeField]
    private TMP_InputField backendLoginPass;

    // 회원가입
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
        string _pass = backendSignUpPass.text;
        string _passCheck = backendSignUpPassChaeck.text;

        BackendLogin.Instance.CustomSignUp(_id, _pass,_passCheck);

    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pass = backendLoginPass.text;
        BackendLogin.Instance.CustomLogin(_id, _pass); // 뒤끝 로그인 함수
        Debug.Log("테스트를 종료합니다.");
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
