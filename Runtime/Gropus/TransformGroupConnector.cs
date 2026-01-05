using UnityEngine;

namespace Utilities.Groups
{
    public class TransformGroupConnector : ObjectGroupConnector<TransformObjectGroup, Transform>
    {
        protected override void Reset() => m_object = transform;
    }
}