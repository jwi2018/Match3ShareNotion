using System.Collections;
using System.Collections.Generic;
using LogicStates;
using UnityEngine;
using UnityEngine.UI;

public struct ColorAndSprite
{
    public EColor color;
    public Sprite sprite;
}

public enum ExceptionList
{
    NONE = -1,
    IDTILE,
    IDGRAVITY
}

public class EditorToggleManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup SelectObjectGroup;

    [SerializeField] private EditorObjectIdListController SelectObjectListController;

    [SerializeField] private ToggleListControll SelectColorGroup;

    [SerializeField] private ToggleListControll SelectHPGroup;

    [SerializeField] private SpriteContainer SpriteContainer;

    [SerializeField] private EditorMissionListController SelectMissionObjectParent;

    [SerializeField] private EditorStageBlockController StageBlockController;

    [SerializeField] private EditorStageFoundationController StageFoundationController;

    [SerializeField] private EditorMultiMapController StageMultiMapController;

    [SerializeField] private EditorRailItemController RailItemController;

    [SerializeField] private EditorConnectController ConnectionController;

    [SerializeField] private TabletUI TabletController;

    [SerializeField] private EditorSpecialTileItemController SpecialTileItemController;

    [SerializeField] private InputField EtcInputField;

    [SerializeField] private Toggle _toggle_Auto;

    [SerializeField] private Image _active_bottom;

    [SerializeField] private GameObject _etcChangeObj;

    [SerializeField] private GameObject _stageBlockAddBtn;

    [SerializeField] private GameObject _SpecialTileShowRect;

    [SerializeField] private GameObject _cencelButton;

    [SerializeField] private GameObject _testButtonObject;

    [SerializeField] private GameObject _editorButonObject;

    [SerializeField] private GameObject _testStatusObject;

    [SerializeField] private int SelectHP = 1;

    [SerializeField] private EColor SelectColor = EColor.NONE;

    [SerializeField] private EID SelectObjectID = EID.NORMAL;

    [SerializeField] private int SelectValue;

    [SerializeField] private int EtcValue;

    [SerializeField] private ExceptionList Exception = ExceptionList.NONE;


    private SpriteContainer container;

    public int SetHP
    {
        get => SelectHP;
        set
        {
            SelectHP = value;
            ChangeColorSprites(value);
        }
    }

    public EColor SetColor
    {
        get => SelectColor;
        set => SelectColor = value;
    }

    public int SetintValue
    {
        get => SelectValue;
        set => SelectValue = value;
    }

    public int SetEtcValue
    {
        get => EtcValue;
        set => EtcValue = value;
    }

    public EID SetObjectID
    {
        get => SelectObjectID;
        set
        {
            SelectObjectID = value;
            Exception = ExceptionList.NONE;
            ChangeObject(value);
        }
    }

    public ExceptionList SetException
    {
        get => Exception;
        set
        {
            Exception = value;
            SelectObjectID = EID.NONE;
            ChangeObject();
        }
    }

    private void Awake()
    {
        container = SpriteContainer;
    }

    private void Start()
    {
        if (EditorAutoModeControll._isAutoFull) EditorAutoModeControll._isAutoMode = true;
        SetObjectID = SelectObjectID;
        SetStageBlock(EID.NORMAL, EColor.RED);

        if (EditorAutoModeControll._isAutoMode)
        {
            _toggle_Auto.isOn = true;
            AutoModeCheck(_toggle_Auto);
            if (EditorAutoModeControll._isAutoFull)
            {
                if (EditorAutoModeControll._isStartStage != StageManager.StageNumber)
                {
                    _toggle_Auto.isOn = false;
                    AutoModeCheck(_toggle_Auto);
                    _toggle_Auto.isOn = true;
                    AutoModeCheck(_toggle_Auto);
                }

                StartCoroutine(AutoStart());
            }
        }
        else
        {
            _toggle_Auto.isOn = false;
            AutoModeCheck(_toggle_Auto);
        }

        SelectIDClear();
    }

    public void ChangeObject(EID Id)
    {
        #region //PlusPopup//

        if (Id == EID.RAIL)
        {
            RailItemController.OpenRailList();
            ConnectionController.CloseButton();
            TabletController.CloseUI();
            var color = new Color(0, 0, 0, 0);
            _active_bottom.color = color;
            LogMessage();
            return;
        }

        if (Id == EID.TABLET)
        {
            RailItemController.CloseButton();
            ConnectionController.CloseButton();
            TabletController.ActiveUI();
            var color = new Color(0, 0, 0, 0);
            _active_bottom.color = color;
            return;
        }

        if (Id == EID.SPECIAL_TILE)
        {
            SelectObjectListController.OnClickButton(null);
            _SpecialTileShowRect.SetActive(true);
            RailItemController.CloseButton();
            ConnectionController.CloseButton();
            TabletController.CloseUI();
            SpecialTileItemController.ChangeItem(EtcValue);
            var color = new Color(0, 0, 0, 0.2f);
            _active_bottom.color = color;
        }
        else if (Id == EID.CONNECTION)
        {
            ConnectionController.OpenConnectionList();
            RailItemController.CloseButton();
            TabletController.CloseUI();
            _SpecialTileShowRect.SetActive(false);
            var color = new Color(0, 0, 0, 0.2f);
            _active_bottom.color = color;
        }
        else
        {
            RailItemController.CloseButton();
            ConnectionController.CloseButton();
            TabletController.CloseUI();
            _SpecialTileShowRect.SetActive(false);
            var color = new Color(0, 0, 0, 0.2f);
            _active_bottom.color = color;
        }

        #endregion

        if (Id == (EID) 41) CencelButtonActive(true);
        else if (Id == (EID) 16) CencelButtonActive(true);
        else if (Id == (EID) 17) CencelButtonActive(true);
        else if (Id == (EID) 18) CencelButtonActive(true);
        else CencelButtonActive(false);

        if (Id == EID.TIMEBOMB_ICE || Id == EID.TIMEBOMB_LAVA) _etcChangeObj.SetActive(true);
        else _etcChangeObj.SetActive(false);

        if (SelectObjectID != EID.GLASS_COLOR && SelectObjectID != EID.NORMAL && SelectObjectID != EID.OAK &&
            SelectObjectID != EID.GIFTBOX &&
            SelectObjectID != EID.TIMEBOMB_ICE && SelectObjectID != EID.TIMEBOMB_LAVA &&
            SelectObjectID != EID.CHAMELEON)
            _stageBlockAddBtn.SetActive(false);
        else _stageBlockAddBtn.SetActive(true);

        var sprite = container.GetSpriteOrNull(Id, SelectColor, SelectHP);

        var hpColorMatrices = new List<HpColorMatrix>();

        hpColorMatrices.AddRange(container.GetCountOfContainer(Id));
        var hpCount = -1;
        foreach (var item in hpColorMatrices) hpCount = item.hp.Count;
        SelectHPGroup.ChangeObjectValueHP(hpCount);
        MapEditor.GetInstance.GravityRenderSetting(false);
    }

    public void ChangeObject()
    {
        _SpecialTileShowRect.SetActive(false);
        SelectObjectListController.OnClickButton(null);
        RailItemController.CloseButton();
        var color = new Color(0, 0, 0, 0.2f);
        _active_bottom.color = color;

        var sprite = container.GetSpriteOrNull(Exception, SelectColor, SelectHP);

        if (Exception == ExceptionList.IDGRAVITY)
            MapEditor.GetInstance.GravityRenderSetting(true);
        else
            MapEditor.GetInstance.GravityRenderSetting(false);

        var hpColorMatrices = new List<HpColorMatrix>();

        hpColorMatrices.AddRange(container.GetCountOfContainer(Exception));
        var hpCount = -1;
        foreach (var item in hpColorMatrices) hpCount = item.hp.Count;
        SelectHPGroup.ChangeObjectValueHP(hpCount);
    }

    public void ChangeColorSprites(int HP)
    {
        var hpColorMatrices = new List<HpColorMatrix>();
        if (SelectObjectID != EID.NONE)
            hpColorMatrices.AddRange(container.GetCountOfContainer(SelectObjectID));
        else
            hpColorMatrices.AddRange(container.GetCountOfContainer(Exception));

        var lists = new List<ColorAndSprite>();
        foreach (var item in hpColorMatrices)
        {
            var colorAndSprite = new ColorAndSprite();
            colorAndSprite.color = item.color;
            if (HP > 0) colorAndSprite.sprite = item.hp[HP - 1];
            else colorAndSprite.sprite = item.hp[0];
            lists.Add(colorAndSprite);
        }

        if (Exception != ExceptionList.NONE)
            for (var i = 0; i < lists.Count; i++)
                SelectColorGroup.ChangeObjectValueColor(lists[i], lists.Count, i, Exception);
        else
            SelectColorGroup.ChangeObjectValueColor(lists);
    }

    public void LogMessage()
    {
        switch (SelectObjectID)
        {
            case EID.NONE:
                if (Exception == ExceptionList.IDGRAVITY)
                    MapEditor.GetInstance.SetGravityMode((EDirection) SelectValue);
                if (Exception == ExceptionList.IDTILE) MapEditor.GetInstance.ChangeTileKind((ETileKind) SelectValue);
                if (Exception == ExceptionList.NONE) MapEditor.GetInstance.SetObjectMode(EID.NONE, 0, 0, 0);
                break;
            case EID.RAIL:
                MapEditor.GetInstance.SetObjectMode(SelectObjectID, SelectColor, SelectHP, EtcValue);
                var property = RailItemController.GetStatus();
                if (property.railKind > 40)
                {
                    var HP = property.railKind - ((int) EID.RAIL_COVER - 1);
                    MapEditor.GetInstance.SetObjectMode(EID.RAIL_COVER, EColor.NONE, HP, 0);
                }
                else
                {
                    MapEditor.GetInstance.SetRailProperty(property);
                }

                break;
            case EID.SPECIAL_TILE:
                SetEtcValue = SelectHP - 1;
                ChangeEtcValue(SetEtcValue);
                MapEditor.GetInstance.SetObjectMode(SelectObjectID, SelectColor, SelectHP, EtcValue);
                break;
            case EID.DROP_RELIC1:
                if (SelectColor == EColor.RED)
                    MapEditor.GetInstance.SetObjectMode(SelectObjectID, 0, SelectHP, EtcValue);
                else if (SelectColor == EColor.YELLOW)
                    MapEditor.GetInstance.SetObjectMode(EID.DROP_RELIC2, 0, SelectHP, EtcValue);
                else if (SelectColor == EColor.GREEN)
                    MapEditor.GetInstance.SetObjectMode(EID.DROP_RELIC3, 0, SelectHP, EtcValue);
                break;
            default:
                MapEditor.GetInstance.SetObjectMode(SelectObjectID, SelectColor, SelectHP, EtcValue);
                break;
        }
    }

    public void ItemAdd()
    {
        if (SelectObjectID == EID.NONE)
        {
            Debug.LogError("SelectObject ID = " + SelectObjectID + " Plase Select ObjectID");
            return;
        }

        Sprite SelectObjectSprite = null;
        if (SelectObjectID == EID.NORMAL)
        {
            SelectObjectSprite = container.GetSpriteOrNull(SelectObjectID, SelectColor, SelectHP);
        }
        else
        {
            if (SelectObjectID == EID.DROP_RELIC1)
            {
                if (SelectColor == EColor.RED) SelectObjectSprite = container.GetMissionImage(EID.DROP_RELIC1);
                else if (SelectColor == EColor.YELLOW) SelectObjectSprite = container.GetMissionImage(EID.DROP_RELIC2);
                else if (SelectColor == EColor.GREEN) SelectObjectSprite = container.GetMissionImage(EID.DROP_RELIC3);
            }
            else
            {
                SelectObjectSprite = container.GetMissionImage(SelectObjectID);
            }
        }

        if (SelectObjectSprite == null)
        {
            Debug.LogError("This mission should not add SpriteContainer");
            return;
        }

        if (SelectObjectID == EID.DROP_RELIC1)
        {
            if (SelectColor == EColor.RED)
                SelectMissionObjectParent.CreateMissionList(SelectObjectSprite, EID.DROP_RELIC1);
            else if (SelectColor == EColor.YELLOW)
                SelectMissionObjectParent.CreateMissionList(SelectObjectSprite, EID.DROP_RELIC2);
            else if (SelectColor == EColor.GREEN)
                SelectMissionObjectParent.CreateMissionList(SelectObjectSprite, EID.DROP_RELIC3);
        }
        else
        {
            SelectMissionObjectParent.CreateMissionList(SelectObjectSprite, SelectObjectID, SelectColor);
        }
    }

    public void StageBlockAdd()
    {
        if (SelectObjectID == EID.NONE)
        {
            Debug.LogError("SelectObject ID = " + SelectObjectID + " Plase Select ObjectID");
            return;
        }

        if (SelectObjectID != EID.GLASS_COLOR && SelectObjectID != EID.NORMAL && SelectObjectID != EID.OAK &&
            SelectObjectID != EID.GIFTBOX &&
            SelectObjectID != EID.TIMEBOMB_ICE && SelectObjectID != EID.TIMEBOMB_LAVA &&
            SelectObjectID != EID.CHAMELEON)
            return;

        Sprite SelectObjectSprite = null;
        SelectObjectSprite = SelectObjectSprite = container.GetMissionImage(SelectObjectID);
        if (SelectObjectSprite == null)
            SelectObjectSprite = container.GetSpriteOrNull(SelectObjectID, EColor.RED, SelectHP);
        StageBlockController.CreateStageBlock(SelectObjectSprite, SelectObjectID, EColor.NONE, SelectHP, 0, EtcValue);
    }

    public void SelectIDClear()
    {
        MapEditor.GetInstance.SetObjectMode(EID.NONE, 0, 0, 0);
    }

    public void ChangeEtcValue(Text text)
    {
        var intvalue = text.text;
        int.TryParse(intvalue, out EtcValue);
        if (SelectObjectID == EID.SPECIAL_TILE) SpecialTileItemController.ChangeItem(EtcValue);
        LogMessage();
    }

    public void ChangeEtcValue(int intValue)
    {
        if (SelectObjectID == EID.SPECIAL_TILE) SpecialTileItemController.ChangeItem(intValue);
    }

    public void AutoModeCheck(Toggle toggle)
    {
        if (toggle.isOn)
        {
            EditorAutoModeControll._isAutoMode = true;
            if (LogicManager.GetInstance.GetStateName() == "WAITUSERINPUT")
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
        }
        else
        {
            EditorAutoModeControll._isAutoMode = false;
            MapEditor.GetInstance.StopAllCoroutines();
            if (EditorAutoModeControll._isChallengeInfo.Count == 0) return;
            EditorAutoLog.GetInstance.TestJson();
            EditorAutoModeControll._isChallengeInfo.Clear();
        }
    }

    public void SetMission(EID eID, EColor eColor, int MissionCount)
    {
        Sprite SelectObjectSprite = null;
        if (eID == EID.NORMAL)
            SelectObjectSprite = container.GetSpriteOrNull(eID, eColor, 1);
        else
            SelectObjectSprite = container.GetMissionImage(eID);

        if (SelectObjectSprite == null)
        {
            Debug.LogError("This mission should not add SpriteContainer");
            return;
        }

        SelectMissionObjectParent.CreateMissionList(SelectObjectSprite, eID, eColor, MissionCount);
    }

    public void SetStageBlock(EID eID, EColor eColor, int HP = 1, int percent = 0, int etc = 0)
    {
        var SelectObjectSprite = container.GetSpriteOrNull(eID, eColor, HP);
        if (SelectObjectSprite == null) return;
        StageBlockController.CreateStageBlock(SelectObjectSprite, eID, eColor, HP, percent, etc);
    }

    public void SetMoveCount(int Num)
    {
        StageFoundationController.SetMoveCount(Num);
    }

    public void SetBlockCount(int Num)
    {
        StageFoundationController.SetBlockCount(Num);
    }

    public void SetMultiMapSetting(Vector2 VMatrix, int Order)
    {
        StageMultiMapController.SetMultiMapSetting(VMatrix, Order);
    }

    public void SetMultiMapSetting(int intMatrix, int Order)
    {
        StageMultiMapController.SetMultiMapSetting(intMatrix, Order);
    }

    public int GetMoveCount()
    {
        return StageFoundationController.GetMoveCount;
    }

    public int GetBlockCount()
    {
        return StageFoundationController.GetBlockCount;
    }

    public List<int> GetBlocksPersent()
    {
        var returnValue = new List<int>();

        return returnValue;
    }

    public List<MissionInfo> GetMissions()
    {
        var missionInfos = new List<MissionInfo>();
        missionInfos.AddRange(SelectMissionObjectParent.GetMissions());

        foreach (var item in missionInfos)
        {
            // Debug.Log(item.eID);
            if (item.eID == EID.NORMAL)
            {
            }
        }

        return missionInfos;
    }

    public List<int> GetMultiMap()
    {
        var returnValue = new List<int>();
        returnValue.AddRange(StageMultiMapController.GetMultiMapList());

        return returnValue;
    }

    public void GetMoveCount_Debug()
    {
        Debug.Log(StageFoundationController.GetMoveCount + " MoveCount");
    }

    public void GetBlockCount_Debug()
    {
        Debug.Log(StageFoundationController.GetBlockCount + " BlockCount");
    }

    public List<StageBlockInfo> GetStageBlock_Debug()
    {
        var stageBlockInfos = new List<StageBlockInfo>();
        stageBlockInfos.AddRange(StageBlockController.GetStageBlocks());

        var returnValue = new List<StageBlockInfo>();
        foreach (var item in stageBlockInfos)
            if (item.blockInfo.ID == EID.NORMAL)
            {
                var persent = item._isPersent / GetBlockCount();
                for (var i = 0; i < GetBlockCount(); i++)
                {
                    var stageBlockInfo = new StageBlockInfo();
                    var blockInfo = new BlockInfo(EID.NORMAL, (EColor) i, item.blockInfo.HP, item.blockInfo.ETC);
                    stageBlockInfo.blockInfo = blockInfo;
                    stageBlockInfo._isPersent = persent;
                    returnValue.Add(stageBlockInfo);
                }
            }
            else
            {
                returnValue.Add(item);
            }

        foreach (var item in returnValue)
        {
            Debug.Log((EID) item.blockInfo.ID);
            // Debug.Log(item.blockInfo.color);
            // Debug.Log(item.blockInfo.HP);
            // Debug.Log(item.blockInfo.ETC);
            // Debug.Log(item._isPersent);
        }

        return returnValue;
    }

    private IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(1);
        MapEditor.GetInstance.ClickGameTest();
    }

    public Sprite GetSptieContainer(BlockInfo blockInfo)
    {
        var returnValue = container.GetSpriteOrNull(blockInfo.ID, blockInfo.color, blockInfo.HP);
        if (returnValue == null) container.GetSpriteOrNull(blockInfo.ID, EColor.RED, blockInfo.HP);
        return returnValue;
    }

    public void AutoFullSet()
    {
        if (EditorAutoModeControll._isAutoFull)
        {
            EditorAutoModeControll._isAutoFull = false;
            Debug.Log("false");
        }
        else
        {
            EditorAutoModeControll._isAutoFull = true;
            Debug.Log("true");
        }
    }

    public void CencelButtonActive(bool Value)
    {
        _cencelButton.SetActive(Value);
    }

    public void MoveScene(Text text)
    {
        var intValue = 0;
        int.TryParse(text.text, out intValue);
        if (intValue == 0) return;
        StageManager.StageNumber = intValue;
        MapEditor.GetInstance.ReturnStage();
    }

    public void isTest(bool Value)
    {
        if (Value)
        {
            _editorButonObject.SetActive(true);
            _testStatusObject.SetActive(true);
            _testButtonObject.SetActive(false);
            RailItemController.CloseButton();
            ConnectionController.CloseButton();
            TabletController.CloseUI();
        }
        else
        {
            _editorButonObject.SetActive(false);
            _testStatusObject.SetActive(false);
            _testButtonObject.SetActive(true);
        }
    }
}