using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControl : IController
{
    public void Init()
    {
        Debug.Log("注册UI");
        UIManager uIManager = GameEngine.Instance.GetManager<UIManager>();
        uIManager.Register<TestPanelBase>("TestPanel");
    }
}
