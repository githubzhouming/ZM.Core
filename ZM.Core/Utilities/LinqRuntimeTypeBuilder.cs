﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace ZM.Core.Utilities
{
    public class LinqRuntimeTypeBuilder
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "ZM.DynamicRuntime" };
        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();
        static LinqRuntimeTypeBuilder()
        {
            ModuleBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> properties)
        {
            //TODO: optimize the type caching -- if fields are simply reordered, that doesn't mean that they're actually different types, so this needs to be smarter
            string key = string.Empty;
            foreach (var prop in properties)
                key += prop.Key + ";" + prop.Value.Name + ";";

            return key;
        }

        private const MethodAttributes RuntimeGetSetAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static Type BuildDynamicType([NotNull] Dictionary<string, Type> properties)
        {
            if (null == properties)
                throw new ArgumentNullException(nameof(properties));
            if (0 == properties.Count)
                throw new ArgumentOutOfRangeException(nameof(properties), "fields must have at least 1 field definition");

            try
            {
                // Acquires an exclusive lock on the specified object.
                Monitor.Enter(BuiltTypes);
                string className = GetTypeKey(properties);

                if (BuiltTypes.ContainsKey(className))
                    return BuiltTypes[className];

                TypeBuilder typeBdr = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var prop in properties)
                {
                    var propertyBdr = typeBdr.DefineProperty(name: prop.Key, attributes: PropertyAttributes.None, returnType: prop.Value, parameterTypes: null);
                    var fieldBdr = typeBdr.DefineField("itheofield_" + prop.Key, prop.Value, FieldAttributes.Private);

                    MethodBuilder getMethodBdr = typeBdr.DefineMethod("get_" + prop.Key, RuntimeGetSetAttrs, prop.Value, Type.EmptyTypes);
                    ILGenerator getIL = getMethodBdr.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, fieldBdr);
                    getIL.Emit(OpCodes.Ret);

                    MethodBuilder setMethodBdr = typeBdr.DefineMethod("set_" + prop.Key, RuntimeGetSetAttrs, null, new Type[] { prop.Value });
                    ILGenerator setIL = setMethodBdr.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, fieldBdr);
                    setIL.Emit(OpCodes.Ret);

                    propertyBdr.SetGetMethod(getMethodBdr);
                    propertyBdr.SetSetMethod(setMethodBdr);
                }

                BuiltTypes[className] = typeBdr.CreateType();

                return BuiltTypes[className];
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }
        }

        private static string GetTypeKey(IEnumerable<PropertyInfo> properties)
        {
            return GetTypeKey(properties.ToDictionary(f => f.Name, f => f.PropertyType));
        }

        public static Type GetDynamicType(IEnumerable<PropertyInfo> properties)
        {
            return BuildDynamicType(properties.ToDictionary(f => f.Name, f => f.PropertyType));
        }
    }
}
