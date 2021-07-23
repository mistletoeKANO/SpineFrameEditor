using System;
using System.Globalization;
using System.Reflection;

namespace ActionFrame.Runtime
{
    public static class ReflectionUtil
    {
        public static void InvokeMethodInstance(string typeName, object instance, string methodName, params object[] args)
        {
            Type type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception($"类型不存在 {typeName}");
            }
            type.InvokeMember(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, instance, args);
        }
        
        public static void InvokeMethodInstance(Type type, object instance, string methodName, params object[] args)
        {
            type.InvokeMember(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, instance, args);
        }
        
        public static void InvokeMethod(Type type, object instance, string methodName, params object[] args)
        {
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic);
            methodInfo?.Invoke(instance, BindingFlags.Instance | BindingFlags.NonPublic, null, args,
                CultureInfo.CurrentCulture);
        }
    }
}