using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = IFsm<IProcedureManager>;
public class ProcedureFightScene : ProcedureBase
{
    protected internal override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.LogError("77777");
        UIManager uIManager = GameEngine.Instance.GetManager<UIManager>();
        uIManager.ShowPanel<TestPanelBase>();
        uIManager.Teset();

    }
   
}
