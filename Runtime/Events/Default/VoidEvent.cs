using UnityEngine;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    [CreateAssetMenu(menuName = "Utilities/Events/VoidEvent", fileName = "VoidEvent")]
    public class VoidEvent : Event<IEventListener>
    {
    }
}