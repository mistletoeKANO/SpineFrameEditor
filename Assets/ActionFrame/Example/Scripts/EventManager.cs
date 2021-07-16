using System;
using System.Collections.Generic;

namespace ActionFrame.Runtime
{
    public delegate void EventHandle();
    public delegate void EventHandle<T>(T e);
    public class EventManager
    {
        private static EventManager m_Instance;
        public static EventManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new EventManager();
                }
                return m_Instance;
            }
        }
        
        private Dictionary<Type, Delegate> m_HandleDic = new Dictionary<Type, Delegate>();

        public void AddEventListener<T>(EventHandle e) where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                this.m_HandleDic.Add(typeof(T), e);
                return;
            }
            this.m_HandleDic[typeof(T)] = (EventHandle)this.m_HandleDic[typeof(T)] + e;
        }
        
        public void AddEventListener<T>(EventHandle<T> e) where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                this.m_HandleDic.Add(typeof(T), e);
                return;
            }
            this.m_HandleDic[typeof(T)] = (EventHandle<T>)this.m_HandleDic[typeof(T)] + e;
        }
        
        public void RemoveEventListener<T>(EventHandle e) where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                return;
            }
            if (this.m_HandleDic[typeof(T)] == null)
            {
                return;
            }
            
            this.m_HandleDic[typeof(T)] = (EventHandle)this.m_HandleDic[typeof(T)] - e;
        }
        
        public void RemoveEventListener<T>(EventHandle<T> e) where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                return;
            }
            if (this.m_HandleDic[typeof(T)] == null)
            {
                return;
            }
            
            this.m_HandleDic[typeof(T)] = (EventHandle<T>)this.m_HandleDic[typeof(T)] - e;
        }

        public void InvokeHandle<T>() where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                return;
            }
            ((EventHandle)this.m_HandleDic[typeof(T)])?.Invoke();
        }
        
        public void InvokeHandle<T>(T e) where T : IEventHandle
        {
            if (!this.m_HandleDic.ContainsKey(typeof(T)))
            {
                return;
            }
            ((EventHandle<T>)this.m_HandleDic[typeof(T)])?.Invoke(e);
        }
    }
}