using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;
using static UnityEditor.Progress;

public class BinaryHeap<T>
{
    private T[] buffer;

    private Dictionary<T, int> keyIndexPairs = new();

    private int N = 0; //last used index of the buffer
    private int size => N;

    private IMyComparer<T> comparer;

    public BinaryHeap(IMyComparer<T> comparer, int starting_size = 1)
    {
        buffer = new T[starting_size+1];
        this.comparer = comparer;
    }

    #region Buffer Size Modification



    public void UpdateCapacity(int capacity)
    {
        var copy = new T[capacity];

        for (int i = 0; i <= N; i++)
        {
            copy[i] = buffer[i];
        }

        buffer = copy;
    }

    private void IncrementSize()
    {
        if (N >= buffer.Length / 2)
        {
            UpdateCapacity(buffer.Length * 2);
        }
        N++;
    }

    private void DecrementSize()
    {
        OnRemove(buffer[N]);
        SetValue(N--, default);
        
    }

    #endregion

    #region External Buffer Interaction

    public void Push(T item)
    {
        if (ContainsItem(item)) return;

        IncrementSize();


        OnAdd(item,N);
        SetValue(N, item);
        
        //buffer[N] = item;

        Swim(size);
    }

    public void RemoveItem(T item)
    {
        if (!ContainsItem(item)) return;

        int index = Find(item);

        if (!NodeExists(index)) return;
        
        Swap(index, N);
        DecrementSize();

        if (!NodeExists(index)) return;

        Sink(Swim(index));
        

    }

    public void ReplaceItem(T target, T replacement)
    {
        if (!ContainsItem(target) || ContainsItem(replacement)) return;

        int index = Find(target);
        OnRemove(target);
        OnAdd(replacement, index);
        SetValue(index, replacement);

        Sink(Swim(index));
    }

    public void ReplaceIf(T target, T replacement, Func<T,T,bool> condition)
    {
        if (condition(target, replacement))
        {
            ReplaceItem(target, replacement);
        }
    }

    public T PopFirst()
    {
        T result = PeekFirst();

        Swap(First(), N);
        
        DecrementSize();

        Sink(First());

        return result;
    }

    public bool TryPopFirst(out T result)
    {
        if(!TryPeekFirst(out result)) return false;

        result = PopFirst();

        return true;
    }

    

    public T PeekFirst()
    {
        return buffer[First()];
    }

    public bool TryPeekFirst(out T result)
    {
        if (IsEmpty())
        {
            result = default(T);
            return false; 
        }

        result = buffer[First()];
        return true;
    }

    public bool IsEmpty()
    {
        return N < First();
    }

    public bool ContainsItem(T item)
    {
        if (item == null) return false;

        return keyIndexPairs.ContainsKey(item);
    }

    #endregion

    #region Internal Buffer Interaction

    private void OnAdd(T val, int index)
    {
        keyIndexPairs.Add(val, index);
    }

    private void OnRemove(T val)
    {
        keyIndexPairs.Remove(val);
    }

    private void Swap(int index_a, int index_b)
    {
        var temp = buffer[index_a];
        SetValue(index_a, buffer[index_b]);
        SetValue(index_b, temp);
    }

    private void SetValue(int index, T val)
    {
        buffer[index] = val;

        if (ContainsItem(val))
        {
            keyIndexPairs[val] = index;
        }
    }

    //private void UpdateValue(T value, int index)
    //{
    //    buffer[index] = value;
    //    keyIndexPairs[value] = index;
    //}
    
    private int Swim(int index)
    {
        while (index > First() && Less(GetParentIndex(index), index))
        {
            Swap(index, GetParentIndex(index));

            index = GetParentIndex(index);
        }

        return index;
    }

    private int Sink(int index)
    {
        while(GetLeftChildIndex(index) <= size)
        {
            int j = GetLeftChildIndex(index);

            if (j < size && Less(j, j + 1)) j++;
            if (!Less(index, j)) break;
            Swap(index, j);
            index = j;

        }

        return index;
    }

    

    #endregion

    #region Node Comparisions

    private int Compare(T a, T b)
    {
        return comparer.Compare(a, b);
    }

    private bool NodeExists(int index)
    {
        if(index < First() || index > N) return false;

        if (buffer[index] == null) return false;


        return true;

    }

    private bool Less(int a, int b)
    {
        return Compare(buffer[a], buffer[b]) < 0;
    }

    

    #endregion

    



    #region Buffer Navigation

    private int First()
    {
        return 1;
    }

    private int Find(T item) 
    {
        if (keyIndexPairs.TryGetValue(item, out var index)) return index;

        //for(int i = 0; i <= N; i++) // slow O(N)
        //{
        //    if (buffer[i].Equals(item)) return i;
        //}        

        return -1;
    }

    private int GetLeftChildIndex(int k)
    {
        return k * 2;
    }

    private int GetRightChildIndex(int k)
    {
        return k * 2 + 1;
    }

    private int GetParentIndex(int k)
    {
        return Mathf.FloorToInt(k / 2f);
    }



    #endregion
}

public class SimpleLamdaComparer<T> : IMyComparer<T>
{
    private Func<T, T, int> lamda_func;
    public SimpleLamdaComparer(Func<T, T, int> lamda_func)
    {
        this.lamda_func = lamda_func;
    }

    public int Compare(T a, T b)
    {
        return lamda_func(a, b);
    }
}

public interface IMyComparer<T>
{
    public int Compare(T a, T b);
}