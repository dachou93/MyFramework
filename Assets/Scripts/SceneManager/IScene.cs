using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScene
{
    string GetSceneName();

    void OnEnter();

    void OnExit();

    string[] GetPreloadAssetsPath();

    void PreloadAssetCallback(string assetPath, object asset, string assetType);
}
