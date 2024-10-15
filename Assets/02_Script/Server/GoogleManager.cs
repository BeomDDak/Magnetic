using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleManager : Singleton<GoogleManager>
{
    public void GoogleLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            SuccessLogin();
        }
        else
        {
            Debug.LogError("·Î±×ÀÎ ¾ÈµÊ");
        }
    }

    IEnumerator SuccessLogin()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.Instance.LoadScene(SceneType.Lobby);
    }
}
