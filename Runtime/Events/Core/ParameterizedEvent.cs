namespace Utilities.General.Events.Core
{
    public abstract class ParameterizedEvent<T> : Event<IEventListener<T>>, IEvent<T>
    {
        public sealed override void Invoke() => Invoke(default);
        
        private T m_eventArgument;
        
        
        public void Invoke(T eventArgument)
        {
            LogEventInvoke();
            m_eventArgument = eventArgument;
            InvokeListeners(m_listeners, ProcessListener, ref m_invokeLevel);
            FlushListeners();
        }

        protected override void ProcessListener(IEventListener<T> listener)
        {
            if (m_listenerToRemove.Contains(listener)) return;
            listener.Invoke(m_eventArgument);
        }
    }
}