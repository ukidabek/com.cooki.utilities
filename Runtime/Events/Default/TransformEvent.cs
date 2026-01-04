using UnityEngine;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    [CreateAssetMenu(menuName = "Utilities/Events/TransformEvent", fileName = "TransformEvent")]
    public class TransformEvent : ParameterizedEvent<Transform>
    {
    }
}