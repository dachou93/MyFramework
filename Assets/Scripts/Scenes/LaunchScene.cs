using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchScene : IScene
{
    public string[] GetPreloadAssetsPath() => null;

    public string GetSceneName() => "Launch";

    public void OnEnter() 
    {
        CorutineInstance.Instance.StartCoroutine(PlayStartAction());
    }

    public void OnExit() { }

    public void PreloadAssetCallback(string assetPath, object asset, string assetType) { }


    IEnumerator PlayStartAction()
    {
        yield return null;

        Scene scene = SceneManager.GetActiveScene();

        foreach (var item in scene.GetRootGameObjects())
        {
            if (item.name == "TopCanvas")
            {
                TextMeshProUGUI text = item.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();

                if (text)
                {
                    for (int i = 1; i > 0; i--)
                    {
                        text.text = "正在播放片头动画，请稍等。。。  " + i;
                        yield return new WaitForSeconds(1);
                    }

                    SceneMgr.LoadScene(new HomeScene());
                }
            }
        }
    }
}
