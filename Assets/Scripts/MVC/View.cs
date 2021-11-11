using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View
{
    protected ViewConfig viewConfig;

    public Transform root;

    public Transform parent;

    public bool IsVisible => root?.gameObject.activeSelf ?? false;

    List<View> childViews = new List<View>();

    public bool isShow;

    View parentView;

    public bool IsGlobal => viewConfig.isGlobal;

    public ViewConfig ViewConfig => viewConfig;

    public View(ViewConfig viewConfig=null,bool isAuto=false)
    {
        //如果是自动装配
        if (isAuto)
        {
            this.viewConfig = new ViewConfig();
            Type type = this.GetType();
            BeanFactory.DoViewConfig(type, this);
            BeanFactory.DoAutoWire(type, this);
            BeanFactory.DoViewEvent(type, this);
            return;
        }

        this.viewConfig = viewConfig;

        ParseViewConfig();
    }

    public virtual void OnShow()
    {

    }

    public virtual void OnHide()
    {

    }

    public virtual void OnDestroy()
    {

    }

    public void Switch()
    {
        if (isShow)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        root.gameObject.SetActive(true);
        isShow = true;
        OnShow();

        for (int i = 0; i < childViews.Count; i++)
        {
            childViews[i].Show();
        }
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
        isShow = false;
        OnHide();

        for (int i = 0; i < childViews.Count; i++)
        {
            childViews[i].Hide();
        }
    }

    public void Destroy()
    {
        OnDestroy();

        for (int i = 0; i < childViews.Count; i++)
        {
            childViews[i].Destroy();
        }
    }

    public T AddView<T>() where T : View, new()
    {
        T subView = new T();
        //subView.parentView = this;
        //subView.Init();
        Transform parent = root.Find(subView.viewConfig.parentHierarchyInParentView);
        subView.root.SetParent(parent, false);

        childViews.Add(subView);

        return subView;
    }

    public void RemoveView()
    {

    }

    public void ParseViewConfig()
    {
        if (!viewConfig.isSubView)
        {
            root = AssetsManager.Instantiate(AssetsManager.Load<GameObject>(viewConfig.prefabPath), viewConfig.isGlobal).transform;
            root.gameObject.SetActive(false);

            GameObject parentObj = GameObject.Find(viewConfig.parentPath);
            if (parentObj)
            {
                parent = parentObj.transform;
                root.SetParent(parent, false);
            }

            if (viewConfig.isGlobal)
            {
                GameObject.DontDestroyOnLoad(root.gameObject);
            }

            UIBind[] uiBinds = viewConfig.uiBinds;
            if (uiBinds == null)
                return;
            for (int i = 0; i < uiBinds.Length; i++)
            {
                UIBind uiBind = uiBinds[i];
                Transform childTran = root.Find(uiBind.uiPathInParent);
                Debug.Assert(childTran, uiBind.uiPathInParent);
                UIEventBindMgr.Instance.BindUIEvent(childTran.gameObject, uiBind.uiEvent, this, uiBind.bindMethodNameOrFieldName);
            }
        }
        else
        {
            root = AssetsManager.Instantiate(AssetsManager.Load<GameObject>(viewConfig.prefabPath), viewConfig.isGlobal).transform;
            root.gameObject.SetActive(false);

            UIBind[] uiBinds = viewConfig.uiBinds;
            if (uiBinds == null)
                return;
            for (int i = 0; i < uiBinds.Length; i++)
            {
                UIBind uiBind = uiBinds[i];
                Transform childTran = root.Find(uiBind.uiPathInParent);
                Debug.Assert(childTran, uiBind.uiPathInParent);
                UIEventBindMgr.Instance.BindUIEvent(childTran.gameObject, uiBind.uiEvent, this, uiBind.bindMethodNameOrFieldName);
            }
        }
    }
}

public class ViewConfig
{
    public bool isGlobal;
    public bool isSubView;
    public string parentPath;
    public string prefabPath;
    public string rootHierarchyInParentView;
    public string parentHierarchyInParentView;
    public UIBind[] uiBinds;
}