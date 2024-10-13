using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : Singleton<BackendManager>
{
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

        //SignUp();
        //Login();
        //UpdateNickname();
        //GameDataInsert();
    }

    void SignUp()
    {
        BackendLogin.Instance.CustomSignUp("user1", "1234"); // [추가] 뒤끝 회원가입 함수
        Debug.Log("테스트를 종료합니다.");
        
    }

    void Login()
    {
        BackendLogin.Instance.CustomLogin("user1", "1234"); // [추가] 뒤끝 회원가입 함수
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
