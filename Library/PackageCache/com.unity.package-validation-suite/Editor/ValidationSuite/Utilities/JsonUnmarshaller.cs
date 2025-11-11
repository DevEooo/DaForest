using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class JsonUnmarshaller
    {
        private static readonly MethodInfo GetValForFieldMethod = typeof(JsonUnmarshaller).GetMethod(nameof(GetValueForFieldInternal), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo GetListMethod = typeof(JsonUnmarshaller).GetMethod(nameof(GetList), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo GetDictionaryMethod = typeof(JsonUnmarshaller).GetMethod(nameof(GetDictionary), BindingFlags.Static | BindingFlags.NonPublic);

        private static Stack<string> fieldStack = new Stack<string>();


        public static T GetValue<T>(object val, ref List<UnmarshallingException> unmarshallingErrors)
        {
            return GetValueInternal<T>(val, ref unmarshallingErrors);
        }

        private static T GetValueForFieldInternal<T>(object val, string fieldName, ref List<UnmarshallingException> unmarshallingErrors)
        {
            try
            {
                fieldStack.Push(fieldName);
                return GetValueInternal<T>(val, ref unmarshallingErrors);
            }
            finally
            {
                fieldStack.Pop();
            }
        }

        private static bool HasConversionOperator(Type from, Type to)
        {
            try
            {
                var inp = Expression.Parameter(from, "inp");
                // If this succeeds then we can cast 'from' type to 'to' type using implicit coercion
                Expression.Lambda(Expression.Convert(inp, to), inp).Compile();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static T GetValueInternal<T>(object val, ref List<UnmarshallingException> unmarshallingErrors)
        {
            if (val == null)
            {
                return default;
            }

            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
            {
                if (!HasConversionOperator(val.GetType(), type))
                {
                    unmarshallingErrors.Add(new UnmarshallingException(string.Join(".", fieldStack), type, val.GetType()));
                    return default;
                }

                var convertedType = (T)Convert.ChangeType(val, type);
                return convertedType;
            }

            if (typeof(IList).IsAssignableFrom(type))
            {
                var getValFunc = GetListMethod.MakeGenericMethod(type.GetGenericArguments());
                var res = getValFunc.Invoke(null, new object[] { val, unmarshallingErrors });
                return (T)res;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var getValFunc = GetDictionaryMethod.MakeGenericMethod(type.GetGenericArguments()[1]);
                var res = getValFunc.Invoke(null, new object[] { val, unmarshallingErrors });
                return (T)res;
            }

            {
                var instance = Activator.CreateInstance<T>();
                foreach (var field in type.GetFields())
                {
                    var nonSerializedAttribute = field.GetCustomAttribute<NonSerializedAttribute>();
                    if (nonSerializedAttribute != null)
                    {
                        continue;
                    }
                    var mainFieldUnmarshallingErrors = DeserializeToField(instance, field, val, field.Name);
                    var success = !mainFieldUnmarshallingErrors.Any();
                    if (!success)
                    {
                        var alternativeSerializationFormats =
                            field.GetCustomAttributes<AlternativeSerializationFormat>();
                        foreach (var attr in alternativeSerializationFormats)
                        {
                            var alternativeField = type.GetField(attr.AlternativeFormatFieldName,
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            var alternativeUnmarshallingErrors = DeserializeToField(instance, alternativeField, val, field.Name);
                            if (!alternativeUnmarshallingErrors.Any())
                            {
                                success = true;
                                break;
                            }
                        }
                    }

                    if (!success)
                    {
                        unmarshallingErrors.AddRange(mainFieldUnmarshallingErrors);
                    }
                }

                return instance;
            }
        }

        private static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        internal static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type);
        }

        private static List<UnmarshallingException> DeserializeToField<T>(T instance, FieldInfo field, object val, string fieldName)
        {
            List<UnmarshallingException> unmarshallingErrors = new List<UnmarshallingException>();
            var getValFunc = GetValForFieldMethod.MakeGenericMethod(field.FieldType);

            if (!(val is Dictionary<string, object>))
            {
                unmarshallingErrors.Add(new UnmarshallingException(string.Join(".", fieldStack), typeof(Dictionary<string, object>), val.GetType()));
                return unmarshallingErrors;
            }

            var dict = (Dictionary<string, object>)val;

            if (dict.TryGetValue(fieldName, out var dictVal))
            {
                var res = getValFunc.Invoke(null, new object[] { dictVal, fieldName, unmarshallingErrors });
                field.SetValue(instance, res);
            }

            return unmarshallingErrors;
        }

        private static List<T> GetList<T>(object val, ref List<UnmarshallingException> unmarshallingErrors)
        {
            var list = new List<T>();

            if (!(val is List<object>))
            {
                unmarshallingErrors.Add(new UnmarshallingException(string.Join(".", fieldStack), typeof(List<T>), val.GetType()));
                return null;
            }

            var i = 0;
            foreach (var item in (List<object>)val)
            {
                list.Add(GetValueForFieldInternal<T>(item, $@"[{i}]", ref unmarshallingErrors));
                ++i;
            }

            return list;
        }

        private static Dictionary<string, T> GetDictionary<T>(object val, ref List<UnmarshallingException> unmarshallingErrors)
        {
            var result = new Dictionary<string, T>();

            if (!(val is Dictionary<string, object>))
            {
                unmarshallingErrors.Add(new UnmarshallingException(string.Join(".", fieldStack), typeof(Dictionary<string, object>), val.GetType()));
                return null;
            }

            foreach (var item in (Dictionary<string, object>)val)
            {
                result.Add(item.Key, GetValueForFieldInternal<T>(item.Value, $@"[{item.Key}]", ref unmarshallingErrors));
            }

            return result;
        }
    }
    internal class AlternativeSerializationFormat : Attribute
    {
        public string AlternativeFormatFieldName { get; }
        internal AlternativeSerializationFormat(string alternativeFormatFieldName)
        {
            AlternativeFormatFieldName = alternativeFormatFieldName;
        }
    }

    internal class UnmarshallingException : Exception
    {
        private readonly string jsonFieldName;
        private readonly Type expectedType;
        private readonly Type actualType;

        public UnmarshallingException(string jsonFieldName, Type expectedType, Type actualType)
        {
            this.jsonFieldName = jsonFieldName;
            this.expectedType = expectedType;
            this.actualType = actualType;
        }

        public override string Message => $@"Error marshalling '{jsonFieldName}' expected {expectedType.Name} but got {actualType} instead";
    }
}
