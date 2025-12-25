using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.General
{
    public abstract class Collection<T> : ICollection<T> where T : IKeyableElement
    {
        [SerializeReference, ReferenceList] protected List<T> m_items = new List<T>(30);

        protected readonly Dictionary<Key, T> m_collectionDictionary = new Dictionary<Key, T>();

        public void Initialize()
        {
            foreach (var item in m_items)
                m_collectionDictionary.Add(item.Key, item);
        }

        public virtual void Tick(float deltaTime, float timeScale)
        {
            var count = m_items.Count;
            for (var i = 0; i < count; i++)
            {
                if (m_items[i] is not ITickableElement element) continue;
                element.Tick(deltaTime, timeScale);
            }
        }

        public IEnumerator<KeyValuePair<Key, T>> GetEnumerator() => m_collectionDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<Key, T> item) => Add(item.Key, item.Value);

        public virtual void Clear() => m_collectionDictionary.Clear();

        public bool Contains(KeyValuePair<Key, T> item) => m_collectionDictionary.ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<Key, T>[] array, int arrayIndex) { }

        public bool Remove(KeyValuePair<Key, T> item) => m_collectionDictionary.Remove(item.Key);

        public int Count => m_collectionDictionary.Count;
        
        public bool IsReadOnly => false;

        public virtual void Add(Key key, T value)
        {
            m_items.Add(value);
            m_collectionDictionary.Add(key, value);
        }

        public bool ContainsKey(Key key) => m_collectionDictionary.ContainsKey(key);

        public bool Remove(Key key)
        {
            if (!m_collectionDictionary.TryGetValue(key, out var value)) return false;
            m_items.Remove(value);
            return m_collectionDictionary.Remove(key);
        }

        public bool TryGetValue(Key key, out T value) => m_collectionDictionary.TryGetValue(key, out value);

        public T this[Key key]
        {
            get => m_collectionDictionary[key];
            set => m_collectionDictionary[key] = value;
        }

        public System.Collections.Generic.ICollection<Key> Keys => m_collectionDictionary.Keys;
        
        public System.Collections.Generic.ICollection<T> Values => m_collectionDictionary.Values;
    }
}