using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    [SerializeField] private Image gravityImage;
    [SerializeField] private Text stageText;
    [SerializeField] private GameObject greenRedTile;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;
    [SerializeField] private GameObject failPopup;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject savePopup;
    [SerializeField] private Text saveText;

    private RectTransform gravityRectTransform;

    private readonly List<Image> greenRedSprites = new List<Image>();

    public void Init()
    {
        if (gravityImage != null) gravityRectTransform = gravityImage.GetComponent<RectTransform>();
    }

    public void SetGravityRotation(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.RIGHT:
                gravityRectTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case EDirection.LEFT:
                gravityRectTransform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case EDirection.UP:
                gravityRectTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case EDirection.DOWN:
                gravityRectTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    public void SetStageNumber(int stageNum)
    {
        if (stageText != null) stageText.text = "STAGE: " + stageNum;
    }

    public void ShowGreenRedTile(List<EditImageTile> tiles, bool isGreen)
    {
        ClearGreenRedTile();

        foreach (var t in tiles)
        {
            var colorTile = Instantiate(greenRedTile);
            if (colorTile == null) return;

            var tileImage = colorTile.GetComponent<Image>();
            if (tileImage == null) return;

            if (isGreen)
                tileImage.color = greenColor;
            else
                tileImage.color = redColor;

            greenRedSprites.Add(tileImage);

            tileImage.rectTransform.SetParent(t.transform);
            tileImage.rectTransform.localPosition = new Vector3(0, 0, 0);
            tileImage.rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ClearGreenRedTile()
    {
        var tempList = new List<Image>();

        foreach (var item in greenRedSprites)
            Destroy(
                item.gameObject);
        greenRedSprites.Clear();
    }

    public void ShowVictoryPopup()
    {
        if (victoryPopup != null) victoryPopup.SetActive(true);
    }

    public void ShowFailPopUp()
    {
        if (failPopup != null) failPopup.SetActive(true);
    }

    public void SetSavePopup(bool value)
    {
        if (savePopup != null) savePopup.SetActive(value);
    }

    public void SetSaveText(string text)
    {
        if (saveText != null) saveText.text = text;
    }

    public void GameEnd()
    {
        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (failPopup != null) failPopup.SetActive(false);
    }
}