using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

public class UnityJavaUIFuncQueue : MonoBehaviour {

    static UnityJavaUIFuncQueue instance;
    public static UnityJavaUIFuncQueue GetInstance()
    {
        if (instance)
            return instance;
        else
            Debug.Log("Warning: UnityJavaUIFuncQueue not initialised for UI updates");
        return null;
    }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.LogError("Attempt to create duplicate UnityJavaUIFuncQueue instance");
            DestroyImmediate(this);
        }
    }

    // Use this for initialization
    void Start ()
    {
        ExampleMainThreadCall();
    }
	
	// Update is called once per frame
	void Update () {

        //if (queue.Count > 0)
        //{
        //    ProcessQueue(); //one method call per frame
        //}
    }

    public void ExampleMainThreadCall()
    {
        Debug.Log(Thread.CurrentThread.Name + ": dispatching test UI call");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(SetText, newtext));
    }

    [SerializeField] Text testText;
    [SerializeField] string newtext;
    void SetText(string newtext)
    {
        testText.text = newtext;
    }

    //generic 1 param
    public void QueueUIMethod<T>(Action<T> method, T param)
    {
        if (UnityGameController.DebugLogEnqueueing)
            Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method, param));
    }
    public IEnumerator EnumeratorWrapper<T>(Action<T> a, T param)
    {
        a(param);
        yield return null;
    }
    
    //generic 2 param
    public void QueueUIMethod<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2)
    {
        Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method, param1, param2));
    }
    public IEnumerator EnumeratorWrapper<T1, T2>(Action<T1, T2> a, T1 param1, T2 param2)
    {
        a(param1, param2);
        yield return null;
    }

    //generic 3 param
    public void QueueUIMethod<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3)
    {
        Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method, param1, param2, param3));
    }
    public IEnumerator EnumeratorWrapper<T1, T2, T3>(Action<T1, T2, T3> a, T1 param1, T2 param2, T3 param3)
    {
        a(param1, param2, param3);
        yield return null;
    }

    //void with no params
    public void QueueUIMethod(Action method)
    {
        Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method));
    }
    public IEnumerator EnumeratorWrapper(Action a)
    {
        a();
        yield return null;
    }

    //void with 1 string param (setText)
    public void QueueUIMethod(Action<string> method, string param)
    {
        if (UnityGameController.DebugLogEnqueueing)
            Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method, param));
    }
    public IEnumerator EnumeratorWrapper(Action<string> a, string param)
    {
        a(param);
        yield return null;
    }

    //void with 1 int param (setAlignment)
    public void QueueUIMethod(Action<int> method, int param)
    {
        Debug.Log("<color=blue> Queueing UI method: " + method + "</color>");
        UnityMainThreadDispatcher.Instance().Enqueue(EnumeratorWrapper(method, param));
    }
    public IEnumerator EnumeratorWrapper(Action<int> a, int param)
    {
        a(param);
        yield return null;
    }
}
