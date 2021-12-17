using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedGameObjectList : SharedVariable
    {
        public List<GameObject> Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private List<GameObject> mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (List<GameObject>)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.Count + " GameObjects"); }
        public static implicit operator SharedGameObjectList(List<GameObject> value) { return new SharedGameObjectList { mValue = value }; }
    }
}