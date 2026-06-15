using UnityEngine;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    [CreateAssetMenu(menuName = "Utilities/Events/Vector2IntEvent", fileName = "Vector2IntEvent")]
    public class Vector2IntEvent : ParameterizedEvent<Vector2Int> { }
}
