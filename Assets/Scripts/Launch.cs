using System.Collections;
using UnityEngine;

public class Launch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitApplicationBeforeSceneLoad()
    {
        // 初始化全局配置
        InitConfig();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitApplicationAfterSceneLoad()
    {
        // 初始化全局控制器

        // 初始化默认场景
        SceneMgr.InitScene();
    }

    static void InitConfig()
    {
       
    }
}