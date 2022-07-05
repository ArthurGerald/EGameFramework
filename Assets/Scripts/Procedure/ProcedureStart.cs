using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = IFsm<IProcedureManager>;
/// <summary>
/// 项目开始流程用于加载项目配置
/// </summary>
public class ProcedureStart : ProcedureBase
{
    bool isLoadConigSucess;
    ProcedureOwner procedure;
    protected internal override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.LogError("进入start");
        isLoadConigSucess = false;
      //  ConfigManager config = GameEngine.Instance.GetManager<ConfigManager>();
        procedure = procedureOwner;
        EventManager.Instance.Register(EventType.EndLoadConfig, EndLoadConfig);
    //    config.LoadConfig();
    }
    protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);  
        ChangeState<ProcedureChangeScene>(procedureOwner);
    }
    public void EndLoadConfig()
    {
        ChangeState<ProcedureChangeScene>(procedure);

    }
    protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        EventManager.Instance.Remove(EventType.EndLoadConfig, EndLoadConfig);
    }
}
