using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class GenericVariable
    {
        [SerializeField]
        public string type = "SharedString";
        [SerializeField]
        public SharedVariable value;
    }

    [System.Serializable]
    public class SharedGenericVariable : SharedVariable
    {
        public GenericVariable Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private GenericVariable mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (GenericVariable)value; }

        public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
        public static implicit operator SharedGenericVariable(GenericVariable value) { return new SharedGenericVariable { mValue = value }; }
    }
}