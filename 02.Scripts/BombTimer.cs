using System.Collections.Generic;
using UnityEngine;

public class BombTimer : MonoBehaviour
{
    public int Turn;
    public GameObject BombObj;
    public TextMesh TurnText;
    public TextOutline Outline;

    public List<GameObject> outlineObj = new List<GameObject>();

    public void Init(GameObject obj)
    {
        BombObj = obj;
        gameObject.transform.SetParent(BombObj.transform);
        //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -15);
    }


    public void SetTurn(int turn)
    {
        Turn = turn;
        TurnText.text = Turn.ToString();

        if (BombObj != null)
        {
            var block = BombObj.GetComponentInParent<GameBlock>();
            if (block != null)
                if (block.Tile != null)
                {
                    if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                    {
                        if (outlineObj.Count == 0)
                        {
                            var outlines = GetComponentsInChildren<TextMesh>();
                            if (outlines != null)
                                for (var i = 0; i < outlines.Length; i++)
                                {
                                    var test = outlines[i].gameObject.GetComponent<BombTimer>();
                                    if (test == null)
                                    {
                                        outlineObj.Add(outlines[i].gameObject);
                                        outlines[i].gameObject.SetActive(true);
                                    }
                                }
                        }
                        else
                        {
                            foreach (var item in outlineObj) item.SetActive(true);
                        }
                    }
                    else
                    {
                        if (outlineObj.Count == 0)
                        {
                            var outlines = GetComponentsInChildren<TextMesh>();
                            if (outlines != null)
                                for (var i = 0; i < outlines.Length; i++)
                                {
                                    var test = outlines[i].gameObject.GetComponent<BombTimer>();
                                    if (test == null)
                                    {
                                        outlineObj.Add(outlines[i].gameObject);
                                        outlines[i].gameObject.SetActive(false);
                                    }
                                }
                        }
                        else
                        {
                            foreach (var item in outlineObj) item.SetActive(false);
                        }
                    }
                }
        }
    }

    public void Clear()
    {
        Outline.Dead();
    }
}