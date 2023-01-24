using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, IGameService> servicesDictionary = new Dictionary<Type, IGameService>();

    public static void Register(Type t, IGameService service)
    {
        servicesDictionary[t] = service;
    }

    public static void RemoveService(Type t)
    {
        if(servicesDictionary.ContainsKey(t))
            servicesDictionary.Remove(t);
    }

    public static T Resolve<T>() where T : IGameService
    {
        if (!servicesDictionary.ContainsKey(typeof(T)))
        {
            Debug.LogError("Error : Couldn't find " + typeof(T) + " panel in service locator");
            return default;
        }

        return (T)servicesDictionary[typeof(T)];
    }
}

public interface IGameService
{

}
