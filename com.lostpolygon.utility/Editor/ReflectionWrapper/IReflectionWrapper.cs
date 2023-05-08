using System;

namespace LostPolygon.Unity.Utility.Editor {
    internal interface IReflectionWrapper {
        ReflectionWrapper.MethodHandle<T> Method<T>(string methodName, Type[] parameterTypes = null);
        ReflectionWrapper.MethodHandle<object> Method(string methodName, Type[] parameterTypes = null);
        ReflectionWrapper.FieldHandle<T> Field<T>(string fieldName);
        ReflectionWrapper.FieldHandle<object> Field(string fieldName);
        ReflectionWrapper.PropertyHandle<object> Property(string propertyName);
        ReflectionWrapper.PropertyHandle<T> Property<T>(string propertyName);
    }
}
