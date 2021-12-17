using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoSingleton<GameEngine>
{
    private List<IBaseManager> m_Managers;
    private Dictionary<Type, IBaseManager> m_ManagerDic;
    private GameSetting gameSetting;
    protected override void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartRun();
    }

    void RegisterrMangers()
    {
        RegisterManger<FsmManager>();
        RegisterManger<GameMapManager>();
        RegisterManger<ProcedureManager>();     
        RegisterManger<UIManager>();
        foreach (var manager in m_Managers)
        {
            manager.OnInit(this);
            manager.OnRegisterSetting(gameSetting);
        }
    }
    void RegisterManger<T>() where T : IBaseManager, new()
    {
        Type type = typeof(T);
        if (m_ManagerDic.ContainsKey(type))
        {
            Debug.Log(type.ToString() + "已经注册过到GameEngine");
            return;
        }
        T manager = new T();
        m_Managers.Add(manager);
        m_ManagerDic.Add(type, manager);
    }
    public T GetManager<T>() where T : IBaseManager
    {
        Type type = typeof(T);
        IBaseManager manager;
        m_ManagerDic.TryGetValue(type, out manager);
        if (manager != null)
        {
            return (T)manager;
        }
        else
        {
            Debug.Log(type.ToString() + "没有注册到GameEngine,在\"GetManager\"方法中无法获取！");
            return default(T);
        }
    }
    void Init()
    {
        m_Managers = new List<IBaseManager>();
        m_ManagerDic = new Dictionary<Type, IBaseManager>();
        gameSetting= GameSetting.Instance.Init();
    }
  

    void StartManagers()
    {
        foreach (var manager in m_Managers)
        {
            manager.OnStart();
        }
    }

    void UpdateManagers()
    {       
        foreach (var manager in m_Managers)
        {
            manager.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
    private void Update()
    {
        UpdateManagers();
    }
    void DestroyAllManagers()
    {       
        foreach (var manager in m_Managers)
        {
            manager.OnDestroy();
        }

        m_Managers.Clear();
        m_ManagerDic.Clear();
    }

    public virtual void StartRun()
    {
        Init();
      
        RegisterrMangers();
        StartManagers();
        
        //isStarted = true;
    }

  

    public virtual void Stop()
    {
        DestroyAllManagers();        
        m_Managers = null;
        m_ManagerDic = null;     
        Time.timeScale = 1.0f;
    }

    public virtual void Clear()
    {
     
    }
}
