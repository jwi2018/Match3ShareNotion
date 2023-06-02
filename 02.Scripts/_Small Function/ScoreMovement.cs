using UnityEngine;
using UnityEngine.UI;

public class ScoreMovement : MonoBehaviour
{
    [SerializeField] private float movingTime = 0.5f;

    [SerializeField] private EDirection movingDirection = EDirection.UP;

    [SerializeField] private float _preSpeed = 1f;

    [SerializeField] private float heightValue = 10f;

    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Text _score;

    private bool _isMoving;

    private float _preTime;

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _score = gameObject.GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            _preTime += Time.deltaTime;
            _rectTransform.Translate(new Vector3(0, _preSpeed * Time.deltaTime, 0));

            var alpha = _score.color.a;

            var preAlpha = Mathf.Lerp(alpha, 0, Time.deltaTime * 3);

            _score.color = new Color(_score.color.r, _score.color.g, _score.color.b, preAlpha);

            if (_preTime >= movingTime)
            {
                _isMoving = false;
                DynamicObjectPool.GetInstance.PoolObject(gameObject, false);
                //gameObject.SetActive(false);
            }
        }
    }

    public void Setting(Vector2Int matrix, EDirection direction, int score, EColor color = EColor.NONE)
    {
        if (_rectTransform == null) _rectTransform = gameObject.GetComponent<RectTransform>();
        if (_score == null) _score = gameObject.GetComponent<Text>();
        //TileManager.GetInstance.SetRectPositionUseMatrix(_rectTransform, matrix);
        _rectTransform.position = TileManager.GetInstance.GetTilePosition(matrix);
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x,
            _rectTransform.anchoredPosition.y + heightValue);
        _preTime = 0f;
        _isMoving = true;
        _score.text = score.ToString();
        _score.color = new Color(_score.color.r, _score.color.g, _score.color.b, 1);
        switch (color)
        {
            case EColor.BLUE:
                _score.color = new Color32(96, 204, 231, 255);
                break;
            case EColor.GREEN:
                _score.color = new Color32(234, 254, 143, 255);
                break;
            case EColor.RED:
                _score.color = new Color32(241, 162, 159, 255);
                break;
            case EColor.PURPLE:
                _score.color = new Color32(189, 70, 246, 255);
                break;
            case EColor.YELLOW:
                _score.color = new Color32(255, 254, 140, 255);
                break;
            case EColor.ORANGE:
                _score.color = new Color32(232, 109, 46, 255);
                break;
            default:
                _score.color = new Color32(254, 238, 192, 255);
                break;
        }
    }
}