using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Utilities.General.Events.Core
{
    public abstract class Event<T> : ScriptableObject, IEvent where T : IEventListener
    {
        protected const string Invoke_Log_Format = "[<color=#{0}>Event</color>] {1} invoked!";

        [SerializeField] protected bool m_enableLogging = false;
        [SerializeField] protected Color m_color = new Color(0f, 0f, 0f, 1f);
        
        protected HashSet<T> m_listeners = new HashSet<T>(30);
        protected HashSet<T> m_listenerToRemove = new HashSet<T>(30);
        protected HashSet<T> m_listenersToAdd = new HashSet<T>(30);

        protected int m_invokeLevel = 0;
        
        public virtual void Invoke()
        {
            LogEventInvoke();
            InvokeListeners(m_listeners, ProcessListener, ref m_invokeLevel);
            FlushListeners();
        }

        protected virtual void ProcessListener(T listener)
        {
            if(m_listenerToRemove.Contains(listener)) return;
            listener.Invoke();
        }

        protected void InvokeListeners(HashSet<T> listeners, Action<T> invoker, ref int invokeLevel)
        {
            invokeLevel++;
            foreach (var listener in listeners)
            {
                try
                {
                    invoker(listener);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            invokeLevel--;
        }

        public void Subscribe(T listener)
        {
            if (m_invokeLevel > 0)
            {
                m_listenerToRemove.Remove(listener);
                m_listenersToAdd.Add(listener);
                return;
            }
            m_listeners.Add(listener);
        }

        public void Unsubscribe(T listener)
        {
            if (m_invokeLevel > 0)
            {
                m_listenerToRemove.Add(listener);
                m_listenersToAdd.Remove(listener);
                return;
            }
            m_listeners.Remove(listener);
        }

        protected void FlushListeners()
        {
            if (m_invokeLevel > 0) return;
            
            foreach (var listener in m_listenerToRemove)
                m_listeners.Remove(listener);
            foreach (var listener in m_listenersToAdd)
                m_listeners.Add(listener);
        } 

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        protected void LogEventInvoke()
        {
            if (!m_enableLogging) return;
            var colorHexValue = ColorUtility.ToHtmlStringRGB(m_color);
            Debug.LogFormat(Invoke_Log_Format, colorHexValue, name);
        }
    }
}