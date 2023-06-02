using UnityEngine;

public class CongraturationAnimController : MonoBehaviour
{
    [SerializeField] private SpineCharacterController monkeyScript;

    public void ActiveMonkeyAnim()
    {
        if (monkeyScript == null) return;
        monkeyScript.ShowAnim();
    }
}