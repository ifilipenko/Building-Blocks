using System;
using System.Reflection;

namespace BuildingBlocks.Common.Utils
{
    public static class TypesUtil
    {
        static Type LocalTypeLoad(string entityClassFullName)
        {
            return Type.GetType(entityClassFullName, true, false);
        }

        static Type AssemblyTypeLoad(string entityClassFullName)
        {
            try
            {
                return _entityTypesAssembly.GetType(entityClassFullName, true, false);
            }
            catch (TypeLoadException)
            {
                if (AppDomain.CurrentDomain != null)
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type type = assembly.GetType(entityClassFullName, false, false);
                        if (type != null)
                        {
                            return type;
                        }
                    }
                }
                throw;
            }
        }

        delegate Type ClassToTypeMethod(string entityClassFullName);

        private static ClassToTypeMethod _classToTypeMethod = LocalTypeLoad;

        private static Assembly _entityTypesAssembly;

        public static string EntityTypesAssembly
        {
            get { return _entityTypesAssembly.FullName; }
            set
            {
                _entityTypesAssembly = Assembly.Load(value);
                if (_entityTypesAssembly != null)
                    _classToTypeMethod = AssemblyTypeLoad;
            }
        }

        public static Assembly EntitysAssembly
        {
            get { return _entityTypesAssembly; }
            set
            {
                _entityTypesAssembly = value;
                if (_entityTypesAssembly != null)
                    _classToTypeMethod = AssemblyTypeLoad;
            }
        }

        [Obsolete("Need change type of exception")]
        public static Type NameToType(string name)
        {
            switch (name.ToLower())
            {
                case "int32":
                    return typeof(int);
                case "string":
                    return typeof(string);
                case "double":
                    return typeof (double);
            }
            throw new Exception(string.Format("Type name \"{0}\" can't be converted to Type structure", name));
        }

        public static Type EntityClassToType(string entityClassFullName)
        {
            try
            {
                return _classToTypeMethod(entityClassFullName);
            }
            catch (TypeLoadException)
            {
                try
                {
                    return LocalTypeLoad(entityClassFullName);
                }
                catch (TypeLoadException)
                {
                }
                throw;
            }
        }

        public static object ConvertDBNullToDefaultValue(Type valueType)
        {
            switch (Type.GetTypeCode(valueType.GetType()))
            {
                case TypeCode.String:
                    return default(string);
                case TypeCode.Boolean:
                    return default(bool);
                case TypeCode.Int32:
                    return default(int);
                case TypeCode.Int64:
                    return default(long);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.Double:
                    return default(double);
            }
            return null;
        }
    }
}