using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public void OpenUI(GameObject _ui)
    {
        _ui.SetActive(true);
    }

    public void CloseUI(GameObject _ui)
    {
        _ui.SetActive(false);
    }


}
