using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseManager 
{
      void OnInit(GameEngine gameEngine);
      void OnRegisterSetting(GameSetting gameSetting);
  
    /// <summary>
    /// 游戏框架模块轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    // public virtual void Update(float elapseSeconds, float realElapseSeconds);
    /// <summary>
    /// 关闭并清理游戏框架模块。
    /// </summary>
    void OnShutdown();
    void OnStart();
    void OnUpdate(float elapseSeconds, float realElapseSeconds);
    void OnDestroy();
}
