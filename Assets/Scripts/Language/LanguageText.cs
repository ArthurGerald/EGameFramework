using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LanguageText : MonoBehaviour
{
    public string key;   
    public  void Localize()
    {
        if(!string.IsNullOrEmpty(key))
            GetComponent<Text>().text = LanguageData.Get(key);
    }
    private void Start()
    {
        if (!string.IsNullOrEmpty(key))
            GetComponent<Text>().text = LanguageData.Get(key);
    }

    private void OnEnable()
    {
        LanguageManager.OnLocalize += Localize;
    }

    private void OnDisable()
    {
        LanguageManager.OnLocalize -= Localize;
    }
}
