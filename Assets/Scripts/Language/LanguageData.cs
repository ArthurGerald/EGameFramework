using System.Collections.Generic;
using UnityEngine;

public class LanguageData 
{
    static public string Get(string key)
    {
        List<languageInfo> data = ConfigManager.Instance.ReadJsonFromSSPath();
        languageInfo selscetData = null;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                selscetData = data[i];
            }
        }
        if (selscetData != null)
        {
            if (LanguagePlayerPrefs.Language == LanguageType.CN)
            {
                return selscetData.cn;
            }
            if (LanguagePlayerPrefs.Language == LanguageType.EN)
            {
                return selscetData.en;
            }
        }
        return "未赋值";
    }
    static public Sprite GetImg(string key)
    {
        List<languageInfo> data = ConfigManager.Instance.ReadJsonFromSSPath();
        languageInfo selscetData = null;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                selscetData = data[i];
            }
        }
        if (selscetData != null)
        {
            if (LanguagePlayerPrefs.Language == LanguageType.CN)
            {
                return null;
            }
            if (LanguagePlayerPrefs.Language == LanguageType.EN)
            {
                return null;
            }
        }
        return null;
    }
}
public enum LanguageType
{
    Default,
    CN,
    EN
}

public static class LanguagePlayerPrefs
{
    private static LanguageType m_Language = LanguageType.Default;
    public static LanguageType Language
    {
        get
        {
            if (m_Language == LanguageType.Default)
            {
                m_Language = (LanguageType)PlayerPrefs.GetInt("LanguageSetting", (int)LanguageType.CN);
            }
            return m_Language;
        }
        set
        {
            m_Language = value;
            PlayerPrefs.SetInt("LanguageSetting", (int)m_Language);
        }
    }
}
