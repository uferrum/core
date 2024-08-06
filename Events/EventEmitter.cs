using System;
using System.Collections.Generic;

namespace Ferrum.Events
{

    public class EventEmitter
    {
        private Dictionary<string, List<Delegate>> eventDictionary = new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// Registers an event listener.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The event listener to register.</param>
        public void On(string eventName, Delegate listener)
        {
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = new List<Delegate>();
            }

            eventDictionary[eventName].Add(listener);
        }

        /// <summary>
        /// Registers a one-time event listener.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The one-time event listener to register.</param>
        public void Once(string eventName, Delegate listener)
        {
            Delegate wrapper = null;
            if (listener is Action)
            {
                wrapper = new Action(() =>
                {
                    listener.DynamicInvoke();
                    Off(eventName, wrapper);
                });
            }
            else if (listener is Delegate)
            {
                var method = listener.GetType().GetMethod("Invoke");
                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {
                    wrapper = Delegate.CreateDelegate(listener.GetType(), listener.Target, method, false);
                    var wrapped = wrapper;
                    wrapper = Delegate.Combine(wrapper, new Action(() =>
                    {
                        Off(eventName, wrapped);
                    })) as Delegate;
                }
            }

            On(eventName, wrapper);
        }

        /// <summary>
        /// Emits an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="args">The arguments to pass to the event listeners.</param>
        public void Emit(string eventName, params object[] args)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                foreach (var listener in eventDictionary[eventName])
                {
                    listener.DynamicInvoke(args);
                }
            }
        }

        /// <summary>
        /// Removes an event listener.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The event listener to remove.</param>
        public void Off(string eventName, Delegate listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName].Remove(listener);
                if (eventDictionary[eventName].Count == 0)
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// Removes all listeners for an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        public void RemoveAllListeners(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary.Remove(eventName);
            }
        }

        /// <summary>
        /// Gets the listener count for an event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>The number of listeners for the event.</returns>
        public int ListenerCount(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                return eventDictionary[eventName].Count;
            }
            return 0;
        }

        /// <summary>
        /// Gets all event names.
        /// </summary>
        /// <returns>A list of all event names that have listeners.</returns>
        public List<string> EventNames()
        {
            return new List<string>(eventDictionary.Keys);
        }
    }

}