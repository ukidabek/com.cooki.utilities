using System.Collections;
using System.Collections.Generic;

namespace Utilities.General
{
    public class Carousel<T>
    {
        private IList<T> m_connectedlist = null;
        private Index index = null;

        public Carousel(IList<T> list)
        {
            m_connectedlist = list;
            index = new Index((IList)list);
        }

        public T Previous => m_connectedlist[index.Previous];
        public T Current => m_connectedlist[index.Current];
        public T Next => m_connectedlist[index.Next];

        public T Advance() => m_connectedlist[index++];
        public T Retreat() => m_connectedlist[index--];
    }
}