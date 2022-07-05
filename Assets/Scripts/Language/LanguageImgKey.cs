using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class LanguageImgKey : MonoBehaviour
{
    public string key;
    public void Localize()
    {
        if (!string.IsNullOrEmpty(key))
            GetComponent<Image>().sprite = LanguageData.GetImg(key);
    }
    private void Start()
    {
        if (!string.IsNullOrEmpty(key))
            GetComponent<Image>().sprite = LanguageData.GetImg(key);
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
