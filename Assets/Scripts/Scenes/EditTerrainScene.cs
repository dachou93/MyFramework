using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTerrainScene : IScene
{
    string sceneName = "EditTerrain";

    public string[] GetPreloadAssetsPath()
    {
        return new string[]{
            "AssetBundleData/Prefabs/Terrain/Building/2cengfangzi.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/cn_bui_nc_house01.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/cn_bui_nc_house06.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/cn_bui_TScountryHouse08.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/cn_bui_ZW_xiaofangzi.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/gaoyadian.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/menwei.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi02.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi03.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi04.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi05.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi06.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi07.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/xiaofangzi08.prefab",
            "AssetBundleData/Prefabs/Terrain/Building/yabi.prefab",
            "AssetBundleData/Prefabs/Terrain/Tree/Broadleaf_Desktop.prefab",
            "AssetBundleData/Prefabs/Terrain/Tree/Broadleaf_Mobile.prefab",
            "AssetBundleData/Prefabs/Terrain/Tree/Conifer_Desktop.prefab",
        };
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public void OnEnter()
    {
        ////CommandDataLinkController orderController = new CommandDataLinkController();

        ////ProtoHandleController protoHandleController = new ProtoHandleController();

        ////GroupController groupController = new GroupController();

        ////UnitInfoController unitInfoController = new UnitInfoController();

        //CombatConceptToolController combatConceptToolController = new CombatConceptToolController();

        //GISDataSearchController gisDataSearchController = new GISDataSearchController();

        // ForceInquiryController forceInquiryController = new ForceInquiryController();
        ////MeasurementController measurementController = new MeasurementController();
        
        //InquireController inquireController = new InquireController();

        //GlobalInformationController globalInfoController = new GlobalInformationController();
        //globalInfoController.ShowButtonTrans(false);

        ////LinkLongPanController linkLongPanController = new LinkLongPanController();

        ////SituationFunctionController situationFunctionContreoller = new SituationFunctionController();

        ////MeasurementController measureController = new MeasurementController();

        //LayerController layerController = new LayerController();

        ////DirectViewController commandViewController = new DirectViewController();

        ////EquiptmentAnalyzeController equiptmentAnalyzeController = new EquiptmentAnalyzeController();

        ////ResultPlayController resultPlayController = new ResultPlayController();

        ////EditController editController = new EditController();
        ////EnvironmentController environmentController = new EnvironmentController();
        ////MessageMgr.Instance.SendMsg("SWITCH_ENVIRRONMENTVIEW");

        //// 地形编辑
        //TerrainEditorController terrainEditorController = new TerrainEditorController();

        //// 地名编辑
        //TerrainNameEditorController terrainNameEditorController = new TerrainNameEditorController();

        ////河流编辑
        //RiverEditController riverEditController = new RiverEditController();

        //RoadEditController roadEditController = new RoadEditController();

        //// 地名列表
        //TerrainNameListController terrainNameListController = new TerrainNameListController();

        //// 植被分析
        //DetailAnalyseController detailAnalyseController = new DetailAnalyseController();

        //// 河流分析
        //RiverAnalyseController riverAnalyseController = new RiverAnalyseController();

        //// 土壤分析
        //SoildAnalyseController soildAnalyseController = new SoildAnalyseController();
    }

    public void OnExit()
    {

    }

    public void PreloadAssetCallback(string assetPath, object asset, string assetType)
    {

    }
}
