using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewMgr :Singleton<ViewMgr>
{
    Dictionary<string, View> viewMap = new Dictionary<string, View>();

    private ViewMgr()
    {
        SceneManager.sceneUnloaded += (Scene scene) =>
        {
            List<string> removeViews = new List<string>();
           
            // 销毁非全局view
            foreach (var view in viewMap.Values)
            {
                if (!view.IsGlobal)
                {
                    removeViews.Add(view.GetType().ToString());
                    view.Destroy();
                }
            }

            foreach (var viewName in removeViews)
            {
                viewMap.Remove(viewName);
            }
        };
    }

    public T Create<T>() where T : View, new()
    {
        string id = typeof(T).ToString();

        if (viewMap.TryGetValue(id, out View view))
        {
            return view as T;
        }

        view = new T();
        AddView(view, typeof(T).ToString());

        return view as T;
    }
    public bool Create(string id, out View view)
    {
        if (viewMap.TryGetValue(id, out  view))
        {
            return true ;
        }
        object ins=  typeof(View).Assembly.CreateInstance(id) ;
        if (ins is View)
        {
            view = (View)ins;
            AddView(view, id);
            return true;
        }
        else 
        {
            return false;
        }
       
    }
    public bool TryGetView(string id ,out View view) 
    {

        return viewMap.TryGetValue(id, out view);
    }
    //public T Create<T>(string id) where T : View, new()
    //{
    //    if (viewMap.TryGetValue(id, out View view))
    //    {
    //        return view as T;
    //    }

    //    view = new T();
    //    AddView(view, id);

    //    return view as T;
    //}

    public void AddView(View view, string id)
    {
        viewMap.Add(id, view);
    }

    //public T GetViewByID<T>(string id) where T : View, new()
    //{
    //    return Create<T>(id);
    //}

    public T GetViewByType<T>() where T : View, new()
    {
        return Create<T>();
    }
}
