using UnityEngine.SceneManagement;

public class HomeScene : IScene
{
    string name = "Home";

    public string[] GetPreloadAssetsPath() => null;

    public string GetSceneName() => name;

    public void OnEnter()
    {
        //DigitalEarthSystem.RenderSettings. programName = "Null";
        //MainPanelController mainPanelController = new MainPanelController();
        //UserInfoController userInfoController = new UserInfoController();

        //FileHandleController fileHandleController = new FileHandleController();

        //ModelManagementController modelManagementController = new ModelManagementController();
        //CombatOperationController combatOperationController = new CombatOperationController();
        //PersonnelManagerController personnelManagerController = new PersonnelManagerController();
        //NavigationBarController navigationBarController = new NavigationBarController();
        //CombatEntityController combatEntityController = new CombatEntityController();
        //TipController tipController = new TipController();
        //LeadinTipController leadinTipController = new LeadinTipController();

        ////TestDM testdm = new TestDM();
        ////DMWrite dMWrite = new DMWrite();

        //BasicDataController basicdataController = new BasicDataController();
        //StaffingDataController staffingController = new StaffingDataController();
        //SoliderDataController soliderDataController = new SoliderDataController();
        //CompiledDataController compliedDataController = new CompiledDataController();
        ////MapManagerController mapDataController = new MapManagerController();
        //ConfigureController configureController = new ConfigureController();

        //NewMapManagerController mapDataController = new NewMapManagerController();
        //BattlefieldEnvironmentController battlefieldEnvironmentController = new BattlefieldEnvironmentController();

        //ModelDataController modelDataController = new ModelDataController();
        //SystemDataManagementController systemController = new SystemDataManagementController();

        //Loom loom = Loom.Instance;
    }

    public void OnExit() { }

    public void PreloadAssetCallback(string assetPath, object asset, string assetType) { }
}
