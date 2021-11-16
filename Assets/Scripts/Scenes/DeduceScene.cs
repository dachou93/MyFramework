using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 推演场景
/// </summary>
public class DeduceScene : IScene
{
    public string[] GetPreloadAssetsPath() => null;

    public string GetSceneName() => "Deduce";

    public void OnEnter()
    {
        //GlobalInformationController globalInfoController = new GlobalInformationController();
        //globalInfoController.ShowButtonTrans();

        //FileHandleController fileHandleController = new FileHandleController();

        //ProgrammeController programmeController = new ProgrammeController();
        //TipController tipController = new TipController();
        //MarshallingController marshallingController = new MarshallingController();
        //StaffingDataController staffingDataController = new StaffingDataController();
        //DeduceGanttController deduceGanttController = new DeduceGanttController();
        //DirectViewController commandViewController = new DirectViewController();
        //CommunicationViewController communicationViewController = new CommunicationViewController();
        ////EnvironmentController environmentController = new EnvironmentController();
        //CombatConceptToolController combatConceptToolController = new CombatConceptToolController();

        //// 植被分析
        //DetailAnalyseController Controller = new DetailAnalyseController();

        //// 河流分析
        //RiverAnalyseController riverAnalyseController = new RiverAnalyseController();

        //// 土壤分析
        //SoildAnalyseController soildAnalyseController = new SoildAnalyseController();

        //LayerController layerController = new LayerController();
        //MeasurementController measurementController = new MeasurementController();
        //ChoseTargetController choseTargetController = new ChoseTargetController();
        //GroupController groupController = new GroupController();
        //SetRecordController setRecordController = new SetRecordController();
        //groupController.canCreate = true;

        //GroupInfoController groupInfoController = new GroupInfoController();

        //SchemeEditorController schemeEditorController = new SchemeEditorController();

        //GISDataSearchController gisDataSearchController = new GISDataSearchController();

        //EditController editController = new EditController();

        //ForceInquiryController forceInquiryController = new ForceInquiryController();

        //SynthesizeMessageController synthesizeMessageController = new SynthesizeMessageController();

        //VirtualInfoController virtualInfoController = new VirtualInfoController();
        //InquireController inquireController = new InquireController();

        //// 时空冲突分析
        ////SpaceTimeConflictController spaceTimeConflictController = new SpaceTimeConflictController();
        //UpdataWeatherController updataWeather = new UpdataWeatherController();

        //DetailAnalyseController detailAnalyseController = new DetailAnalyseController();

        //if (programmeController.ProgrammeModel.CurrProgrammeName != null)
        //{
        //    MessageMgr.Instance.SendMsg("INITGROUPWITHPROGRAMME", programmeController.ProgrammeModel.CurrProgramme);
        //    MessageMgr.Instance.SendMsg("OPEN_EDITVIEW");
        //    MessageMgr.Instance.SendMsg("SET_STARTTIME", programmeController.ProgrammeModel.CurrProgramme.name, programmeController.ProgrammeModel.CurrProgramme.startTime);
        //    MessageMgr.Instance.SendMsg("UPDATA_WIND", programmeController.ProgrammeModel.CurrProgramme.weather, "", "");
        //}

        //MessageMgr.Instance.SendMsg("SHOW_PLANEDIT");

        //ElementDrawSystem.Instance.RecoverJB();
    }

    public void OnExit()
    {
        //Debug.Log("退出 推演场景");
        //ElementDrawSystem.Instance.ClearJB();
    }

    public void PreloadAssetCallback(string assetPath, object asset, string assetType)
    {

    }
}
