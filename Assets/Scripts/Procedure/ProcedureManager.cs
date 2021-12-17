//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------


using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 流程管理器。
/// </summary>
   public  sealed class ProcedureManager : IBaseManager, IProcedureManager
    {
        private IFsmManager m_FsmManager;
        private IFsm<IProcedureManager> m_ProcedureFsm;
    private ProcedureBase m_EntranceProcedure = null;
    //[SerializeField]
    //private string[] m_AvailableProcedureTypeNames = null;

    //[SerializeField]
    //private string m_EntranceProcedureTypeName = null;
    /// <summary>
    /// 初始化流程管理器的新实例。
    /// </summary>
    public ProcedureManager()
        {
            m_FsmManager = null;
            m_ProcedureFsm = null;
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal  int Priority
        {
            get
            {
                return -10;
            }
        }
   
    /// <summary>
    /// 获取当前流程。
    /// </summary>
    public ProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    //throw new GameFrameworkException("You must initialize procedure first.");
                }

                return (ProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    //throw new GameFrameworkException("You must initialize procedure first.");
                }

                return m_ProcedureFsm.CurrentStateTime;
            }
        }
    public string GetBeginProcedureName(ProcedureType procedureType)
    {
        switch (procedureType)
        {
            case ProcedureType.Start:
                return "ProcedureStart";
        }
        return null;
    }
    public void OnStart()
    {
      GameEngine.Instance.StartCoroutine(StartProcedure());
    }
    private IEnumerator StartProcedure()
    {
        ProcedureBase[] procedures = new ProcedureBase[GameSetting.Instance.procedureSetting.procedureName.Count];
        for (int i = 0; i < GameSetting.Instance.procedureSetting.procedureName.Count; i++)
        {
            //Type procedureType = Assembly.Load(GetBeginProcedureName(GameSetting.Instance.procedureSetting.procedureName[i])).GetType();
            //if (procedureType == null)
            //{
            //   // Log.Error("Can not find procedure type     '{0}'.", m_AvailableProcedureTypeNames[i]);
            //    yield break;
            //}

            procedures[i] = (ProcedureBase)Assembly.Load(Assembly.GetExecutingAssembly().GetName()).CreateInstance(GameSetting.Instance.procedureSetting.procedureName[i]);
            if (procedures[i] == null)
            {
                //Log.Error("Can not create procedure instance '{0}'.", m_AvailableProcedureTypeNames[i]);
                yield break;
            }

            //if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
            //{
            //    m_EntranceProcedure = procedures[i];
            //}
        }
        m_EntranceProcedure = procedures[0];
        if (m_EntranceProcedure == null)
        {
          
            yield break;
        }

        Initialize(procedures);

        yield return new WaitForEndOfFrame();

        StartProcedure(m_EntranceProcedure.GetType());
    }
    public  void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理流程管理器。
        /// </summary>
        public  void OnShutdown()
        {
            if (m_FsmManager != null)
            {
                if (m_ProcedureFsm != null)
                {
                    m_FsmManager.DestroyFsm(m_ProcedureFsm);
                    m_ProcedureFsm = null;
                }

                m_FsmManager = null;
            }
        }

        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        /// <param name="fsmManager">有限状态机管理器。</param>
        /// <param name="procedures">流程管理器包含的流程。</param>
        public void Initialize( params ProcedureBase[] procedures)
        {

            m_FsmManager = GameEngine.Instance.GetManager<FsmManager>(); 
            m_ProcedureFsm = m_FsmManager.CreateFsm(this, procedures);
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
             //   throw new GameFrameworkException("You must initialize procedure first.");
            }

            m_ProcedureFsm.Start<T>();
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <param name="procedureType">要开始的流程类型。</param>
        public void StartProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
              //  throw new GameFrameworkException("You must initialize procedure first.");
            }

            m_ProcedureFsm.Start(procedureType);
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
              //  throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.HasState<T>();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <param name="procedureType">要检查的流程类型。</param>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
               // throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.HasState(procedureType);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
               // throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
               // throw new GameFrameworkException("You must initialize procedure first.");
            }

            return (ProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }

    public void OnInit(GameEngine gameEngine)
    {
       
    }

    public void OnRegisterSetting(GameSetting gameSetting)
    {
        
    }

    public void OnDestroy()
    {
      
    }
}

