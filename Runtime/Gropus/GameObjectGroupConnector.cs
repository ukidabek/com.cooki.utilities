using UnityEngine;

namespace Utilities.Groups
{
    public class GameObjectGroupConnector : ObjectGroupConnector<GameObjectGroup, GameObject>
    {
        protected override void Reset() => m_object = gameObject;
    }
}