using UnityEngine;
using UnityEngine.EventSystems;

public class StageScrollBar : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private GameObject _stageViewPort;

    [SerializeField] private GameObject ScrollButton;

    private RectTransform Rect;
    private float ScrollActiveDelay;

    public float SetScrollActiveDelay
    {
        set => ScrollActiveDelay = value;
    }

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ScrollActiveDelay = 3.9f;
    }

    private void Update()
    {
        ScrollActiveDelay += Time.deltaTime;

        if (ScrollActiveDelay > 4.0f)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.enterEventCamera != null) OnClickScrollBar(data.position.y, data.enterEventCamera.scaledPixelHeight);
    }

    public void OnPointerDown(PointerEventData data)
    {
        OnClickScrollBar(data.position.y, data.enterEventCamera.scaledPixelHeight);
    }

    private void OnClickScrollBar(float YPosition, float YPixel)
    {
        ScrollActiveDelay = 0.0f;


        var ScrollButtonRectSizeY = ScrollButton.GetComponent<RectTransform>().sizeDelta.y;

        var RealPositionY = Rect.parent.GetComponent<RectTransform>().sizeDelta.y * (YPosition / YPixel);

        if (RealPositionY - ScrollButtonRectSizeY * 0.5f < Rect.offsetMin.y)
            ScrollButton.transform.localPosition = new Vector3(ScrollButton.transform.localPosition.x, 0, 0);
        else if (RealPositionY - (Rect.offsetMin.y - ScrollButtonRectSizeY * 0.5f) > Rect.rect.height)
            ScrollButton.transform.localPosition = new Vector3(ScrollButton.transform.localPosition.x,
                Rect.rect.height - ScrollButtonRectSizeY, 0);
        else
            ScrollButton.transform.localPosition = new Vector3(ScrollButton.transform.localPosition.x,
                RealPositionY - Rect.offsetMin.y - ScrollButtonRectSizeY * 0.5f, 0);
        var ScrollValue = 1 - ScrollButton.transform.localPosition.y / (Rect.rect.height - ScrollButtonRectSizeY);

        _stageViewPort.GetComponent<DynamicScrollList>().ScrollBarValueChange(ScrollValue);
    }

    public void ScrollRectValueChange(float ScrollRectValue)
    {
        if (ScrollRectValue < 0)
            ScrollRectValue = 0;
        else if (ScrollRectValue > 1) ScrollRectValue = 1;
        ScrollButton.transform.localPosition = new Vector3(ScrollButton.transform.localPosition.x,
            (1 - ScrollRectValue) * (Rect.rect.height - ScrollButton.GetComponent<RectTransform>().sizeDelta.y), 0);
    }
}