using UnityEngine;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    [CreateAssetMenu(menuName = "Utilities/Events/ObjectEvent", fileName = "ObjectEvent")]
    public class ObjectEvent : ParameterizedEvent<Object>
    {
    }
}