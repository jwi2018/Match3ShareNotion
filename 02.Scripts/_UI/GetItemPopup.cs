using UnityEngine;
using UnityEngine.UI;

public class GetItemPopup : MonoBehaviour
{
    public Text suceess;
    public Text fail;

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    public void SetPopup(bool isOk)
    {
        if (isOk)
        {
            suceess.gameObject.SetActive(true);
            fail.gameObject.SetActive(false);
        }
        else
        {
            suceess.gameObject.SetActive(false);
            fail.gameObject.SetActive(true);
        }
    }
}