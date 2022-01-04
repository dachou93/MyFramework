using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TBTBaseNode
{
    private List<TBTBaseNode> children;
     
	public TBTBaseNode()
    {
        children = new List<TBTBaseNode>();
    }
    public TBTBaseNode AddChild(TBTBaseNode node)
    {
        children.Add(node);
        return this;
    }
    public int GetChildCount()
    {
        return children.Count;
    }
    public T GetChild<T>(int index)where T:TBTBaseNode
    {
        return (T)children[index];
    }
}
