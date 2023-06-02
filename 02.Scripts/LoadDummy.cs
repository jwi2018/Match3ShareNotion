using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDummy : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Dummy());
    }

    private IEnumerator Dummy()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("LoadScene");
    }
}