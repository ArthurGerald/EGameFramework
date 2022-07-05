using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using UnityEditor;



#if UNITY_EDITOR
public class UIBase
{
    static string m_className = "";
    static Transform m_SelectTrans;
    static string m_Path = "";
    const string csDirPath = "/UI/Panel/";
    const string genDir = "Assets/Scripts"; //代码生成根节点
    //difine 
    private static List<string> defineMember = new List<string>();
    private static void CreateNewCSharp(Transform trans)
    {
        //对应path
        UnityEngine.Object[] obj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        string prefabPath = AssetDatabase.GetAssetPath(obj[0]);
        Debug.Log($"prefabPath :{prefabPath}");

        if (prefabPath.IndexOf("Assets/Prefabs/UIPrefabs/") == 0)
        {
            m_className = Path.GetFileNameWithoutExtension(prefabPath);
            m_Path = csDirPath + m_className + "Base.cs";
            Debug.Log($"genPath :{ m_Path}");
        }
        else
        {
            return;
        }

        FileInfo file = new System.IO.FileInfo(genDir + m_Path);
        file.Directory.Create(); // If the directory already exists, this method does nothing.
        try
        {
            if (m_className.EndsWith("Panel"))
                CheckCS("BasePanel");
            else if (m_className.EndsWith("Container"))
                CheckCS("BaseContainer");
            else if (m_className.EndsWith("Section"))
            {
                CheckCS("BaseSection");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return;
        }

        AssetDatabase.Refresh();
    }

    static string[] m_PreMustObj = { "Btn", "Tgl", "Sld", "Ifd" };
    static Dictionary<string, Transform> m_NeedObjs = new Dictionary<string, Transform>();
    static List<string> m_MustObj = new List<string>();
    static List<Transform> m_DycObj = new List<Transform>();
    static List<string> m_FuncAdd = new List<string>();
    static List<string> m_StaticAdd = new List<string>();
    static Dictionary<Transform, string> m_DicDyc = new Dictionary<Transform, string>();
    static Dictionary<string, List<string>> m_DicScvRef = new Dictionary<string, List<string>>();
    static string m_preDymic = "Dmc";
    static string m_preStatic = "Stc";
    static string m_preSection = "Sct";
    static string[] m_ScrollHand = { "Lsv", "Lsh" };
    static string[] m_PreObjectExt = { "Img", "Txt","Tmp" };
    static string m_Prefix = "_";
    static List<string> m_UseObject = new List<string>();
    static StringBuilder m_TempfunStr;
    static StringBuilder m_DstText;

    static void CheckOnInit()
    {
        m_DstText.AppendLine("\t\t\t\tUnityEngine.GameObject go = gameObject;");      
        foreach (var Dic in m_NeedObjs)
        {
            string preTemp = Dic.Key;
            Transform trans = Dic.Value;
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            Debug.Log(m_Prefix + m_PreMustObj[0]);
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Button>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}.onClick.AddListener(OnClick{0});", ObjName));
                defineMember.Add($"UnityEngine.UI.Button _{ObjName};");
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Toggle>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}.onValueChanged.AddListener(On{0});", ObjName));
                defineMember.Add($"UnityEngine.UI.Toggle _{ObjName};");
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t _{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Slider>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}.onValueChanged.AddListener(On{0});", ObjName));
                defineMember.Add($"UnityEngine.UI.Slider _{ObjName};");
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.InputField>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}.onValueChanged.AddListener(On{0});", ObjName));
                defineMember.Add($"UnityEngine.UI.InputField _{ObjName};");
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[0]))
            {               
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Image>();", ObjName, preTemp));
                defineMember.Add($"UnityEngine.UI.Image _{ObjName};");
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[1]))
            {             
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Text>();", ObjName, preTemp));
                defineMember.Add($"UnityEngine.UI.Text _{ObjName};");

            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<TMPro.TMP_Text>();", ObjName, preTemp));
                defineMember.Add($"TMPro.TMP_Text _{ObjName};");              
            }
            else if (trans.name.StartsWith(m_Prefix + m_ScrollHand[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t _{0} = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
                defineMember.Add($"UnityEngine.GameObject _{ObjName};");
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}TempCache = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, System.Object>>();", ObjName));
                defineMember.Add($"System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, System.Object>> _{ObjName}TempCache;");
                m_DstText.AppendLine(string.Format("\t\t\t\t_TempCache.Add(_TempCache.Count,_{0}TempCache);", ObjName));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}Scroll =_{0}.GetComponent<UI.CLoopScrollRect>();", ObjName));
                defineMember.Add($"UI.CLoopScrollRect _{ObjName}Scroll;");

            }
            else if (trans.name.StartsWith(m_Prefix + m_ScrollHand[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = go.transform.Find(\"{1}\")", ObjName, preTemp));
                defineMember.Add($"UnityEngine.GameObject  _{ObjName};");
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}TempCache =new  System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, System.Object>>();", ObjName));
                defineMember.Add($"System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, System.Object>>  _{ObjName}TempCache;");
                m_DstText.AppendLine(string.Format("\t\t\t\t_TempCache.Add(_TempCache.Count_{0}TempCache)", ObjName));
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0}Scroll =_{0}.GetComponent<UI.CLoopScrollRect>();)", ObjName));
                defineMember.Add($"UI.CLoopScrollRect _{ObjName}Scroll;");
            }
            else
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
                defineMember.Add($"UnityEngine.GameObject _{ObjName};");
            }

        }
    }

    static void CheckPreObject()
    {
        foreach (var preTemp in m_UseObject)
        {
            Transform trans = m_NeedObjs[preTemp];
            string[] tempNames = preTemp.Split('/');
            int len = tempNames.Length;
            string ObjName = "";
            if (len > 1)
            {
                ObjName = tempNames[len - 2] + "_" + tempNames[len - 1];
            }
            else
            {
                ObjName = tempNames[len - 1];
            }

            ObjName = ObjName.Replace(m_Prefix, "");

            if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[0]))
            {
                m_TempfunStr.AppendLine(string.Format("\t\t\t\tUnityEngine.UI.Image _{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Image>();", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[1]))
            {
                m_TempfunStr.AppendLine(string.Format("\t\t\t\tUnityEngine.UI.Text _{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Text>();", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} =go.transform.Find(\"{1}\").gameObject.GetComponent<TMPro.TMP_Text>();", ObjName, preTemp));
                defineMember.Add($"TMPro.TMP_Text _{ObjName};");
            }
            else
            {
                m_TempfunStr.AppendLine(string.Format("\t\t\t\tUnityEngine.GameObject _{0} = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
            }
        }
    }

    #region Dymic
    static void CheckOnDymicInit(string name)
    {
        string dynamicName = name.Replace(m_Prefix, "");
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic UnityEngine.GameObject Clone{1}(UnityEngine.GameObject parent)", m_className, dynamicName));
        m_DstText.AppendLine(string.Format("\t\t\tvar item = {{}}"));
        // m_DstText.AppendLine(string.Format("\tself[\"_{0}\"..id].id = id", dynamicName));
        m_DstText.AppendLine(string.Format("\titem.go = UIUtil:cloneTemplate(self._{0}, parent, true)", dynamicName));
        m_DstText.AppendLine(string.Format("\tlocal go = item.go"));

        foreach (var Dic in m_NeedObjs)
        {
            string preTemp = Dic.Key;
            Transform trans = Dic.Value;
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getButton(go, \"{1}\", self.OnClick{0}, {{self ,item}})", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getToggle(go, \"{1}\", self.On{0}, {{self ,item}})", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getSlider(go, \"{1}\", self.On{0}, {{self ,item}})", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getInputField(go, \"{1}\", self.On{0},  {{self ,item}})", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[0]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getImage(go, \"{1}\", self)", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[1]))
            {

                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getText(go, \"{1}\" ,self)", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[2]))
            {             
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getTMP_Text(go, \"{1}\" ,self)", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_ScrollHand[0]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getObject(go, \"{1}\")", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\titem.{0}TempCache = {{ }}", ObjName));
                m_DstText.AppendLine(string.Format("\titem.{0}Scroll =UIUtil:getCLoopScrollRect(item.{0},\"\")", ObjName));

            }
            else if (trans.name.StartsWith(m_Prefix + m_ScrollHand[1]))
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getObject(go, \"{1}\")", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\titem.{0}TempCache = {{ }}", ObjName));
                m_DstText.AppendLine(string.Format("\titem.{0}Scroll =UIUtil:getCLoopScrollRect(item.{0},\"\")", ObjName));

            }
            else
            {
                m_DstText.AppendLine(string.Format("\titem.{0} = UIUtil:getObject(go, \"{1}\")", ObjName, preTemp));
            }

        }
        m_DstText.AppendLine("\treturn item");
        m_DstText.AppendLine("end");
    }

    private static void CheckDymicMust()
    {
        foreach (var must in m_MustObj)
        {
            Transform trans = m_NeedObjs[must];
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");

            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                CheckDymicBtn(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                CheckDymicToggle(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                CheckDymicSlider(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                CheckDymicInputField(ObjName);
            }
        }
    }

    static void CheckOnStaticInit(string name)
    {
        string dynamicName = name.Substring(1);
        dynamicName = dynamicName.Split('_')[0];
        if (m_StaticAdd.Contains(dynamicName))
        {
            return;
        }
        else
        {
            m_StaticAdd.Add(dynamicName);
        }
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic System.Collections.Generic.Dictionary<string, System.Object> Generate{0}(bool isCopy,UnityEngine.GameObject srcObj,UnityEngine.Transform parent)", dynamicName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tSystem.Collections.Generic.Dictionary<string, System.Object> item = new System.Collections.Generic.Dictionary<string, System.Object>();"));
        m_DstText.AppendLine(string.Format("\t\t\t\tvar go = isCopy ? UnityEngine.GameObject.Instantiate(srcObj) : srcObj;"));
        m_DstText.AppendLine("\t\t\t\tif (isCopy) {go.transform.SetParent(parent ==null ? srcObj.transform.parent:parent.transform);}");
        m_DstText.AppendLine(string.Format("\t\t\t\titem[\"go\"] = go;"));
        foreach (var Dic in m_NeedObjs)
        {
            string preTemp = Dic.Key;
            Transform trans = Dic.Value;
            string ObjName = trans.name;

            if (preTemp.Equals(m_Prefix + m_preStatic))
            {
                preTemp = "";
                ObjName = ObjName.Substring(4).Split('_')[0];

            }
            else
            {
                ObjName = ObjName.Substring(1);
            }
            if (ObjName.StartsWith(m_PreMustObj[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Button>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\t((UnityEngine.UI.Button)item[\"BtnButton\"]).onClick.AddListener(OnClick{0});", ObjName));
            }
            else if (ObjName.StartsWith(m_PreMustObj[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Toggle>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (ObjName.StartsWith(m_PreMustObj[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Slider>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] .onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (ObjName.StartsWith(m_PreMustObj[3]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.InputField>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (ObjName.StartsWith(m_PreObjectExt[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Image>()", ObjName, preTemp));
            }
            else if (ObjName.StartsWith(m_PreObjectExt[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Text>()", ObjName, preTemp));
            }
            else if (ObjName.StartsWith(m_PreObjectExt[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<TMPro.TMP_Text>()", ObjName, preTemp));
            }
            else if (ObjName.StartsWith(m_ScrollHand[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}TempCache\"] =new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>>();", ObjName));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}Scroll\"] =item[\"{0}\"].GetComponent<UI.CHorizontalLoopScrollRect>();", ObjName));
            }
            else if (ObjName.StartsWith(m_ScrollHand[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}TempCache\"] = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>>();", ObjName));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}Scroll\"] =item[\"{0}\"].GetComponent<UI.CLoopScrollRect>();", ObjName));
            }
            else if (!preTemp.Equals(""))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
            }

        }

        m_DstText.AppendLine("\t\t\t\treturn item;");
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckStaticMust()
    {
        foreach (var must in m_MustObj)
        {
            Transform trans = m_NeedObjs[must];
            string ObjName = trans.name;
            if (must.Equals(m_Prefix + m_preStatic))
            {
                ObjName = ObjName.Substring(4).Split('_')[0];
            }
            else
            {
                ObjName = ObjName.Substring(1);
            }

            if (m_FuncAdd.Contains(ObjName))
            {
                return;
            }
            else
            {
                m_FuncAdd.Add(ObjName);
            }

            if (ObjName.StartsWith(m_PreMustObj[0]))
            {
                CheckDymicBtn(ObjName);
            }
            else if (ObjName.StartsWith(m_PreMustObj[1]))
            {
                CheckDymicToggle(ObjName);
            }
            else if (ObjName.StartsWith(m_PreMustObj[2]))
            {
                CheckDymicSlider(ObjName);
            }
            else if (ObjName.StartsWith(m_PreMustObj[3]))
            {
                CheckDymicInputField(ObjName);
            }
        }
    }

    private static void CheckDymicBtn(string btnName)
    {
        // btn func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual  void OnClick{1}()", m_className, btnName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:OnClick{1} not implemented.\");", m_className, btnName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckDymicToggle(string toggleName)
    {
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(bool isSelected)", m_className, toggleName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, toggleName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckDymicSlider(string sliderName)
    {
        // slider func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(float curValue)", m_className, sliderName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, sliderName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckDymicInputField(string inputFieldName)
    {
        // InputField func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(string curValue)", m_className, inputFieldName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, inputFieldName));
        m_DstText.AppendLine("\t\t\t}");
    }
    #endregion


    #region ScollLoopView
    static void CheckScollCom()
    {
        foreach (var scvRef in m_DicScvRef)
        {
            string dynamicName = scvRef.Key.Replace(m_Prefix, "");
            List<string> templates = scvRef.Value;
            m_DstText.AppendLine();
            if (templates.Count == 1)
            {
                m_DstText.AppendLine(string.Format("\t\tpublic void Update{0}(int count ,int startIndex)", dynamicName));
                m_DstText.AppendLine("\t\t\t{");
                m_DstText.AppendLine(string.Format("\t\t\t\tvar _scroll = _{0}Scroll;", dynamicName));
                m_DstText.AppendLine(string.Format("\t\t\t\tvar tempCache = _{0}TempCache;", dynamicName));
                m_DstText.AppendLine(string.Format("\t\t\t\tUIUtil.SetComponentScrollView(_scroll, count, (go,index) =>"));
                m_DstText.AppendLine("\t\t\t\t{");
                m_DstText.AppendLine("\t\t\t\t\tint tStartIndex = go.name.IndexOf(\"(\");");
                m_DstText.AppendLine("\t\t\t\t\tint tEndIndex = go.name.IndexOf(\")\");");
                m_DstText.AppendLine("\t\t\t\t\tint tLength = tEndIndex - tStartIndex - 1;");
                m_DstText.AppendLine("\t\t\t\t\tint tIndex = int.Parse(go.name.Substring(tStartIndex + 1, tLength));");
                m_DstText.AppendLine("\t\t\t\t\tvar tempItem = tempCache[tIndex];");
                m_DstText.AppendLine("\t\t\t\t\ttIndex++;");
                m_DstText.AppendLine("\t\t\t\t\tif(tempItem==null)");
                m_DstText.AppendLine("\t\t\t\t\t{");
                m_DstText.AppendLine(string.Format("\t\t\t\t\t\ttempItem = Init{0}Item{1}(go, index + 1);", dynamicName, templates[0]));
                m_DstText.AppendLine(string.Format("\t\t\t\t\t\ttempCache[tIndex] = tempItem;"));
                m_DstText.AppendLine("\t\t\t\t\t}");
                m_DstText.AppendLine("\t\t\t\t\telse");
                m_DstText.AppendLine("\t\t\t\t\t{");
                m_DstText.AppendLine("\t\t\t\t\ttempItem[\"index\"] = index + 1;");
                m_DstText.AppendLine("\t\t\t\t\t}");
                m_DstText.AppendLine(string.Format("\t\t\t\t\t\tUpdate{0}Item(tempItem);", dynamicName));
                m_DstText.AppendLine("\t\t\t\t\t}, startIndex);");               
                m_DstText.AppendLine("\t\t\t}");
            }
            else
            {
                m_DstText.AppendLine(string.Format("\t\tpublic void Update{0}(int count ,int startIndex)", dynamicName));
                m_DstText.AppendLine("\t\t\t{");
                m_DstText.AppendLine(string.Format("\t\t\t\tvar _scroll = _{0}Scroll;", dynamicName));
                m_DstText.AppendLine(string.Format("\t\t\t\tvar tempCache = _{0}TempCache;", dynamicName));
                m_DstText.AppendLine("\t\t\t\t\tif (item==null)");
                m_DstText.AppendLine(string.Format("\t\t\t\t\t\t\t_{0}Templates = new System.Collections.Generic.Dictionary<string, System.Object>();", dynamicName));
                m_DstText.AppendLine("\telse");
                m_DstText.AppendLine(string.Format("\t\titem.{0}Templates = {{}}", dynamicName));
                m_DstText.AppendLine("\tend");
                m_DstText.AppendLine(string.Format("\tlocal Templates = item and item.{0}Templates or self._{0}Templates", dynamicName));

                for (int i = 0; i < templates.Count; i++)
                {
                    m_DstText.AppendLine(string.Format("\tTemplates[{0}] = \"{1}\"", i + 1, templates[i]));
                }
                m_DstText.AppendLine(string.Format("\tUIUtil:setComponentScrollView(_scroll, count, function (go, index)", dynamicName));
                m_DstText.AppendLine(string.Format("\t\tlocal _, _, tIndex = string.find(go.name, \"%((%d+)%)\")"));
                m_DstText.AppendLine("\t\ttIndex = tonumber(tIndex) + 1");
                m_DstText.AppendLine(string.Format("\t\tlocal tempItem = tempCache[tIndex]", dynamicName));
                m_DstText.AppendLine("\t\tif not tempItem then");
                foreach (var temp in templates)
                {
                    m_DstText.AppendLine(string.Format("\t\t\tif string.split(go.name,'_')[1] == \"{0}\" then", temp));
                    m_DstText.AppendLine(string.Format("\t\t\t\ttempItem = self:Init{0}Item{1}(go, index + 1);", dynamicName, temp));
                    m_DstText.AppendLine("\t\t\tend");
                }

                m_DstText.AppendLine(string.Format("\t\t\ttempCache[tIndex] = tempItem", dynamicName));
                m_DstText.AppendLine("\t\telse");
                m_DstText.AppendLine("\t\t\ttempItem.index = index + 1");

                m_DstText.AppendLine("\t\tend");
                m_DstText.AppendLine(string.Format("\t\tself:Update{0}Item(tempItem)", dynamicName));
                m_DstText.AppendLine("\t  end ,startIndex ,function (index)");
                m_DstText.AppendLine(string.Format("\t\treturn self:Select{0}Item(index + 1)", dynamicName));
                m_DstText.AppendLine("\t  end )");
                m_DstText.AppendLine("end");
                m_DstText.AppendLine();
                m_DstText.AppendLine(string.Format("function {0}:Select{1}Item(index)", m_className, dynamicName));
                m_DstText.AppendLine(string.Format("\terror(\"[UIPrefab]{0}:Select{1}Item  not implemented.\")", m_className, dynamicName));
                // m_DstText.AppendLine(string.Format("\t\treturn self._{0}Templates[1]", dynamicName));
                for (int i = 0; i < templates.Count; i++)
                {
                    m_DstText.AppendLine(string.Format("\tif index == {0} then", i + 1));
                    m_DstText.AppendLine(string.Format("\t\treturn \"{0}\"", templates[i]));
                    m_DstText.AppendLine("\tend");
                }
                m_DstText.AppendLine("\treturn \"\"");
                m_DstText.AppendLine("\t\t\t}");
            }
            m_DstText.AppendLine();
            m_DstText.AppendLine(string.Format("\t\tpublic void Refresh{0}(UnityEngine.EventSystems.UIBehaviour scroll)", dynamicName));
            m_DstText.AppendLine("\t\t\t{");
            m_DstText.AppendLine(string.Format("\t\t\t\tvar _scroll =_{0}Scroll;", dynamicName));
            m_DstText.AppendLine(string.Format("\t\t\t\t _scroll.RefreshCount();"));
            m_DstText.AppendLine("\t\t\t}");

            m_DstText.AppendLine();
            m_DstText.AppendLine(string.Format("\t\tpublic virtual void Update{0}Item(System.Collections.Generic.Dictionary<string, System.Object> item)", dynamicName));
            m_DstText.AppendLine("\t\t\t{");
            m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:Update{1}Item  not implemented.\");", m_className, dynamicName));
            m_DstText.AppendLine("\t\t\t}");
        }
        m_DicScvRef.Clear();
    }



    static void CheckItemInit(string ObjName, string preStr = "", string preTemp = "")
    {
        ObjName = ObjName.Replace(m_Prefix, "");
        if (ObjName.StartsWith(m_PreMustObj[0]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Button>();", preStr + ObjName, preTemp));
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onClick.AddListener(OnClick{0});", preStr + ObjName));
        }
        else if (ObjName.StartsWith(m_PreMustObj[1]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Toggle>();", preStr + ObjName, preTemp));
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", preStr + ObjName));
        }
        else if (ObjName.StartsWith(m_PreMustObj[2]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"]=go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Slider>();", preStr + ObjName, preTemp));
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", preStr + ObjName));
        }
        else if (ObjName.StartsWith(m_PreMustObj[3]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"]= go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.InputField>();", preStr + ObjName, preTemp));
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", preStr + ObjName));
        }
        else if (ObjName.StartsWith(m_PreObjectExt[0]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Image>();", preStr + ObjName, preTemp));
        }
        else if (ObjName.StartsWith(m_PreObjectExt[1]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Text>()", preStr + ObjName, preTemp));         
        }
        else if (ObjName.StartsWith(m_PreObjectExt[2]))
        {
            m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<TMPro.TMP_Text>()", preStr + ObjName, preTemp));
        }

    }
    static void CheckItemMust(string ObjName, string preStr = "")
    {
        ObjName = ObjName.Replace(m_Prefix, "");
        if (ObjName.StartsWith(m_PreMustObj[0]))
        {
            CheckScollBtn(preStr + ObjName);
        }
        else if (ObjName.StartsWith(m_PreMustObj[1]))
        {
            CheckScollToggle(preStr + ObjName);
        }
        else if (ObjName.StartsWith(m_PreMustObj[2]))
        {
            CheckScollSlider(preStr + ObjName);
        }
        else if (ObjName.StartsWith(m_PreMustObj[3]))
        {
            CheckScollInputField(preStr + ObjName);
        }
    }
    static void CheckOnScollInit(string name, string tempName)
    {
        string dynamicName = name.Replace(m_Prefix, "");
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic System.Collections.Generic.Dictionary<string, System.Object> Init{0}Item{1}(UnityEngine.GameObject go,int  index)", dynamicName, tempName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tSystem.Collections.Generic.Dictionary<string, System.Object> item = new System.Collections.Generic.Dictionary<string, System.Object>();"));
        m_DstText.AppendLine(string.Format("\t\t\t\titem[\"index\"] = index;"));
        m_DstText.AppendLine(string.Format("\t\t\t\titem[\"go\"] = go;"));
        m_DstText.AppendLine(string.Format("\t\t\t\titem[\"templateName\"] = \"{0}\";", tempName));
        m_DstText.AppendLine(string.Format("\t\t\t\t//Begin Template {0}Item{1}", dynamicName, tempName));
        string tempPara = tempName.Replace(m_Prefix, "").Substring(8);
        CheckItemInit(tempPara, "Template");
        foreach (var Dic in m_NeedObjs)
        {
            string preTemp = Dic.Key;
            Transform trans = Dic.Value;
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Button>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onClick.AddListener(OnClick{0});", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Toggle>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Slider>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"]= go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.InputField>();", ObjName, preTemp));
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"].onValueChanged.AddListener(On{0});", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Image>();", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[1]))
            {

                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<UnityEngine.UI.Text>();", ObjName, preTemp));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[2]))
            {

                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"] = go.transform.Find(\"{1}\").gameObject.GetComponent<TMPro.TMP_Text>();", ObjName, preTemp));
            }
            else
            {
                m_DstText.AppendLine(string.Format("\t\t\t\titem[\"{0}\"]= go.transform.Find(\"{1}\").gameObject;", ObjName, preTemp));
            }

        }
        m_DstText.AppendLine(string.Format("\t\t\t\t//End Template {0}Item{1}", dynamicName, tempName));
        m_DstText.AppendLine("\t\t\t\treturn item;");
        m_DstText.AppendLine("\t\t\t}");
        CheckItemMust(tempPara, "Template");
    }

    private static void CheckScollMust()
    {
        foreach (var must in m_MustObj)
        {
            Transform trans = m_NeedObjs[must];
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                CheckScollBtn(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                CheckScollToggle(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                CheckScollSlider(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                CheckScollInputField(ObjName);
            }
        }
    }

    private static void CheckScollBtn(string btnName)
    {
        // btn func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void OnClick{0}(GameObject item)", btnName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:OnClick{1} not implemented.\");", m_className, btnName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckScollToggle(string toggleName)
    {
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{0}(bool isSelected ,GameObject item)", toggleName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, toggleName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckScollSlider(string sliderName)
    {
        // slider func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{0}(float curValue ,GameObject item)", sliderName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, sliderName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckScollInputField(string inputFieldName)
    {
        // InputField func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{0}(float curValue ,object item)", inputFieldName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, inputFieldName));
        m_DstText.AppendLine("\t\t\t}");
    }
    #endregion
    private static void CheckMust()
    {
        foreach (var must in m_MustObj)
        {
            Transform trans = m_NeedObjs[must];
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                CheckBtn(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                CheckToggle(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                CheckSlider(ObjName);
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                CheckInputField(ObjName);
            }
        }
    }

    private static void CheckBtn(string btnName)
    {
        // btn func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void OnClick{1}()", m_className, btnName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:OnClick{1} not implemented.\");", m_className, btnName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckToggle(string toggleName)
    {
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(bool isSelected)", m_className, toggleName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, toggleName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckSlider(string sliderName)
    {
        // slider func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(float curValue)", m_className, sliderName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, sliderName));
        m_DstText.AppendLine("\t\t\t}");
    }

    private static void CheckInputField(string inputFieldName)
    {
        // InputField func
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void On{1}(string curValue)", m_className, inputFieldName));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tUnityEngine.Debug.LogError(\"[UIPrefab]{0}:On{1}  not implemented.\");", m_className, inputFieldName));
        m_DstText.AppendLine("\t\t\t}");
    }

    static string GetFullPath(Transform curTrans, Transform Target, Transform dymicTrans, out bool isAssign)
    {
        string fullPath = curTrans.name;
        Transform curParent = curTrans.parent;
        isAssign = true;
        while (!curParent.name.Equals(Target.name))
        {
            foreach (var dymic in m_DycObj)
            {
                if (curParent == dymic)
                {
                    isAssign = false;
                    return "";
                }
            }
            fullPath = curParent.name + "/" + fullPath;
            curParent = curParent.parent;
        }
        return fullPath;
    }

    static void CheckTransform(Transform selectTran)
    {
        Transform dynamicTrans = null;
        m_MustObj.Clear();
        m_UseObject.Clear();
        m_NeedObjs.Clear();
        m_DycObj.Clear();

        Transform[] transforms = selectTran.GetComponentsInChildren<Transform>(true);
        foreach (var temp in transforms)
        {
            bool isMust = false;
            bool isAssign = true;
            if (temp == selectTran)
            {
                if (temp.name.StartsWith(m_Prefix + m_preStatic))
                {
                    m_NeedObjs.Add(m_Prefix + m_preStatic, temp);
                    m_MustObj.Add(m_Prefix + m_preStatic);
                }
                continue;
            }

            if (temp.name.StartsWith(m_Prefix))
            {
                Debug.Log("transform:" + temp.name);
                foreach (var mustTemp in m_PreMustObj)
                {
                    if (temp.name.StartsWith(m_Prefix + mustTemp))
                    {
                        string fullPath = GetFullPath(temp, selectTran, dynamicTrans, out isAssign);
                        //UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "transformMust :", fullPath);
                        if (isAssign)
                        {
                            m_NeedObjs.Add(fullPath, temp);
                            m_MustObj.Add(fullPath);
                        }
                        isMust = true;
                        break;
                    }
                }
                if ((!isMust) && (temp.name.StartsWith(m_Prefix + m_preDymic)))
                {

                    string fullPath = GetFullPath(temp, selectTran, dynamicTrans, out isAssign);
                    //UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "transformMust :", fullPath);
                    if (isAssign)
                    {
                        m_NeedObjs.Add(fullPath, temp);
                        m_DycObj.Add(temp);
                        m_DicDyc.Add(temp, temp.name);

                    }
                    isMust = true;
                }
                if ((!isMust) && (temp.name.StartsWith(m_Prefix + m_preStatic)))
                {

                    string fullPath = GetFullPath(temp, selectTran, dynamicTrans, out isAssign);
                    Debug.Log($"transformMust :{fullPath}");
                    if (isAssign)
                    {
                        m_NeedObjs.Add(fullPath, temp);
                        m_DycObj.Add(temp);
                        m_DicDyc.Add(temp, temp.name);

                    }
                    isMust = true;
                }

                if ((!isMust) && (temp.name.StartsWith(m_Prefix + m_preSection)))
                {
                    string fullPath = GetFullPath(temp, selectTran, dynamicTrans, out isAssign);
                    //UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "transformMust :", fullPath);
                    if (isAssign)
                    {
                        m_NeedObjs.Add(fullPath, temp);
                        m_DycObj.Add(temp);
                        m_DicDyc.Add(temp, temp.name);
                    }
                    isMust = true;
                }

                if (!isMust)
                {
                    string fullPath = GetFullPath(temp, selectTran, dynamicTrans, out isAssign);
                    //UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "transform:", fullPath);
                    if (isAssign)
                    {
                        m_NeedObjs.Add(fullPath, temp);
                        m_UseObject.Add(fullPath);
                        for (int i = 0; i < m_ScrollHand.Length; i++)
                        {
                            var scroll = m_ScrollHand[i];
                            if (temp.name.StartsWith(m_Prefix + scroll))
                            {
                                //if (i == 0)
                                //{
                                //    XN_UIFramework.UI.CVerticalLoopScrollRect tempCom = temp.GetComponent<XN_UIFramework.UI.CVerticalLoopScrollRect>();
                                //    if (tempCom == null)
                                //    {
                                //        tempCom = temp.gameObject.AddComponent<XN_UIFramework.UI.CVerticalLoopScrollRect>();
                                //    }
                                //}
                                //else if (i == 1)
                                //{
                                //    XN_UIFramework.UI.CHorizontalLoopScrollRect tempCom = temp.GetComponent<XN_UIFramework.UI.CHorizontalLoopScrollRect>();
                                //    if (tempCom == null)
                                //    {
                                //        tempCom = temp.gameObject.AddComponent<XN_UIFramework.UI.CHorizontalLoopScrollRect>();
                                //    }

                                //}
                                //var loopCom = temp.GetComponent<LoopScrollRect>();
                                //if (loopCom != null)
                                //{
                                //    loopCom.enabled = false;
                                //    GameObject.DestroyImmediate(temp.gameObject.GetComponent<LoopScrollRect>(),true);
                                //}
                                //RectMask2D mask = temp.GetComponent<RectMask2D>();
                                //if (mask == null)
                                //{
                                //    temp.gameObject.AddComponent<RectMask2D>();
                                //}
                                int count = 0;
                                List<string> templates = new List<string>();
                                for (int j = 0; j < temp.childCount; j++)
                                {
                                    Transform tempTran = temp.GetChild(j);
                                    if (tempTran.name.Replace(m_Prefix, "").StartsWith("Template"))
                                    {
                                        m_DycObj.Add(tempTran);
                                        m_DicDyc.Add(tempTran, temp.name);
                                        templates.Add(tempTran.name);
                                        count++;
                                    }
                                }
                                if (count > 0)
                                {
                                    m_DicScvRef.Add(temp.name, templates);
                                }
                                else
                                {
                                    Debug.Log($"not find Template:{ temp.name}");
                                }
                            }
                        }
                    }
                }
            }
            // Debug.Log("transform:" + temp.name);
            // UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "transform:", temp.name);
        }
    }

    private static void CheckCS(string viewType)
    {
        m_DstText = new StringBuilder();
        m_DicDyc.Clear();
        m_FuncAdd.Clear();
        m_StaticAdd.Clear();
        defineMember.Clear();
       
        //namespace
        m_DstText.AppendLine();     
        m_DstText.AppendLine(string.Format("\tpublic class {0}Base : {1}", m_className,viewType));
        m_DstText.AppendLine("\t{");
        m_DstText.AppendLine();

       

        //onInitialize
        CheckTransform(m_SelectTrans);
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic void InitBind()"));
        m_DstText.AppendLine("\t\t\t{");
       // m_DstText.AppendLine(string.Format("\t\t\t\tbase.OnInitialize();"));
        CheckOnInit();
        m_DstText.AppendLine("\t\t\t\tif (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)");
        m_DstText.AppendLine("\t\t\t\t\tOnInitAndroid();");    
        m_DstText.AppendLine("\t\t\t\tif (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)");
        m_DstText.AppendLine("\t\t\t\t\tOnInitIos();");
        m_DstText.AppendLine("\t\t\t\t _InitState = true;");
        m_DstText.AppendLine("\t\t\t}");

        //define add
        m_DstText.AppendLine();
        m_DstText.AppendLine("\t\t[UnityEngine.HideInInspector]");
        m_DstText.AppendLine("\t\tpublic bool _InitState = false;");
        m_DstText.AppendLine("\t\tpublic System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>> _TempCache = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>>();");
        for(int i=0;i<defineMember.Count;i++)
        {
            m_DstText.AppendLine("\t\t[UnityEngine.HideInInspector]");
            m_DstText.AppendLine($"\t\tpublic {defineMember[i]}");
        }
             
        //onEnabled
       // m_DstText.AppendLine();
       // m_DstText.AppendLine(string.Format("\t\tpublic override void OnEnabled(bool isEnabled,params object[] parm)"));
       // m_DstText.AppendLine("\t\t\t{");
       // m_DstText.AppendLine(string.Format("\t\t\t\tbase.OnEnabled(isEnabled,parm);"));
       // m_DstText.AppendLine("\t\t\t\tif (isEnabled)");
       // m_DstText.AppendLine("\t\t\t\t{");
       // m_DstText.AppendLine("\t\t\t\t\t//addContextListener(MainUIGlobals.MAINUI_APP_RECOVER, self.ApplicationRecover, self)");
       // m_DstText.AppendLine("\t\t\t\t}");
       // m_DstText.AppendLine("\t\t\t\telse");
       // m_DstText.AppendLine("\t\t\t\t{");
       // m_DstText.AppendLine("\t\t\t\t\t//Framework.SignalBus:delContextListener(MainUIGlobals.MAINUI_APP_RECOVER, self.ApplicationRecover, self);");
       // m_DstText.AppendLine("\t\t\t\t}");

       //m_DstText.AppendLine("\t\t\t}");

        ////onShow
        //m_DstText.AppendLine();
        //m_DstText.AppendLine(string.Format("\t\tpublic override void OnShow(params object[] parm)"));
        //m_DstText.AppendLine("\t\t\t{");
        //m_DstText.AppendLine(string.Format("\t\t\t\tbase.OnShow(parm);"));
        //// m_DstText.AppendLine("\tself._InitState = true ");
        //m_DstText.AppendLine("\t\t\t}");

        //onExit
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic override void OnExit(bool isDestroy)"));
        m_DstText.AppendLine("\t\t\t{");
        m_DstText.AppendLine(string.Format("\t\t\t\tbase.OnExit(isDestroy);"));
        m_DstText.AppendLine("\t\t\t\tif (isDestroy)");
        m_DstText.AppendLine("\t\t\t\t{");
        m_DstText.AppendLine("\t\t\t\t\t _InitState = false;");
        m_DstText.AppendLine("\t\t\t\t\tif (_TempCache.Count>0)");
        m_DstText.AppendLine("\t\t\t\t\t\t{");
        m_DstText.AppendLine("\t\t\t\t\t\t\tfor(int i=0;i<_TempCache.Count;i++)");
        m_DstText.AppendLine("\t\t\t\t\t\t\t\t{");
        m_DstText.AppendLine("\t\t\t\t\t\t\t\t\t_TempCache[i].Clear();");
        m_DstText.AppendLine("\t\t\t\t\t\t\t\t}");
        m_DstText.AppendLine("\t\t\t\t\t\t}");
        m_DstText.AppendLine("\t\t\t\t}");
        m_DstText.AppendLine("\t\t\t\t_TempCache.Clear();");
        foreach (var Dic in m_NeedObjs)
        {
            Transform trans = Dic.Value;
            string ObjName = trans.name;
            ObjName = ObjName.Replace(m_Prefix, "");
            if (trans.name.StartsWith(m_Prefix + m_PreMustObj[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreMustObj[3]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[0]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[1]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else if (trans.name.StartsWith(m_Prefix + m_PreObjectExt[2]))
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
            else
            {
                m_DstText.AppendLine(string.Format("\t\t\t\t_{0} = null;", ObjName));
            }
        }
        m_DstText.AppendLine("\t\t\t}");


        //addListeners
        //m_DstText.AppendLine();
        //m_DstText.AppendLine(string.Format("\t\tpublic override void AddListeners()"));
        //m_DstText.AppendLine("\t\t\t{");
        //m_DstText.AppendLine(string.Format("\t\t\t\tbase.AddListeners();"));
        //m_DstText.AppendLine("\t\t\t}");

        ////removeListeners
        //m_DstText.AppendLine();
        //m_DstText.AppendLine(string.Format("\t\tpublic override void DelListeners()"));
        //m_DstText.AppendLine("\t\t\t{");
        //m_DstText.AppendLine(string.Format("\t\t\t\tbase.DelContextListeners();"));
        //m_DstText.AppendLine("\t\t\t}");

        //OnInitAndroid
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void OnInitAndroid()", m_className));
        m_DstText.AppendLine("\t\t\t{");
        // m_DstText.AppendLine(string.Format("\terror(\"[UIPrefab]{0}:onInitAndroid not implemented.\")", m_LuaName));
        m_DstText.AppendLine("\t\t\t}");

        //OnInitIos
        m_DstText.AppendLine();
        m_DstText.AppendLine(string.Format("\t\tpublic virtual void OnInitIos()", m_className));
        m_DstText.AppendLine("\t\t\t{");
        // m_DstText.AppendLine(string.Format("\terror(\"[UIPrefab]{0}:onInitIos not implemented.\")", m_LuaName));
        m_DstText.AppendLine("\t\t\t}");

        //m_DstText.AppendLine();
        //m_DstText.AppendLine(string.Format("\t\tpublic override void ApplicationRecover(float lostTime)"));
        //m_DstText.AppendLine("\t\t\t{");
        //m_DstText.AppendLine("\t\t\t\tbase.ApplicationRecover(lostTime);");
        //// m_DstText.AppendLine(string.Format("\terror(\"[UIPrefab]{0}:ApplicationRecover not implemented.\")", m_LuaName));
        //m_DstText.AppendLine("\t\t\t}");
        CheckMust();

        List<Transform> tempObjs = new List<Transform>(m_DycObj);
        List<Transform> tempExtObjs = new List<Transform>();
        List<Transform> sectionObjs = new List<Transform>();
        sectionObjs.Clear();
        tempExtObjs.Clear();

        do
        {
            CheckScollCom();
            tempObjs.AddRange(tempExtObjs);
            tempExtObjs.Clear();

            foreach (var obj in tempObjs)
            {
                string controlType = m_DicDyc[obj];
                if (controlType.StartsWith(m_Prefix + m_preSection))
                {
                    sectionObjs.Add(obj);
                    m_DycObj.Clear();
                }
                else
                {
                    CheckTransform(obj);
                    //UDLib.Utility.CDebugOut.Log(CDebugOut.LEVEL.DEBUG, "DynamicObj:", obj.name+ "||type:" + controlType);
                    if (controlType.StartsWith(m_Prefix + m_preDymic))
                    {
                        CheckOnDymicInit(controlType);
                        CheckDymicMust();
                    }
                    else if (controlType.StartsWith(m_Prefix + m_preStatic))
                    {
                        CheckOnStaticInit(controlType);
                        CheckStaticMust();
                    }
                    else if (controlType.StartsWith(m_Prefix + m_ScrollHand[0]) || controlType.StartsWith(m_Prefix + m_ScrollHand[1
                        ]))
                    {
                        CheckOnScollInit(controlType, obj.name);
                        CheckScollMust();
                    }
                }

                m_DicDyc.Remove(obj);
                tempExtObjs.AddRange(m_DycObj);
            }
            tempObjs.Clear();
        } while (tempExtObjs.Count > 0);

        m_DicDyc.Clear();
        m_FuncAdd.Clear();
        m_StaticAdd.Clear();
        m_DstText.AppendLine();      
        m_DstText.AppendLine("\t}");
        File.WriteAllText(genDir + m_Path, m_DstText.ToString());

        foreach (var obj in sectionObjs)
        {
            m_className = obj.name.Replace(m_Prefix + m_preSection, "") + "Section";
            m_Path = csDirPath + m_className + "Base.cs";
            m_SelectTrans = obj;
            CheckCS("BaseSection");
        }
    }

    static void CheckFiles()
    {
        //检查UIBase
        m_GenObjs.Clear();
        if (Selection.activeObject.GetType() == typeof(DefaultAsset))
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                m_GenObjs.Add(go);
            }
            Debug.Log($"Dir:{path}");
        }
        else if (Selection.activeObject.GetType() == typeof(GameObject))
        {
            m_GenObjs.Add(Selection.activeObject);
            Debug.Log($"File:obj");
        }
    }
    static List<object> m_GenObjs = new List<object>();

    [MenuItem("Assets/GenC#File")]
    public static void GenCSharpFile()
    {

        CheckFiles();
        foreach (var go in m_GenObjs)
        {
            Transform trans = (go as GameObject).transform;
            m_SelectTrans = trans;
            //新建.cs
            CreateNewCSharp(trans);
        }

        EditorUtility.DisplayDialog("完成", "生成CSharp代码成功", "确定");
    }
    [MenuItem("Assets/CheckC#File")]
    public static void CheckErrorFile()
    {
        CheckFiles();
        foreach (var go in m_GenObjs)
        {
            Transform trans = (go as GameObject).transform;
            //UICheckLua.CheckTransform(trans);
        }
    }
}


#endif
