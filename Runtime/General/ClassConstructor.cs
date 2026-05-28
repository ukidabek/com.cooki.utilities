using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities.General
{
    /// <summary>
    /// Experimental class.
    /// </summary>
    [Serializable] 
    public class ClassConstructor
    {
        [Serializable] public class TypeInfo
        {
            [SerializeField] private string _assemblyFullName = string.Empty;
            public string AssemblyFullName => _assemblyFullName;

            [SerializeField] private string _typeName = string.Empty;
            public string FullName => _typeName;

            [SerializeField] private string _name = string.Empty;
            public string Name => _name;

            private Type _type = null;

            public TypeInfo(Type type)
            {
                _assemblyFullName = type.Assembly.GetName().FullName;
                _typeName = type.FullName;
                _name = type.Name;
            }

            private bool AssemblyQuery(Assembly assembly) => assembly.GetName().FullName == _assemblyFullName;

            public static implicit operator Type(TypeInfo stateInfo)
            {
                if (stateInfo == null || string.IsNullOrEmpty(stateInfo._assemblyFullName) || string.IsNullOrEmpty(stateInfo._typeName))
                    return null;

                if (stateInfo._type != null) return stateInfo._type;
                
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(stateInfo.AssemblyQuery);
                stateInfo._type = assembly.GetType(stateInfo._typeName);

                return stateInfo._type;
            }
        }

        [Serializable] 
        public class ConstructorParameter
        {
            [SerializeField] private string _parameterName = string.Empty;
            public string ParameterName => _parameterName;

            [SerializeField] private TypeInfo _type = null;
            public TypeInfo Type
            {
                get => _type;
                set => _type = value;
            }

            public UnityEngine.Object ObjectValue;
            public int IntValue = 0;
            public float FloatValue = 0f;
            public bool BoolValue = false;
            public string StringValue = string.Empty;
            public Color ColorValue = Color.white;

            public ConstructorParameter(ParameterInfo parameterInfo)
            {
                Type = new TypeInfo(parameterInfo.ParameterType);
                _parameterName = parameterInfo.Name;
            }

            public override int GetHashCode()
            {
                var value = 0;
                for (var i = 0; i < _parameterName.Length; i++)
                    value += _parameterName[i];
                return value;
            }

            public object GetObject()
            {
                Type type = Type;
                switch (type.Name)
                {
                    case "Int32":
                        return IntValue;
                    case "Single":
                        return FloatValue;
                    case "Boolean":
                        return BoolValue;
                    case "String":
                        return StringValue;
                    case "Color":
                        return ColorValue;
                    default:
                        return ObjectValue;
                }
            }
        }

        [SerializeField] private TypeInfo _baseType = null;
        public TypeInfo BaseType
        {
            get => _baseType;
            set => _baseType = value;
        }

        [SerializeField] private TypeInfo _type = null;
        public TypeInfo Type => _type;

        [SerializeField] private ConstructorParameter[] _parameters = null;
        public ConstructorParameter[] Parameters => _parameters;

        [SerializeField] private string _name = string.Empty;
        public string Name => _name;

        public ClassConstructor() { }

        public ClassConstructor(Type type)
        {
            _baseType = new TypeInfo(type);
        }

        public ClassConstructor(ConstructorInfo info, Type type) : this(type)
        {
            _type = new TypeInfo(info.DeclaringType);
            _name = $"{info.DeclaringType.Name} (";

            var parameters = info.GetParameters();
            _parameters = new ConstructorParameter[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                _parameters[i] = new ConstructorParameter(parameters[i]);
                _name += $"{_parameters[i].Type.Name} {_parameters[i].ParameterName}{(i < parameters.Length - 1 ? ", " : "")}";
            }
            _name += ")";
        }


        public object CreateInstance()
        {
            var data = new object[_parameters.Length];

            for (var i = 0; i < _parameters.Length; i++)
                data[i] = _parameters[i].GetObject();

            return Activator.CreateInstance(_type, data);
        }

        public override int GetHashCode()
        {
            var value = 0;
            foreach (var character in _type.FullName)
                value += character;

             value += _parameters.Sum(t => t.GetHashCode());

            return value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return GetHashCode() == obj.GetHashCode();
        }

        public static bool operator != (ClassConstructor a, ClassConstructor b)
        {
            return !(a == b);
        }

        public static bool operator == (ClassConstructor a, ClassConstructor b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            
            if ((ReferenceEquals(a, null) && !ReferenceEquals(b, null)) ||
                (!ReferenceEquals(a, null) && ReferenceEquals(b, null)))
                return false;

            return a.Equals(b);
        }
    }
}