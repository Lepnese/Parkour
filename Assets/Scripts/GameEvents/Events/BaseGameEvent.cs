using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();

    public void Raise(T item) {
        for (int i = eventListeners.Count - 1; i >= 0; i--) {
            eventListeners[i].OnEventRaised(item);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener) {
        if (eventListeners.Contains(listener)) return;
        eventListeners.Add(listener);
    }
    
    public void UnregisterListener(IGameEventListener<T> listener) {
        if (!eventListeners.Contains(listener)) return;
        eventListeners.Remove(listener);
    }
}
