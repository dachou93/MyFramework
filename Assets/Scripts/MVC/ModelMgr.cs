using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModelMgr : Singleton<ModelMgr>
{
    Dictionary<string, Model> modelMap = new Dictionary<string, Model>();

    private ModelMgr()
    {
        SceneManager.sceneUnloaded += (Scene scene) =>
        {
            List<string> removeModels = new List<string>();

            // 销毁非全局view
            foreach (var model in modelMap.Values)
            {
                if (!model.isGobal)
                {
                    removeModels.Add(model.GetType().ToString());
                }
            }

            foreach (var modelName in removeModels)
            {
                modelMap.Remove(modelName);
            }
        };
    }

    public T Create<T>() where T : Model, new()
    {
        string id = typeof(T).ToString();

        if (modelMap.TryGetValue(id, out Model model))
        {
            return model as T;
        }

        model = new T();
        AddModel(model, typeof(T).ToString());

        return model as T;
    }

    void AddModel(Model model, string id)
    {
        modelMap.Add(id, model);
    }

    public T GetModelByType<T>() where T : Model, new()
    {
        return Create<T>();
    }


    public Model GetModelByType(Type type)
    {
        string id= type.ToString();
        if (modelMap.TryGetValue(id, out Model model))
        {
            return model;
        }

        model = (Model)Activator.CreateInstance(type);

        IsGlobal isGlobal = type.GetCustomAttribute<IsGlobal>();
        if (isGlobal != null)
        {
            model.isGobal = true;
        }

        AddModel(model, type.ToString());

        return model;
    }
}
