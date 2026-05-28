using System;
using UnityEngine;

namespace Utilities.General
{
    [CreateAssetMenu(fileName = "NewKey", menuName = "Utilities/Key")]
    public class Key : ScriptableObject, IEquatable<Key>
    {
        [SerializeField, HideInInspector] private int m_hash = default;
        [SerializeField, HideInInspector] private string m_guid = default;
        
        private void OnValidate() => Hash();

        private void Reset() => Hash();

        private void Hash()
        {
            if(!string.IsNullOrEmpty(m_guid)) return;
            m_guid = Guid.NewGuid().ToString();
            m_hash = HashFunctions.FNVHash(m_guid);
        }

        public static bool operator ==(Key a, Key b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.m_hash == b.m_hash;
        }

        public static bool operator !=(Key a, Key b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.m_hash != b.m_hash;
        }

        public bool Equals(Key other)
        {
            if (other is null) return false;
            return m_hash == other.m_hash;
        }

        public override bool Equals(object obj) => obj is Key other && Equals(other);

        public override int GetHashCode() => m_hash;
    }
}