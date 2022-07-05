using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UIMsgID
{
    None = 0,
}

public class UIManager :IBaseManager
{
    //UI节点
    public RectTransform m_UiRoot;
    //窗口节点
    private RectTransform m_panelRoot;
    //UI摄像机
    private Camera m_UICamera;
    //EventSystem节点
    private EventSystem m_EventSystem;
    //屏幕的宽高比
    private float m_CanvasRate = 0;
    private GameObject mPrefab;
    private string m_UIPrefabPath = "Assets/GameData/Prefabs/UGUI/Panel/";
    //注册的字典
    private Dictionary<string, System.Type> m_RegisterDic = new Dictionary<string, System.Type>();
    //所有打开的窗口
    private Dictionary<string, BasePanel> m_BasePanelDic = new Dictionary<string, BasePanel>();
    //打开的窗口列表
    private List<BasePanel> m_BasePanelList = new List<BasePanel>();
    private Dictionary<string, BasePanel> dic_panel = new Dictionary<string, BasePanel>();

    

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="uiRoot">UI父节点</param>
    /// <param name="panelRoot">窗口父节点</param>
    /// <param name="uiCamera">UI摄像机</param>
  
    public  void OnRegisterSetting(GameSetting gameSetting)
    {      
        m_UiRoot = gameSetting.uiRoot;
        m_panelRoot = gameSetting.panelRoot;
        m_UICamera = gameSetting.UICamera;
        m_EventSystem = gameSetting.EventSystem;
        m_CanvasRate = Screen.height / (m_UICamera.orthographicSize * 2);
        RegiseAllControll();
    }
    public void Teset()
    {
        Debug.LogError("23333");
    }
    /// <summary>
    /// 设置所有节目UI路径
    /// </summary>
    /// <param name="path"></param>
    public void SetUIPrefabPath(string path)
    {
        m_UIPrefabPath = path;
    }
    public void ShowPanel<T>(Action<bool, T> callback = null) where T : BasePanel
    {
        string panelName = typeof(T).ToString();     
        if (dic_panel.ContainsKey(panelName))
        {
            var panelSctipy = dic_panel[panelName];
            //ShowPanel4(panel);
            panelSctipy.Init();
            callback?.Invoke(true, dic_panel[panelName].GetComponent<T>());
         
        }
        else
        {
           GameEngine.Instance.StartCoroutine(AddPanelAsync<T>(callback));
        }     
    }
    void SetPrefab<T>(string name) where T : BasePanel
    {
        GameObject go;
        T panel;
        if (!dic_panel.ContainsKey(name))
        {
            go = GameObject.Instantiate(mPrefab) as GameObject;
            panel = go.GetComponent<T>();
            if (null == panel)
                panel = go.AddComponent<T>();
           // yield return panel.PreloadAtlas();
            if (dic_panel.ContainsKey(name))
            {
                dic_panel.Remove(name);
            }
            dic_panel.Add(name, panel);
        }
        else
        {
            go = dic_panel[name].gameObject;
            panel = go.AddComponent<T>();
        }

        //设置UI基础参数
        panel.InitUIParam(name);

        panel.Init();

        go.transform.SetParent(m_UiRoot, false);

        go.name = name;

      

    }
    private IEnumerator AddPanelAsync<T>(Action<bool, T> finishedCallback) where T : BasePanel
    {     
        string name = typeof(T).ToString();
        if (dic_panel.ContainsKey(name))
        {
            finishedCallback?.Invoke(false, null);
         
            yield break;
        }

        mPrefab = null;
        yield return LoadPrefab(name);
        if (mPrefab == null)
        {
            finishedCallback?.Invoke(false, null);
        }
        else
        {
            SetPrefab<T>(name);
            finishedCallback?.Invoke(true, dic_panel[name].GetComponent<T>());
        }
    }

