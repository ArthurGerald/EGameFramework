using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
public class LanguageSwitch 
{
    [MenuItem("语言切换/中文",false,10)]
    public static void ChangeCn()
    {
        LanguagePlayerPrefs.Language = LanguageType.CN;
        SetLanguage();
    }
    [MenuItem("语言切换/英文", false, 10)]
    public static void ChangeEn()
    {
        LanguagePlayerPrefs.Language = LanguageType.EN;
        SetLanguage();
    }

    private static void SetLanguage()
    {
        if (LanguageManager.OnLocalize != null)
        {
            LanguageManager.OnLocalize();
        }

        if(!Application.isPlaying)
        RefreshPrefab();
    }
    [MenuItem("语言切换/英文",true)]
    static bool ENToggle()
    {
        Menu.SetChecked("语言切换/英文", LanguagePlayerPrefs.Language == LanguageType.EN);
        return true;
    }
    [MenuItem("语言切换/中文", true)]
    static bool CNToggle()
    {
        Menu.SetChecked("语言切换/中文", LanguagePlayerPrefs.Language == LanguageType.CN);
        return true;
    }
    /// <summary>
    /// 静态刷新预制体
    /// </summary>
    public static void RefreshPrefab()
    {
        //object[] obj = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        List<GameObject> prefabs = GetAllPrefab();
        for (int i = 0; i < prefabs.Count; i++)
        {
            //string ext = System.IO.Path.GetExtension(obj[i].ToString());
            //if (!ext.Contains(".GameObject"))
            //{
            //    continue;
            //}
            //GameObject go = (GameObject)obj[i];
            EditorUtility.DisplayProgressBar("刷新预制体", "刷新预制体中..", (float)i / prefabs.Count);
            foreach (Transform trans in prefabs[i].GetComponentsInChildren<Transform>(true))
            {
                LanguageText languageText = trans.GetComponent<LanguageText>();
                Text text = trans.GetComponent<Text>();
                if (text != null && languageText != null)
                    languageText.Localize();
            }
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
        RefreshScenePrefab();
    }
    public static List<GameObject> GetAllPrefab()
    {
        List<GameObject> prefabs = new List<GameObject>();
        var resourcePath = Application.dataPath;
        var absoultePaths = System.IO.Directory.GetFiles(resourcePath, "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i<absoultePaths.Length;i++)
        {
            EditorUtility.DisplayProgressBar("获取项目预制体", "获取预制体中..", (float)i / absoultePaths.Length);
            string path = "Assets" + absoultePaths[i].Remove(0, resourcePath.Length);
            path = path.Replace("\\","/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab != null)
                prefabs.Add(prefab);
        }
        EditorUtility.ClearProgressBar();
        return prefabs;
    }
    /// <summary>
    /// 刷新场景中的预制体
    /// </summary>
   public static void RefreshScenePrefab()
    {
        LanguageText[] languageTexts = GameObject.FindObjectsOfType<LanguageText>();
        for (int i = 0; i < languageTexts.Length; i++)
        {
            languageTexts[i].Localize();
            EditorUtility.SetDirty(languageTexts[i]);
        }
    }
}
