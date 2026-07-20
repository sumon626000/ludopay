using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static string JsonToString(string target, string s)
    {

        string[] newString = Regex.Split(target, s);

        return newString[1];

    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    public static void AddOnClickEvent(MonoBehaviour target, UIButton btn, string method, object value, Type type)
    {
        EventDelegate onClickEvent = new EventDelegate(target, method);
        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.value = value;
        param.expectedType = type;
        onClickEvent.parameters[0] = param;
        EventDelegate.Add(btn.onClick, onClickEvent);
    }

    public static void AddOnClickTriggerEvent(MonoBehaviour target, UIEventTrigger btn, string method, object value, Type type)
    {
        EventDelegate onClickEvent = new EventDelegate(target, method);
        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.value = value;
        param.expectedType = type;
        onClickEvent.parameters[0] = param;
        EventDelegate.Add(btn.onClick, onClickEvent);
    }

    
}
