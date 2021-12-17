using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// UI节点类型
    /// </summary>
    public enum NodeType
    {
        BelowNavBar = 10,         //导航键下层界面
        NavBar = 50,                   //导航键界面
        AboveNavBar = 100,         //导航键上层界面
        Guide = 200,          //新手引导节点
        Confirm = 300,        //确认窗节点
        Bar = 400
    }
    
    public class BasePanel : MonoBehaviour
    {
        [HideInInspector]
        public NodeType mNodeType;     //UI节点类型
        [HideInInspector]
        public string mUIName;         //UIname

       

        private void Awake()
        {

        }

        /// <summary>
        /// 初始化UI参数
        /// </summary>
        public virtual void InitUIParam(string name, NodeType type = NodeType.AboveNavBar)
        {
            mUIName = name;
            mNodeType = type;
        }

        /// <summary>
        /// 是否需要显示顶部信息栏
        /// </summary>
        public virtual bool NeedShowTopPartInfo { get; set; } = false;
        /// <summary>
        /// 底部导航栏跳转界面类型
        /// </summary>
    

        /// <summary>
        /// 初始化UI
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 带数据的初始化
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="datas"></param>
        public virtual void Init<V>(List<V> datas)
        {

        }

        /// <summary>
        /// 带数据的初始化
        /// </summary>
        /// <param name="param">参数</param>
        public virtual void Init(object param)
        {

        }

        /// <summary>
        /// 关闭并销毁UI
        /// </summary>
        public virtual void CloseUI()
        {
            //UIManager.Instance.CloseUI(this, true);
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public virtual void HideUI()
        {
            //UIManager.Instance.CloseUI(this, false);
        }

        public virtual void OnExit(bool isDestroy)
        {
            if (isDestroy)
                CloseUI();
            else
                HideUI();


        }

       

       

        public virtual void SetNeedAtlasList() { }
    }

