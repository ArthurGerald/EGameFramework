using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager: IBaseManager
{
    protected GameEngine engine;
    protected GameSetting setting;
    public virtual void OnInit(GameEngine gameEngine)
    {
        this.engine = gameEngine;
    }
    public virtual void OnRegisterSetting(GameSetting gameSetting)
    {
        this.setting = gameSetting;
    }
    /// <summary>
    /// 获取游戏框架模块优先级。
    /// </summary>
    /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
    internal virtual int Priority
    {
        get
        {
            return 0;
        }
    }

    /// <summary>
    /// 游戏框架模块轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
   // public virtual void Update(float elapseSeconds, float realElapseSeconds);

    /// <summary>
    /// 关闭并清理游戏框架模块。
    /// </summary>
    public virtual void OnShutdown() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
    public virtual void OnDestroy() { }
}
