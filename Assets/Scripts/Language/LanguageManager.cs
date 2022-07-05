using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager
{
    public delegate void ChangeLanguageDelegate();
    public static ChangeLanguageDelegate OnLocalize;
}
