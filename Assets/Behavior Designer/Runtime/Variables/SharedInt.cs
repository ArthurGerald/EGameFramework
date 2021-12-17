using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedInt : SharedVariable
    {
        public int Value { get { return mValue; } set { if (mValue != value) { ValueChanged(); } mValue = value; } }
        [SerializeField]
        private int mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (int)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedInt(int value) { return new SharedInt { mValue = value }; }
    }
}