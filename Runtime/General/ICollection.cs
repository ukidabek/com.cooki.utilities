using System.Collections.Generic;

namespace Utilities.General
{
    public interface ICollection<T> : IDictionary<Key, T>, ITickableElement where T : IKeyableElement
    {
        void Initialize();
    }
}