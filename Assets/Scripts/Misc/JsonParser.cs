using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Done
//Third party JSON parser
public static class JsonParser
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.data;
    }



    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.data = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static string ToJson<T>(T data)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        T[] dataArray = { data };

        wrapper.data = dataArray;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.data = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] data;
    }
}


