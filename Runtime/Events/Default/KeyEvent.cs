using UnityEngine;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    [CreateAssetMenu(menuName = "Utilities/Events/KeyEvent", fileName = "KeyEvent")]
    public class KeyEvent : ParameterizedEvent<Key>
    {
    }
}