using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorStageScoreController : MonoBehaviour
{
    [SerializeField] private InputField StageScore_0;

    [SerializeField] private InputField StageScore_1;

    [SerializeField] private InputField StageScore_2;

    private int Score_0;
    private int Score_1;
    private int Score_2;

    public void SetScore(int Member_Num, int Score)
    {
        switch (Member_Num)
        {
            case 0:
                StageScore_0.text = Score.ToString();
                Score_0 = Score;
                break;
            case 1:
                StageScore_1.text = Score.ToString();
                Score_1 = Score;
                break;
            case 2:
                StageScore_2.text = Score.ToString();
                Score_2 = Score;
                break;
        }
    }

    public void ChangeScore(Text text)
    {
        var Scoreint = text.text;
        if (Scoreint.Length == 0) return;
        if (text == StageScore_0.textComponent)
            int.TryParse(Scoreint, out Score_0);
        else if (text == StageScore_1.textComponent)
            int.TryParse(Scoreint, out Score_1);
        else if (text == StageScore_2.textComponent)
            int.TryParse(Scoreint, out Score_2);
        else return;
    }

    public List<int> GetScores()
    {
        var returnValue = new List<int>();
        returnValue.Add(Score_0);
        returnValue.Add(Score_1);
        returnValue.Add(Score_2);
        return returnValue;
    }
}