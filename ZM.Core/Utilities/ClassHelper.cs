using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ZM.Core.Utilities
{
    public static class ClassHelper
    {
        /// <summary>
        /// 获取所有继承的子类
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetChildTypes(Type baseType)
        {
            var assembly = Assembly.GetAssembly(baseType);
            return assembly.GetTypes().Where(x => x != baseType && x.IsAssignableTo(baseType));
        }

        public static IEnumerable<KeyValuePair<Type, PropertyInfo[]>> GetChildTypePropertyInfos(Type baseType)
        {
            var childTypes = GetChildTypes(baseType);
            return childTypes.Select(x => KeyValuePair.Create(x, x.GetProperties()));
        }

        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool HasImplementedRawGeneric([NotNull] this Type type, [NotNull] Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            // 测试接口。
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

            // 测试类型。
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;

            // 测试某个类型是否是指定的原始接口。
            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }

        public static (string name, string typeName) GetPropertyNameType(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            var typeName = string.Empty;
            if (propertyInfo.PropertyType.IsGenericType)
            {
                var definition = propertyInfo.PropertyType.GetGenericTypeDefinition();
                if (definition != null && definition.IsAssignableTo(typeof(Nullable<>)))
                {
                    typeName = propertyInfo.PropertyType.GetGenericArguments()[0].Name;
                }
            }
            else
            {
                typeName = propertyInfo.PropertyType.Name;
            }
            return (name, typeName);
        }

    }
}
