using Entity;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ConfigName
{
    LanguageInfo
}
public class InfoBase 
{
    //表id
    public int baseId;
}

public class ConfigManager : Singleton<ConfigManager>
{
    private readonly string CONFIGS_PATH_RESOURCES_DIR = "Json/";
    private Dictionary<ConfigName, Dictionary<int, InfoBase>> dic_infos = new Dictionary<ConfigName, Dictionary<int, InfoBase>>();
    private GameEngine m_Mono;
    /// <summary>
    /// 读取本地 StreamingAssets文件夹下的 JSon文件
    /// </summary>
    public List<languageInfo> ReadJsonFromSSPath()
    {
        StreamReader streamReader = new StreamReader(Application.dataPath + "/Json/" + "language" + ".json");
        string str = streamReader.ReadToEnd();
        Debug.LogError(str);
        JsonData m_jMoneyData = JsonMapper.ToObject(str);
        List<languageInfo> info = JsonMapper.ToObject<List<languageInfo>>(str);
        return info;
    }

    public void Test<T>(ConfigName confimName)
    {
     
    }

    /// <summary>
    /// 解析 Json 字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public JsonData ReadJsonFromStr(string str)
    {
        JsonData jsonData = JsonMapper.ToObject(str);
        return jsonData;
    }

    public void LoadConfig()
    {
        Debug.LogError("ddd");
        m_Mono.StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        List<IEnumerator> enumerators = new List<IEnumerator>();
        enumerators.Add(InitInfoAsync<languageInfo>(ConfigName.LanguageInfo, "language"));
        foreach (var item in enumerators)
        {
            yield return item;
        }
        EventManager.Instance.Send(EventType.EndLoadConfig);
    }

    public void OnInit(GameEngine gameEngine)
    {
        m_Mono = gameEngine;
    }
    IEnumerator InitInfoAsync<T>(ConfigName infoName, string name) where T : InfoBase
    {
        StreamReader streamReader = new StreamReader(Application.dataPath + "/Json/" + name + ".json");
        string str = streamReader.ReadToEnd();
        Debug.LogError(str);
        JsonData m_jMoneyData = JsonMapper.ToObject(str);
        List<T> infoList = JsonMapper.ToObject<List<T>>(str);
        Dictionary<int, InfoBase> dic = new Dictionary<int, InfoBase>();
        for (int i = 0; i < infoList.Count; i++)
        {
            if (infoName == ConfigName.LanguageInfo)
            {
                languageInfo info = infoList[i] as languageInfo;
            }
            dic[infoList[i].baseId] = infoList[i];
        }
        dic_infos.Add(infoName, dic);
        yield return null ;

    }
    public List<T> GetInfoItems<T>(ConfigName name) where T : InfoBase
    {
        List<T> list = new List<T>();
        if (dic_infos.ContainsKey(name))
            foreach (KeyValuePair<int, InfoBase> kvp in dic_infos[name])
            {
                list.Add((T)kvp.Value);
            }
        return list;
    }

    public void OnRegisterSetting(GameSetting gameSetting)
    {
        
    }

    public void OnShutdown()
    {
       
    }

    public void OnStart()
    {
        
    }

    public void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {

    }

    public void OnDestroy()
    {
       
    }
}
