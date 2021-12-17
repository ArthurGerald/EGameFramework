using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedMaterial : SharedVariable
    {
        public Material Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Material mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Material)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedMaterial(Material value) { return new SharedMaterial { mValue = value }; }
    }
}