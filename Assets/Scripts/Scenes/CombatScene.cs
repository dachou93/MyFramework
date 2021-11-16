using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScene: IScene
{
    string sceneName = "Combat";
    public string[] GetPreloadAssetsPath()
    {
        return null;
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public void OnEnter()
    {
        //CommandDataLinkController orderController = new CommandDataLinkController();

        //GroupController groupController = new GroupController();

        //UnitInfoController unitInfoController = new UnitInfoController();

        //MeasurementController measureController = new MeasurementController();

        //LayerController layerController = new LayerController();

        //CombatConceptToolController combatConceptToolController = new CombatConceptToolController();

        //GlobalInformationController globalInfoController = new GlobalInformationController();


        //globalInfoController.ShowButtonTrans();
    }

    public void OnExit()
    {

    }

    public void PreloadAssetCallback(string assetPath, object asset, string assetType)
    {

    }

}