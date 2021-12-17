using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedString : SharedVariable
    {
        public string Value { get { return mValue; } set { if (mValue != value) { ValueChanged(); } mValue = value; } }
        [SerializeField]
        private string mValue = "";

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (string)value; }

        public override string ToString() { return string.IsNullOrEmpty(mValue) ? "" : mValue.ToString(); }
        public static implicit operator SharedString(string value) { return new SharedString { mValue = value }; }
    }
}