using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DirectionPosition
{
    public EDirection direction;
    public Vector2 position;
    public Vector3 rotation;
}

public class EditorRail : MonoBehaviour
{
    [SerializeField] private SpriteRenderer nextDirection;
    [SerializeField] private SpriteRenderer preDirection;
    [SerializeField] private SpriteRenderer mainSprite;

    public List<DirectionPosition> nextPosition = new List<DirectionPosition>();
    public List<DirectionPosition> prePosition = new List<DirectionPosition>();

    public float alpha = 0.5f;

    public void Setting(EDirection nextD, EDirection preD)
    {
        foreach (var item in nextPosition)
            if (item.direction == nextD)
            {
                nextDirection.gameObject.transform.localPosition = item.position;
                nextDirection.gameObject.transform.rotation = Quaternion.Euler(item.rotation);
            }

        foreach (var item in prePosition)
            if (item.direction == preD)
            {
                preDirection.gameObject.transform.localPosition = item.position;
                preDirection.gameObject.transform.rotation = Quaternion.Euler(item.rotation);
            }
    }

    public void SetMainSprite(Sprite sprite)
    {
        Debug.Log(sprite.name);
        mainSprite.sprite = sprite;
    }

    public void SetOrderSprite(int order)
    {
        var color = new Color();
        switch (order)
        {
            case 1:
                mainSprite.color = Color.white;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 2:
                mainSprite.color = Color.yellow;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;

            case 3:
                mainSprite.color = Color.green;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 4:
                mainSprite.color = Color.blue;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 5:
                mainSprite.color = Color.cyan;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 6:
                mainSprite.color = Color.red;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 7:
                mainSprite.color = Color.gray;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            case 8:
                mainSprite.color = Color.black;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
            default:
                mainSprite.color = Color.white;
                color = mainSprite.color;
                color.a = alpha;
                mainSprite.color = color;
                break;
        }
    }
}