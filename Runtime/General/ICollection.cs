using System.Collections.Generic;

namespace Utilities.General
{
    public interface ICollection<T> : IReadOnlyDictionary<Key, T>, ITickableElement where T : IKeyableElement
    {
        void Initialize();
        void Reset();
    }
}