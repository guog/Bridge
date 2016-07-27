using Bridge;

namespace System.Diagnostics
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    [External]
    [NonScriptable]
    public sealed class ConditionalAttribute : Attribute
    {
        public extern ConditionalAttribute(string conditionString);

        public extern string ConditionString { get; }
    }
}