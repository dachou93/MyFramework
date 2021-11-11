using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[AttributeUsage(AttributeTargets.Class)]
public class ViewPath :Attribute
{
    public ViewPath(string parentPath, string prefabPath, string rootHierarchyInParentView = null, string parentHierarchyInParentView = null)
    {
        ParentPath = parentPath;
        PrefabPath = prefabPath;
        RootHierarchyInParentView = rootHierarchyInParentView;
        ParentHierarchyInParentView = parentHierarchyInParentView;
    }
    public string ParentPath { get; set; }

    public string PrefabPath { get; set; }

    public string RootHierarchyInParentView { get; set; }

    public string ParentHierarchyInParentView { get; set; }

   
}
