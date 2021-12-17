using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ProcedureSetting", menuName = "CreateProcedureSetting", order = 0)]
public class ProcedureSetting : ScriptableObject
{
    public List<string> procedureName = new List<string>();   
}
[System.Serializable]
public enum ProcedureType
{
    Start=1
}