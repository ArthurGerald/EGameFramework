using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBool : SharedVariable
    {
        public bool Value { get { return mValue; } set { if (mValue != value) { ValueChanged(); } mValue = value; } }
        [SerializeField]
        private bool mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (bool)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedBool(bool value) { return new SharedBool { mValue = value }; }
    }
}