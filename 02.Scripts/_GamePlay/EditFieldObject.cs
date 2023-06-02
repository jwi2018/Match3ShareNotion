using UnityEngine;

public class EditFieldObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer objectRender;
    [SerializeField] private Transform objTransform;

    public void SetSprite(Sprite sprite)
    {
        //if(objectImage != null)
        {
            //objectImage.sprite = sprite;
        }
        if (objectRender != null) objectRender.sprite = sprite;
    }

    public void Setting(RectTransform tile, EDepth depth)
    {
        //if (rectTransform != null)
        {
            //rectTransform.SetParent(tile);
            //rectTransform.localPosition = new Vector3(0, 0, 0);
            //rectTransform.localScale = new Vector3(1, 1, 1);
        }
        if (objTransform != null)
        {
            objTransform.SetParent(tile);
            if (objectRender.sprite != null)
            {
                // Debug.Log($"Rect{objectRender.sprite.rect}, Pivot:{objectRender.sprite.pivot}");
                var pivotdiff = objectRender.sprite.pivot.y - (objectRender.sprite.rect.height / 2);
                
                objTransform.localPosition = new Vector3(0, pivotdiff, 0);
            }
            else
            {
                objTransform.localPosition = new Vector3(0, 0, 0);
            }
            
            objTransform.localScale = new Vector3(100, 100, 1);
        }

        objectRender.sortingOrder = (int) depth;
    }

    public void SetPosition(Vector2 position)
    {
        //rectTransform.localPosition = position;
        objTransform.localPosition = position;
    }

    public void SetSize(Vector2 size)
    {
        //rectTransform.sizeDelta = size;
        //objTransform.localScale = size;
    }
}