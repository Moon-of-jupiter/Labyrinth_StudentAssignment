using System;
using UnityEngine;

[Obsolete()]
public class TreeBufferDS<T>
{
    
}
[Obsolete()]
public class TreeNode<T> : IComparable<TreeNode<T>> where T : IComparable<T>
{
    public T value;

    public TreeNode<T> left_child;
    public TreeNode<T> right_child;
    public int CompareTo(TreeNode<T> other)
    {
        if (value == null) return -1;
        return value.CompareTo(other.value);
    }

}
