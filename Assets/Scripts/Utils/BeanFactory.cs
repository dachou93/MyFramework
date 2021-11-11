using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class BeanFactory 
{
    /// <summary>
    /// 自动注入
    /// </summary>
    /// <param name="t">类型</param>
    /// <param name="obj">实例</param>
    public static void DoAutoWire(Type t,object obj)
    {
        FieldInfo[] infos = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (infos == null || infos.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < infos.Length; i++)
        {
            Attribute attr = infos[i].GetCustomAttribute(typeof(AutoWired), false);
            if (attr == null)
                continue;
            AutoWired autoWired = (AutoWired)attr;
            object o = null;
            //Component相关只能出现在View中
            if (infos[i].FieldType.IsSubclassOf(typeof(Component)))
            {
                if(!(obj is View))
                {
                    Debug.LogError(obj+"类，"+infos[i].Name+"字段注入失败");
                    continue;
                }
                View v = obj as View;
                Transform tran = v.root.Find(autoWired.UiPathInParent);

                if (autoWired.UIBindEvent == UIBindEvent.Get_Component)
                    o = tran.GetComponent(infos[i].FieldType);
                else if (autoWired.UIBindEvent == UIBindEvent.Get_Components)
                    o = tran.GetComponents(infos[i].FieldType);
                infos[i].SetValue(obj, o);
            }
            else if (infos[i].FieldType.IsSubclassOf(typeof(Model)))
            {
                o = ModelMgr.Instance.GetModelByType(infos[i].FieldType);
                infos[i].SetValue(obj, o);
            }
            else if (infos[i].FieldType.IsSubclassOf(typeof(View)))
            {
                View ov; 
                ViewMgr.Instance.TryGetView(infos[i].FieldType.ToString(), out ov);
                
                if (ov != null && ov.IsGlobal)
                {
                    //如果有view并且是全局的就直接设置
                    infos[i].SetValue(obj, ov);
                }
                else
                {
                    object v = Activator.CreateInstance(infos[i].FieldType);
                    ViewMgr.Instance.AddView(v as View, infos[i].FieldType.ToString());
                    infos[i].SetValue(obj, v);
                }
            }
            else
            {
                infos[i].SetValue(obj, Activator.CreateInstance(infos[i].FieldType));
            }
        }

    }

    /// <summary>
    /// View路径和其他参数配置
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="o">实例</param>
    /// <returns></returns>
    public static View DoViewConfig(Type type,object o)
    {
        ViewPath viewPath = type.GetCustomAttribute<ViewPath>(false);
        if (viewPath == null)
            return null;
        View view = (View)o;
        view.ViewConfig.parentPath = viewPath.ParentPath;
        view.ViewConfig.prefabPath = viewPath.PrefabPath;
        view.ViewConfig.parentHierarchyInParentView = viewPath.ParentHierarchyInParentView;
        view.ViewConfig.rootHierarchyInParentView = viewPath.RootHierarchyInParentView;
        IsGlobal isGlobal = type.GetCustomAttribute<IsGlobal>();
        if (isGlobal!=null)
        {
            view.ViewConfig.isGlobal = true;
        }
        IsSubView isSubView = type.GetCustomAttribute<IsSubView>();
        if (isSubView != null)
        {
            view.ViewConfig.isSubView = true;
        }
        view.ParseViewConfig();
        return view;
    }

    /// <summary>
    /// ViewUI事件注册
    /// </summary>
    /// <param name="t">类型</param>
    /// <param name="obj">实例</param>
    public static void DoViewEvent(Type t, object obj)
    {
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (methods == null || methods.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < methods.Length; i++)
        {
            ViewMethod viewMethod= methods[i].GetCustomAttribute<ViewMethod>(false);
            if (viewMethod == null)
                continue;
            FieldInfo field = t.GetField(viewMethod.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError("ui绑定字段为空");
            }
            object val = field.GetValue(obj);
            if (!val.GetType().IsSubclassOf(typeof(Component)))
                continue;
            UIEventBindMgr.Instance.BindUIEvent(((Component)val).gameObject, viewMethod.UIBindEvent, obj, methods[i].Name);
        }
    }

    /// <summary>
    /// Msg处理方法注册
    /// </summary>
    /// <param name="t">类型</param>
    /// <param name="obj">实例</param>
    public static void DoMsgRegister(Type t, object obj)
    {
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (methods == null || methods.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < methods.Length; i++)
        {
            MsgRegister msgRegister = methods[i].GetCustomAttribute<MsgRegister>(false);
            if (msgRegister == null)
                continue;

            Delegate d = CreateDelegate(obj, methods[i]);
            MessageMgr.Instance.AddListener(msgRegister.Msg,d);
        }

    }

    public static void DoUpdateCall(object obj)
    {
        if (!typeof(IUpdateAble).IsInstanceOfType(obj))
            return;
        Action a = delegate {
            try
            {
                ((IUpdateAble)obj).Update();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        };
        Scheduler.Instance.AddUpdateEvent(a);
    }

    public static void DoFixedUpdateCall(object obj)
    {
        if (!typeof(IFixedUpdateAble).IsInstanceOfType(obj))
            return;
        Action a = delegate {
            try
            {
                ((IFixedUpdateAble)obj).FixedUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        };
        Scheduler.Instance.AddFixedUpdateEvent(a);
    }

    /// <summary>
    /// 动态创建委托
    /// </summary>
    /// <param name="instance">实例</param>
    /// <param name="method">方法信息</param>
    /// <returns></returns>
    private static Delegate CreateDelegate(object instance,MethodInfo method)
    {
        var parametersInfo = method.GetParameters();
        Expression[] expArgs = new Expression[parametersInfo.Length];
        List<ParameterExpression> lstParamExpression = new List<ParameterExpression>();
        for (int i = 0; i < expArgs.Length; i++)
        {
            expArgs[i] = Expression.Parameter(parametersInfo[i].ParameterType, parametersInfo[i].Name);
            lstParamExpression.Add((ParameterExpression)expArgs[i]);
        }
        MethodCallExpression callExpression = Expression.Call(Expression.Constant(instance), method, expArgs);
        LambdaExpression lambdaExpression = Expression.Lambda(callExpression, lstParamExpression);
        return lambdaExpression.Compile();
    }
}
