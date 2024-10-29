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

    // UI
    [SerializeField]
    private GameObject SignUI;

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
        string _pw = backendSignUpPass.text;
        string _pwCheck = backendSignUpPassChaeck.text;

        if (_pw != _pwCheck)
        {
            Debug.Log("패스워드가 일치하지 않습니다");
            return;
        }

        var bro = Backend.BMember.CustomSignUp(_id, _pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
            UIManager.Instance.CloseUI(SignUI);
        }
        else
        {
            // 에러 처리
            string errorCode = bro.GetErrorCode();
            string message = bro.GetMessage();

            if (errorCode == "DuplicatedParameterException" && message.Contains("Duplicated customId"))
            {
                Debug.Log("중복된 아이디 입니다");
            }

            Debug.LogError("회원가입에 실패했습니다. : " + bro);
        }

    }

    public void Login()
    {
        string _id = backendLoginID.text;
        string _pw = backendLoginPass.text;

        var bro = Backend.BMember.CustomLogin(_id, _pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);

            Backend.BMember.UpdateNickname("user" + Time.time);

            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
        else
        {
            if (bro.StatusCode == 400)
            {
                Debug.Log("아이디 혹은 비밀번호를 확인할 수 없습니다");
            }
            else if (bro.StatusCode == 401)
            {
                Debug.Log("아이디 혹은 비밀번호가 잘못되었습니다");
            }
            Debug.LogError("로그인이 실패했습니다. : " + bro);
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
