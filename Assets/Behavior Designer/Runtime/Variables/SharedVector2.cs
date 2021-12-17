using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedVector2 : SharedVariable
    {
        public Vector2 Value { get { return mValue; } set { if (mValue != value) { ValueChanged(); } mValue = value; } }
        [SerializeField]
        private Vector2 mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Vector2)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedVector2(Vector2 value) { return new SharedVector2 { mValue = value }; }
    }
}