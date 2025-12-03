using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObservableValue 
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class ObservableList<T> : IEnumerable<T>
{
    private List<T> innerList = new List<T>();
    public event Action OnListChanged;

    public void Add(T item)
    {
        innerList.Add(item);
        OnListChanged?.Invoke();
    }

    public void Invoke()
    {
        OnListChanged?.Invoke();
    }



    public bool Remove(T item)
    {
        bool result = innerList.Remove(item);
        if (result) OnListChanged?.Invoke();
        return result;
    }

    public void Clear()
    {
        innerList.Clear();
        OnListChanged?.Invoke();
    }

    public bool Any(Func<T, bool> predicate)
    {
        return innerList.Any(predicate);
    }

    public T this[int index] => innerList[index];

    public int Count => innerList.Count;

    public IEnumerator<T> GetEnumerator() => innerList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => innerList.GetEnumerator();
}


public class ObservableValue<T>
{
    private T _value;
    public event Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }
    public ObservableValue(T initialValue = default)
    {
        _value = initialValue;
    }
}
