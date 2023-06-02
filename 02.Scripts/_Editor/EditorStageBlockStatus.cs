using UnityEngine;
using UnityEngine.UI;

public class StageBlockInfo
{
    public int _isPersent;
    public BlockInfo blockInfo;
}

/*public class StageBlockInfo
{
    public EID _ID;
    public EColor _Color;
    public int _HP;
    public int _ETC;
    public int _isPersent;
}*/
public class EditorStageBlockStatus : MonoBehaviour
{
    [SerializeField] private Image _stageBlockImage;

    [SerializeField] private InputField _persentInput;

    [SerializeField] private EditorStageBlockController StageBlockController;

    [SerializeField] private GameObject DestroyButton;

    private readonly StageBlockInfo _stageBlockInfo = new StageBlockInfo();

    public InputField GetInputField => _persentInput;

    public void CreateStageBlock(Sprite sprite, EID iD, EColor color, int HP, int persent, int etc)
    {
        var blockInfo = new BlockInfo(iD, color, HP, etc);
        _stageBlockImage.sprite = sprite;
        _stageBlockInfo.blockInfo = blockInfo;
        _persentInput.text = persent.ToString();
        _stageBlockInfo._isPersent = persent;
        if (iD == EID.NORMAL) DestroyButton.SetActive(false);
    }

    public void BlockPersentChange(int persent)
    {
        var strings = persent.ToString();
        _persentInput.text = strings;
        _stageBlockInfo._isPersent = persent;
    }

    public void BlockPersentChange(Text text)
    {
        var strings = text.text;
        int.TryParse(strings, out _stageBlockInfo._isPersent);
        StageBlockController.CheckPersentValue();
    }

    public StageBlockInfo GetStageBlockInfo()
    {
        return _stageBlockInfo;
    }

    public void DestroyStageBlock()
    {
        BlockPersentChange(0);
        StageBlockController.CheckPersentValue();
        Destroy(gameObject);
    }
}