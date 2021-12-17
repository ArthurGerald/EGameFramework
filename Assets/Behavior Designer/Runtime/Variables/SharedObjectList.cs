using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedObjectList : SharedVariable
    {
        public List<Object> Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private List<Object> mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (List<Object>)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.Count + " Objects"); }
        public static implicit operator SharedObjectList(List<Object> value) { return new SharedObjectList { mValue = value }; }
    }
}