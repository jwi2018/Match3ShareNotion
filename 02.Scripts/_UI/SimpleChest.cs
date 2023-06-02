using UnityEngine;
using UnityEngine.UI;

public class SimpleChest : MonoBehaviour
{
    [SerializeField] private GameObject _chest;

    [SerializeField] private Image rewardImage;

    [SerializeField] private Text rewardText;

    public void SetActiveFalse()
    {
        if (_chest != null) _chest.SetActive(false);
    }

    public void StartOpen(Sprite sprite, int intValue)
    {
        rewardImage.sprite = sprite;
        rewardText.text = "+ " + intValue;
        GetComponent<Animator>().SetTrigger("Open2");
    }
}