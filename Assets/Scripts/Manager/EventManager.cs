using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    //data for send
    public class EventInfo
    {
        private object[] array = null;

        public int Count { get { return array == null ? 0 : array.Length; } }

        public object this[int index]
        {
            get
            {
                if (array == null) return null;
                if (index > array.Length - 1) return null;
                return array[index];
            }
        }

        /// <summary>
        /// 事件参数类
        /// </summary>
        /// <param name="values">传递参数</param>
        public EventInfo(params object[] values)
        {
            if (null == values)
                return;

            array = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i];
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="index">参数下标0开始,默认为0</param>
        /// <returns>参数值</returns>
        public T Param<T>(int index = 0)
        {
            if (null == array || index > array.Length - 1)
                return default(T);

            return (T)array[index];
        }
    }

    public class EventManager : Singleton<EventManager>
    {
        /// <summary>
        /// 带参数的事件字典
        /// </summary>
        private Dictionary<EventType, List<Action<EventInfo>>> _events = new Dictionary<EventType, List<Action<EventInfo>>>();

        /// <summary>
        /// 无参数的事件字典
        /// </summary>
        private Dictionary<EventType, List<Action>> _events2 = new Dictionary<EventType, List<Action>>();

        

        public void Register(EventType t, Action<EventInfo> action)
        {
            List<Action<EventInfo>> _list = null;
            if (_events.ContainsKey(t))
            {
                _list = _events[t];
            } 
            if(_list == null)
            {
                _list = new List<Action<EventInfo>>();
            }
            
            if(!IsExistEvent(_list, action))
            {
                _list.Add(action);
            } else
            {
            Debug.LogError($"重复注册事件 2 EventType:{t}");
            }
            _events[t] = _list;
        }

        private bool IsExistEvent(List<Action<EventInfo>> list, Action<EventInfo> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] == action)
                {
                    return true;
                }
            }
            return false;
        }

        public void Register(EventType t, Action action)
        { 
            List<Action> _list = null;
            if (_events2.ContainsKey(t))
            {
                _list = _events2[t];
            }
            if (_list == null)
            {
                _list = new List<Action>();
            }

            if (!IsExistEvent2(_list, action))
            {
                _list.Add(action);
            }
            else
            {
                Debug.LogError($"重复注册事件 2 EventType:{t}");
            }
            _events2[t] = _list;
        }

        private bool IsExistEvent2(List<Action> list, Action action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == action)
                {
                    return true;
                }
            }
            return false;
        }


        public void Remove(EventType t, Action<EventInfo> action)
        { 
            List<Action<EventInfo>> _list = null;
            if (_events.ContainsKey(t))
            {
                _list = _events[t];
            }
            if (_list != null && _list.Count > 0)
            { 
                for(int i = _list.Count-1; i>-1; --i)
                {
                    if(_list[i] == action)
                    {
                        _list.RemoveAt(i); 
                    }
                }
            }
            _events[t] = _list;
        }

        public void Remove(EventType t, Action action)
        {
            List<Action> _list = null;
            if (_events2.ContainsKey(t))
            {
                _list = _events2[t];
            }
            if (_list != null && _list.Count > 0)
            {
                for (int i = _list.Count - 1; i > -1; --i)
                {
                    if (_list[i] == action)
                    {
                        _list.RemoveAt(i);
                    }
                }
            }
            _events2[t] = _list;
        }

        public void RemoveAll(EventType t)
        { 
            if (_events.ContainsKey(t))
            {
                if (null != _events[t])
                {
                    _events[t].Clear();
                    _events[t] = null;
                }
                _events.Remove(t);
            }

            if (_events2.ContainsKey(t))
            {
                if(null != _events2[t])
                {
                    _events2[t].Clear();
                    _events2[t] = null;
                }
                _events2.Remove(t);
            }
        }

        public void Send(EventType t, EventInfo data = null)
        {

            if (_events.ContainsKey(t) )
            {
                List<Action<EventInfo>> _list1 = _events[t];
                if(_list1 != null && _list1.Count > 0)
                {
                    for(int i=0; i < _list1.Count; i++)
                    {
                        _list1[i](data);
                    }
                }
            }

            if (_events2.ContainsKey(t) )
            {
                List<Action> _list2 = _events2[t];
                if (_list2 != null && _list2.Count > 0)
                {
                    for (int i = 0; i < _list2.Count; i++)
                    {
                        _list2[i]();
                    }
                }
            }
        }
    }

public enum EventType
{
    StartChangeScene,
    EndChangeScene,
    EndLoadConfig,
}