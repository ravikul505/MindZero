using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EventBusUtil{
    public static IReadOnlyList<Type> EventTypes {get; set;}
    public static IReadOnlyList<Type> EventBusTypes {get; set;}

#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeState{get;set;}
    [InitializeOnLoadMethod]
    public static void InitializeEditor(){
        EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
        EditorApplication.playModeStateChanged += onPlayModeStateChanged;
    }
    private static void onPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeState = state;
        if(state == PlayModeStateChange.ExitingPlayMode){
            ClearAllBuses();
        }
    }


#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize(){
        EventTypes  =   PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
        EventBusTypes = InitializeAllBuses();
    }
    private static List<Type> InitializeAllBuses(){
        List<Type> eventBusTypes = new List<Type>();
        var typedef = typeof(EventBus<>);
        foreach(var eventType in EventTypes){
            var busType = typedef.MakeGenericType(eventType);
            eventBusTypes.Add(busType);
            Debug.Log($"initialized EventBud<{eventType.Name}");
        }
        return eventBusTypes;
    }
    public static void ClearAllBuses(){
        Debug.Log("clearing all buses");
        for(int i =0 ;i < EventBusTypes.Count; ++i){
            var busType = EventBusTypes[i];
            var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
            clearMethod.Invoke(null, null);
        }
    }
 }
