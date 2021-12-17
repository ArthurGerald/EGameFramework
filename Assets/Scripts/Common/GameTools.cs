using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public  class GameTools : Singleton<GameTools>
{

    private  readonly string[] RuntimeAssemblyNames =
       {
#if UNITY_2017_3_OR_NEWER
            "UnityGameFramework.Runtime",
#endif
            "Assembly-CSharp",
        };

    private  readonly string[] RuntimeOrEditorAssemblyNames =
      {
#if UNITY_2017_3_OR_NEWER
            "UnityGameFramework.Runtime",
#endif
            "Assembly-CSharp",
#if UNITY_2017_3_OR_NEWER
            "UnityGameFramework.Editor",
#endif
            "Assembly-CSharp-Editor",
        };
    public  string[] GetRuntimeTypeNames(System.Type typeBase)
    {
        return GetTypeNames(typeBase, RuntimeAssemblyNames);
    }
    

    //public Type GetTypeByName(string typeName)
    //{
    //    Assembly.Load(assemblyName).get
    //}
     
    /// <summary>
    /// 在运行时或编辑器程序集中获取指定基类的所有子类的名称。
    /// </summary>
    /// <param name="typeBase">基类类型。</param>
    /// <returns>指定基类的所有子类的名称。</returns>
    public  string[] GetRuntimeOrEditorTypeNames(System.Type typeBase)
    {
        return GetTypeNames(typeBase, RuntimeOrEditorAssemblyNames);
    }

    public  string[] GetTypeNames(System.Type typeBase, string[] assemblyNames)
    {
        List<string> typeNames = new List<string>();
        foreach (string assemblyName in assemblyNames)
        { 
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                continue;
            }

            if (assembly == null)
            {
                continue;
            }

            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }
        }

        typeNames.Sort();
        return typeNames.ToArray();
    }
}

