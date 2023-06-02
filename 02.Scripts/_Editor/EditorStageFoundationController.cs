using UnityEngine;
using UnityEngine.UI;

public class EditorStageFoundationController : MonoBehaviour
{
    [SerializeField] private EditorToggleManager ToggleManager;

    [SerializeField] private InputField Stage_MoveCount;

    [SerializeField] private InputField Stage_BlockCount;

    private int BlockCount;

    private int MoveCount;

    public int GetMoveCount => MoveCount;
    public int GetBlockCount => BlockCount;

    public void ChangeText(Text text)
    {
        var Count = text.text;
        if (Count.Length == 0) return;

        if (text == Stage_MoveCount.textComponent)
            int.TryParse(Count, out MoveCount);
        else if (text == Stage_BlockCount.textComponent)
            int.TryParse(Count, out BlockCount);
        else return;
    }

    public void SetMoveCount(int Num)
    {
        Stage_MoveCount.text = Num.ToString();
        MoveCount = Num;
    }

    public void SetBlockCount(int Num)
    {
        Stage_BlockCount.text = Num.ToString();
        BlockCount = Num;
    }
}