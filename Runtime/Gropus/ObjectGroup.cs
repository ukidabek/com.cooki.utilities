using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Utilities.Groups
{
    public abstract class ObjectGroup : ScriptableObject
    {
        public abstract IEnumerable<Object> GetObjects();
        public abstract void AddObject(Object obj);
        public abstract void RemoveObject(Object obj);
        public abstract void Clear();
    }

    public abstract class ObjectGroup<T> : ObjectGroup, IEnumerable<T> where T : Object
    {
        public UnityEvent<T> OnObjectAdded = new UnityEvent<T>();
        public UnityEvent<T> OnObjectRemoved = new UnityEvent<T>();

        private readonly HashSet<T> m_objects = new HashSet<T>();

        public override IEnumerable<Object> GetObjects() => m_objects;

        public override void AddObject(Object obj)
        {
            if (obj is not T @object) return;
            AddObject(@object);
        }

        public override void RemoveObject(Object obj)
        {
            if (obj is not T @object) return;
            RemoveObject(@object);
        }

        public virtual void AddObject(T obj)
        {
            if (!m_objects.Add(obj)) return;
            OnObjectAdded.Invoke(obj);
        }

        public virtual void RemoveObject(T obj)
        {
            if (!m_objects.Remove(obj)) return;
            OnObjectRemoved.Invoke(obj);
        }

        public override void Clear() => m_objects.Clear();

        public IEnumerator<T> GetEnumerator() => m_objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ObjectGroupConnector<T, T1> : MonoBehaviour where T : ObjectGroup<T1> where T1 : Object
    {
        [SerializeField] protected T m_group = null;
        [SerializeField] protected T1 m_object = null;
        private void OnEnable() => m_group.AddObject(m_object);

        private void OnDisable() => m_group.RemoveObject(m_object);

        protected virtual void Reset() => m_object = GetComponent<T1>();
    }
}