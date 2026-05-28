using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.General
{
    [CreateAssetMenu(fileName = "KeyChain", menuName = "Utilities/KeyChain")]
    public class KeyChain : ScriptableObject, IReadOnlyList<Key>
    {
        [SerializeField] private List<Key> m_keys = new List<Key>(10);
        
        public IEnumerator<Key> GetEnumerator() => m_keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => m_keys.Count;

        public Key this[int index] => m_keys[index];
    }
}