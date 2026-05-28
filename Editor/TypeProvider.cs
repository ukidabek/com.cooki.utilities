using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Profiling;

namespace Utilities.General
{
    public class TypeProvider : ScriptableObject, ISearchWindowProvider
    {
        private static Type[] Types = null;
        private static Dictionary<Type, Type[]> BaseTypeToRelatedTypes = new Dictionary<Type, Type[]>();
        public SerializedProperty Property { get; set; }
        private readonly List<SearchTreeEntry> m_searchTreeEntry = new List<SearchTreeEntry>();

        [InitializeOnLoadMethod]
        private static void CacheTypes()
        {
            BaseTypeToRelatedTypes.Clear();
            Types = null;
            CacheTypesIfNecessary();
        }

        public void GenerateTreeEntries(Type baseType)
        {
            m_searchTreeEntry.Clear();
            m_searchTreeEntry.Add(new SearchTreeGroupEntry(new GUIContent(baseType.Name)));

            CacheTypesIfNecessary();
            
            Profiler.BeginSample($"({nameof(TypeProvider)}) - Generate Tree Entries");

            Type[] selectedTypes = null;
            if(BaseTypeToRelatedTypes.TryGetValue(baseType, out var cachedTypes))
                selectedTypes = cachedTypes;
            else
            {
                selectedTypes = Types.Where(TypeValidateLogic).ToArray();
                BaseTypeToRelatedTypes.Add(baseType, selectedTypes);
            }

            foreach (var type in selectedTypes)
                m_searchTreeEntry.Add(new SearchTreeEntry(new GUIContent(type.Name))
                {
                    level = 1,
                    userData = type,
                });
            Profiler.EndSample();
            
            return;

            bool TypeValidateLogic(Type type)
            {
                if (baseType.IsInterface)
                    return baseType.IsAssignableFrom(type);

                return (type.IsSubclassOf(baseType) || type == baseType) && !type.IsAbstract;
            }
        }

        private static void CacheTypesIfNecessary()
        {
            if (Types != null ) return;
            Profiler.BeginSample($"({nameof(TypeProvider)}) - Cache Types");
            
            Types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(IsTypeValid)
                .ToArray();
            
            Profiler.EndSample();
            
            return;

            bool IsTypeValid(Type type)
            {
                if (type.IsAbstract) return false;
                if (type.IsInterface) return false;
                if (type.IsSubclassOf(typeof(UnityEngine.Object))) return false;
                if (!type.GetCustomAttributes(true).OfType<SerializableAttribute>().Any()) return false;
                return true;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_searchTreeEntry;

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            try
            {
                var Type = SearchTreeEntry.userData as Type;
                var instance = Activator.CreateInstance(Type);
                if (Property.isArray)
                {
                    var index = Property.arraySize++;
                    var newElement = Property.GetArrayElementAtIndex(index);
                    newElement.managedReferenceValue = instance;
                }
                else
                    Property.managedReferenceValue = instance;

                ReferencePropertyDroverHelper.SaveAndReserialize(Property.serializedObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
                
            return true;
        }
    }
}