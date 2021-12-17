using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedFloat : SharedVariable
    {
        public float Value { get { return mValue; } set { if (mValue != value) { ValueChanged(); } mValue = value; } }
        [SerializeField]
        private float mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (float)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedFloat(float value) { return new SharedFloat { mValue = value }; }
    }
}