    private IEnumerator LoadPrefab(string name)
    {
        name = "TestPanel";
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(name);
        if (!handle.IsValid())
        {
           
            yield break;
        }
        yield return handle;
        if (handle.IsDone)
        {
           
            mPrefab = handle.Result;
        }

        if (null == mPrefab)
        {
           
            yield break;
        }
     
    }
    /// <summary>
    /// 显示或者隐藏所有UI
    /// </summary>
    public void ShowOrHideUI(bool show)
    {
        if (m_UiRoot != null)
        {
            m_UiRoot.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// 设置默认选择对象
    /// </summary>
    /// <param name="obj"></param>
    public void SetNormalSelectObj(GameObject obj)
    {
        if (m_EventSystem == null)
        {
            m_EventSystem = EventSystem.current;
        }
        m_EventSystem.firstSelectedGameObject = obj;
    }

    /// <summary>
    /// 窗口的更新
    /// </summary>
    public void OnUpdate()
    {
        for (int i = 0; i < m_BasePanelList.Count; i++)
        {
            if (m_BasePanelList[i] != null)
            {
                //m_BasePanelList[i].OnUpdate();
            }
        }
    }

    /// <summary>
    /// 窗口注册方法
    /// </summary>
    /// <typeparam name="T">窗口泛型类</typeparam>
    /// <param name="name">窗口名</param>
    public void Register<T>(string name) where T : BasePanel
    {
        m_RegisterDic[name] = typeof(T);
    }

    /// <summary>
    /// 发送消息给窗口
    /// </summary>
    /// <param name="name">窗口名</param>
    /// <param name="msgID">消息ID</param>
    /// <param name="paralist">参数数组</param>
    /// <returns></returns>
    public bool SendMessageTopanel(string name, UIMsgID msgID = 0, params object[] paralist)
    {
        BasePanel panel = FindpanelByName<BasePanel>(name);
        if (panel != null)
        {
            // return panel.OnMessage(msgID, paralist);
        }
        return false;
    }

    /// <summary>
    /// 根据窗口名查找窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T FindpanelByName<T>(string name) where T : BasePanel
    {
        BasePanel panel = null;
        if (dic_panel.TryGetValue(name, out panel))
        {
            return (T)panel;
        }

        return null;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="bTop"></param>
    /// <param name="para1"></param>
    /// <param name="para2"></param>
    /// <param name="para3"></param>
    /// <returns></returns>
    public void PopUppanel(BasePanel panel)
    {

    }

    public void OnInit(GameEngine gameEngine)
    {
        
    }

    public void OnShutdown()
    {
       
    }

    public void OnStart()
    {
       
    }

    public void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
      
    }

    public void OnDestroy()
    {
       
    }

    /// <summary>
    /// 根据窗口名关闭窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destory"></param>
    //public void Closepanel(string name, bool destory = false)
    //{
    //    BasePanel panel = FindpanelByName<BasePanel>(name);
    //    Closepanel(panel, destory);
    //}

    /// <summary>
    /// 根据窗口对象关闭窗口
    /// </summary>
    /// <param name="BasePanel"></param>
    /// <param name="destory"></param>
    //public void Closepanel(BasePanel BasePanel, bool destory = false)
    //{
    //    if (BasePanel != null)
    //    {
    //        BasePanel.OnDisable();
    //        BasePanel.OnClose();
    //        if (m_BasePanelDic.ContainsKey(BasePanel.Name))
    //        {
    //            m_BasePanelDic.Remove(BasePanel.Name);
    //            m_BasePanelList.Remove(BasePanel);
    //        }

    //        if (destory)
    //        {
    //            ObjectManager.Instance.ReleaseObject(BasePanel.GameObject, 0, true);
    //        }
    //        else
    //        {
    //            ObjectManager.Instance.ReleaseObject(BasePanel.GameObject, recycleParent: false);
    //        }
    //        BasePanel.GameObject = null;
    //        BasePanel = null;
    //    }
    //}

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    //public void CloseAllpanel()
    //{
    //    for (int i = m_BasePanelList.Count - 1; i >= 0; i--)
    //    {
    //        Closepanel(m_BasePanelList[i]);
    //    }
    //}

    /// <summary>
    /// 切换到唯一窗口
    /// </summary>
    //public void SwitchStateByName(string name, bool bTop = true, params object[] paralist)
    //{
    //    CloseAllpanel();
    //    PopUppanel(name, bTop, paralist);
    //}

    /// <summary>
    /// 根据名字隐藏窗口
    /// </summary>
    /// <param name="name"></param>
    //public void Hidepanel(string name)
    //{
    //    BasePanel panel = FindpanelByName<BasePanel>(name);
    //    Hidepanel(panel);
    //}

    /// <summary>
    /// 根据窗口对象隐藏窗口
    /// </summary>
    /// <param name="panel"></param>

    //public void Hidepanel(BasePanel panel)
    //{
    //    if (panel != null)
    //    {
    //        panel.GameObject.SetActive(false);
    //        panel.OnDisable();
    //    }
    //}

    /// <summary>
    /// 根据窗口名字显示窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="paralist"></param>
    //public void Showpanel(string name, bool bTop = true, params object[] paralist)
    //{
    //    BasePanel panel = FindpanelByName<BasePanel>(name);
    //    Showpanel(panel, bTop, paralist);
    //}

    /// <summary>
    /// 根据窗口对象显示窗口
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="paralist"></param>
    //public void Showpanel(BasePanel panel, bool bTop = true, params object[] paralist)
    //{
    //    if (panel != null)
    //    {
    //        if (panel.GameObject != null && !panel.GameObject.activeSelf) panel.GameObject.SetActive(true);
    //        if (bTop) panel.Transform.SetAsLastSibling();
    //        panel.OnShow(paralist);
    //    }
    //}

    public void RegiseAllControll()
    {
        var types = Assembly.GetCallingAssembly().GetTypes();
        var aType = typeof(IController);
        List<IController> ass = new List<IController>();
        var typess = Assembly.GetCallingAssembly().GetTypes();  //获取所有类型
        foreach (var t in typess)
        {
            Type[] tfs = t.GetInterfaces();  //获取该类型的接口
            foreach (var tf in tfs)
            {
                if (tf.FullName == aType.FullName)  //判断全名，是否在一个命名空间下面
                {
                    IController a = Activator.CreateInstance(t) as IController;
                    ass.Add(a);
                }
            }
        }
        Debug.Log(ass.Count);
        foreach (var item in ass)
        {
            item.Init();  //调用所有继承该接口的类中的方法
        }

    }
}

public interface IModel 
{
    void Init();
}
public interface ISystem
{
    void Init();
}
public interface IController 
{
    void Init();
}