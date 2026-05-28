using UnityEngine;
using UnityEngine.Events;
using Utilities.General.Events.Core;

namespace Utilities.General.Events
{
    public class VoidEventListener : EventListenerBehaviour
    { 
        public UnityEvent Callback = new UnityEvent();
        
        [SerializeField] private VoidEvent m_event = null;
        public VoidEvent Event => m_event;
        
        public override void Invoke() => Callback.Invoke();

        private void OnEnable() => Event.Subscribe(this);

        private void OnDisable() => Event.Unsubscribe(this);
    }
}