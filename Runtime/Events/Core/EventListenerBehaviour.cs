using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.General.Events.Core
{
    public class EventListenerBehaviour<T, T1> : EventListenerBehaviour<T1> where T : ParameterizedEvent<T1>
    {
        [SerializeField] private T m_event;
        protected override ParameterizedEvent<T1> Event => m_event;
    }

    public abstract class EventListenerBehaviour<T> : EventListenerBehaviour, IEventListener<T>
    {
        public UnityEvent<T> Callback =  new UnityEvent<T>();
        protected abstract ParameterizedEvent<T> Event { get; }
        private void OnEnable() => Event?.Subscribe(this);

        private void OnDisable() => Event?.Unsubscribe(this);

        public override void Invoke() => Invoke(default);
        
        public void Invoke(T eventArgument) => Callback.Invoke(eventArgument);
        
    }

    public abstract class EventListenerBehaviour : MonoBehaviour, IEventListener
    {
        public abstract void Invoke();
    }

    public class EventListener<T> : IEventListener<T>, IDisposable
    {
        public event Action<T> Callback = null;

        public EventListener() { }

        public EventListener(Action<T> callback) => Callback = callback;

        public void Invoke(T eventArgument) => Callback.Invoke(eventArgument);

        public void Invoke() => Invoke(default);

        public void Dispose() => Callback = null;
    }

    public interface IEventListener
    {
        void Invoke();
    }

    public interface IEventListener<in T> : IEventListener
    {
        void Invoke(T eventArgument);
    }
}