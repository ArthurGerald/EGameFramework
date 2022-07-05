using ProcedureOwner = IFsm<IProcedureManager>;
/// <summary>
/// 切换场景的流程，在流程内对场景切换进行操作，进行资源的管理，真正的场景通过GameMapManager进行加载
/// </summary>
    public class ProcedureChangeScene : ProcedureBase
    {
    bool isChangeEnd;
    protected internal override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        isChangeEnd = false;
        //GameMapManager gameMap = GameEngine.Instance.GetManager<GameMapManager>();
        GameEngine.Instance.GetManager<GameMapManager>().LoadScene("FightScene", new object[] { true });
        EventManager.Instance.Register(EventType.EndChangeScene, EndChangeScene);
        
    }
    protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        if(isChangeEnd)
        ChangeState<ProcedureFightScene>(procedureOwner);
    }
    public void EndChangeScene()
    {
        isChangeEnd = true;
        //ChangeState<ProcedureMenu>(procedureOwner);
    }
    protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        EventManager.Instance.Remove(EventType.EndChangeScene, EndChangeScene);
    }

}

