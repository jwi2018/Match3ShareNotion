using UnityEngine;

public class PrivacyPolicyNewPopup : MonoBehaviour
{
    public GameObject KOR;
    public GameObject EU;

    private void Start()
    {
        if (PlayerData.GetInstance.NumLanguage == 9)
        {
            EU.SetActive(false);
            KOR.SetActive(true);
        }
        else
        {
            KOR.SetActive(false);
            EU.SetActive(true);
        }
    }

    public void OnClickOK()
    {
        Destroy(gameObject);
    }
}