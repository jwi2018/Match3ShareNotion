using UnityEngine;
using UnityEngine.UI;

public class TutorialToggleStatus : MonoBehaviour
{
    [SerializeField] private TutorialPopup _tutorialMaster;

    [SerializeField] private GameObject _activeObject;

    [SerializeField] private Toggle _toggle;

    public void OnClickToggle(string TermName)
    {
        if (_toggle.isOn)
        {
            _activeObject.SetActive(true);
            _tutorialMaster.ChangeText(TermName);
        }
        else
        {
            _activeObject.SetActive(false);
        }
    }
}