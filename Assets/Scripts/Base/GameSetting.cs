using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetting : MonoSingleton<GameSetting>
{
    public ProcedureSetting procedureSetting;
    public ProcedureBase CurrentProcedure;
    public RectTransform uiRoot;
    public RectTransform panelRoot;
    public Camera UICamera;
    public EventSystem EventSystem;

    public GameSetting Init()
    {
        RectTransform uiRoot = transform.Find("UIRoot") as RectTransform;
        panelRoot = transform.Find("UIRoot/WndRoot") as RectTransform;
        UICamera = transform.Find("UIRoot/UICamera").GetComponent<Camera>();
        EventSystem= transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>();
        return this;
     
    }
    //public ProcedureBase GetBeginProcedure()
    // {
    //     switch (procedureSetting.procedureName[0])
    //     {
    //         case ProcedureType.Start:             
    //             return (ProcedureBase)Assembly.Load(Assembly.GetExecutingAssembly().GetName()).CreateInstance("ProcedureExample");               
    //     }
    //     return null;
    // }
    private void Start()
    {
       
    }
  
}
 