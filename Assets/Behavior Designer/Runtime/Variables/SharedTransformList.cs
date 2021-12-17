using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedTransformList : SharedVariable
    {
        public List<Transform> Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private List<Transform> mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (List<Transform>)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.Count + " Transforms"); }
        public static implicit operator SharedTransformList(List<Transform> value) { return new SharedTransformList { mValue = value }; }
    }
}