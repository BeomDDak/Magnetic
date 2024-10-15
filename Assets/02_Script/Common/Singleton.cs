using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T m_instance;
    protected bool isDestoryOnLoad = false;

    public static T Instance
    {
        get { return m_instance; }
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (m_instance == null)
        {
            m_instance = (T)this;

            if (!isDestoryOnLoad)
            {
                DontDestroyOnLoad(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
