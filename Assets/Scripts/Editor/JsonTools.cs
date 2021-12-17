using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using LitJson;
using System.Text;
using UnityEditor;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

public class JsonTools : EditorWindow
{
    private static string txt_ExcelName = "tab1.xlsx"; //表格名称
    private static string txt_JsonSavePath = "Json";
    private static string txt_EntitySavePath = "Entity";
    private Vector2 scrollPos;
    [MenuItem("Tools/ExcelToJson")]
    static void OpenWindow()
    {
        JsonTools window = (JsonTools)EditorWindow.GetWindow(typeof(JsonTools));
        window.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("表格名称: ");
        txt_ExcelName = GUILayout.TextField(txt_ExcelName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Json保存路径: ");
        txt_JsonSavePath = GUILayout.TextField(txt_JsonSavePath);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("生成实体类"))
        {
            CreateEntities(txt_ExcelName);
        }
        if (GUILayout.Button("生成Json"))
        {
            ExcelToJson(txt_ExcelName);
        }
        EditorGUILayout.EndScrollView();
    }

    private static void ExcelToJson(string _ExcelName)
    {
        if (!string.IsNullOrEmpty(_ExcelName))
        {
            string filepath = Application.dataPath + "/Excel/" + _ExcelName;
            string headPath = $"{Application.dataPath}/{txt_JsonSavePath}";
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    //获得所有工作表
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    List<System.Object> lst = new List<object>();

                    //遍历所有工作表
                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        //当前工作表 
                        ExcelWorksheet sheet = workSheets[i];
                        //初始化集合
                        lst.Clear();
                        int columnCount = sheet.Dimension.End.Column;
                        int rowCount = sheet.Dimension.End.Row;
                        //根据实体类创建对象集合序列化到json中
                        for (int z = 4; z <= rowCount; z++)
                        {
                            Assembly ab = Assembly.Load("Assembly-CSharp"); //要注意对面在那个程序集里面dll
                            Type type = ab.GetType($"Entity.{sheet.Name}Info");
                            if (type == null)
                            {
                                Debug.LogError("你还没有创建对应的实体类!");
                                return;
                            }
                            if (!Directory.Exists(headPath))
                                Directory.CreateDirectory(headPath);
                            object o = ab.CreateInstance(type.ToString());
                            for (int j = 1; j <= columnCount; j++)
                            {
                                FieldInfo fieldInfo = type.GetField(sheet.Cells[i, j].Text); //先获得字段信息，方便获得字段类型
                                object value = Convert.ChangeType(sheet.Cells[z, j].Text, fieldInfo.FieldType);
                                type.GetField(sheet.Cells[1, j].Text).SetValue(o, value);
                            }
                            lst.Add(o);
                        }
                        //写入json文件
                        string jsonPath = $"{headPath}/{sheet.Name}.json";
                        if (!File.Exists(jsonPath))
                        {
                            File.Create(jsonPath).Dispose();
                        }
                        
                        string str = JsonMapper.ToJson(lst);
                        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
                        str = reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
                        File.WriteAllText(jsonPath, str);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
    [MenuItem("Tools/全部Excel转Json")]
    public static void AllExcelToJson()
    {
        string RegPath = Application.dataPath + "/Excel/";
        string[] filePaths = Directory.GetFiles(RegPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (!filePaths[i].EndsWith(".xlsx"))
                continue;
            EditorUtility.DisplayProgressBar("查找文件夹下的类", "正在扫描路径" + filePaths[i] + "... ...", 1.0f / filePaths.Length * i);
            string path = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1);
            ExcelToJson(path);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Tools/全部Excel转Object")]
    public static void AllExcelToObject()
    {
        string RegPath = Application.dataPath + "/Excel/";
        string[] filePaths = Directory.GetFiles(RegPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (!filePaths[i].EndsWith(".xlsx"))
                continue;
            EditorUtility.DisplayProgressBar("查找文件夹下的类", "正在扫描路径" + filePaths[i] + "... ...", 1.0f / filePaths.Length * i);
            string path = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1);
            CreateEntities(path);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
   static void CreateEntities(string _ExcelName)
    {
        if (!string.IsNullOrEmpty(_ExcelName))
        {
            string filepath = Application.dataPath + "/Excel/" + _ExcelName;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    //获得所有工作表
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    //遍历所有工作表
                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        CreateEntity(workSheets[i]);
                    }
                    AssetDatabase.Refresh();
                }
            }
        }
    }
   static void CreateEntity(ExcelWorksheet sheet)
    {
        string dir = $"{Application.dataPath}/{txt_EntitySavePath}";
        string path = $"{dir}/{sheet.Name}Info.cs";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("namespace Entity");
        sb.AppendLine("{");
        sb.AppendLine($"\tpublic class {sheet.Name}Info");
        sb.AppendLine("\t{");
        //遍历sheet首行每个字段描述的值
     
        for (int i = 1; i <= sheet.Dimension.End.Column; i++)
        {
            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine($"\t\t///{sheet.Cells[3, i].Text}");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendLine($"\t\tpublic {sheet.Cells[2, i].Text} {sheet.Cells[1, i].Text};");
        }
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        try
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose(); //避免资源占用
            }
            File.WriteAllText(path, sb.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Excel转json时创建对应的实体类出错，实体类为：{sheet.Name}Info,e:{e.Message}");
        }
    }

}