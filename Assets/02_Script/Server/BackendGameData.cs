using UnityEngine;
using System.Text;
using BackEnd;

public class UserData
{
    public string userName = string.Empty;
    public int win = 0;
    public int lose = 0;
    public int energy = 5;

    // �����͸� ������ϱ� ���� �Լ��Դϴ�.(Debug.Log(UserData);)
    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"NickName : {userName}");
        result.AppendLine($"Win : {win}");
        result.AppendLine($"Lose : {lose}");
        result.AppendLine($"Energy : {energy}");

        return result.ToString();
    }
}

public class BackendGameData
{
    private static BackendGameData _instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendGameData();
            }

            return _instance;
        }
    }

    public static UserData userData;

    private string gameDataRowInDate = string.Empty;

    public void GameDataInsert()
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        Debug.Log("�����͸� �ʱ�ȭ�մϴ�.");
        userData.userName = string.Empty;
        userData.win = 0;
        userData.lose = 0;
        userData.energy = 5;

        Debug.Log("�ڳ� ������Ʈ ��Ͽ� �ش� �����͵��� �߰��մϴ�.");
        Param param = new Param();
        param.Add("userName", userData.userName);
        param.Add("win", userData.win);
        param.Add("lose", userData.lose);
        param.Add("energy", userData.energy);

        Debug.Log("���� ���� ������ ������ ��û�մϴ�.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ������ ���Կ� �����߽��ϴ�. : " + bro);

            //������ ���� ������ �������Դϴ�.  
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("���� ���� ������ ���Կ� �����߽��ϴ�. : " + bro);
        }
    }

    public void GameDataGet()
    {
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);


            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json���� ���ϵ� �����͸� �޾ƿ�  

            // �޾ƿ� �������� ������ 0�̶�� �����Ͱ� �������� �ʴ� �� 
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
                GameDataInsert();
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //�ҷ��� ���� ������ ������  

                userData = new UserData();

                userData.userName = gameDataJson[0]["userName"].ToString();
                userData.win = int.Parse(gameDataJson[0]["win"].ToString());
                userData.lose = int.Parse(gameDataJson[0]["lose"].ToString());
                userData.energy = int.Parse(gameDataJson[0]["energy"].ToString());

                Debug.Log(userData.ToString());
            }
        }
        else
        {
            Debug.LogError("���� ���� ��ȸ�� �����߽��ϴ�. : " + bro);
        }
    }

    public void GameDataUpdate()
    {
        if (userData == null)
        {
            Debug.LogError("�������� �ٿ�ްų� ���� ������ �����Ͱ� �������� �ʽ��ϴ�. Insert Ȥ�� Get�� ���� �����͸� �������ּ���.");
            return;
        }

        Param param = new Param();
        param.Add("userName", userData.userName);
        param.Add("win", userData.win);
        param.Add("lose", userData.lose);
        param.Add("energy", userData.energy);

        BackendReturnObject bro = null;
        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("�� ���� �ֽ� ���� ���� ������ ������ ��û�մϴ�.");
            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}�� ���� ���� ������ ������ ��û�մϴ�.");
            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("���� ���� ������ ������ �����߽��ϴ�. : " + bro);
        }
        else
        {
            Debug.LogError("���� ���� ������ ������ �����߽��ϴ�. : " + bro);
        }
    }
}