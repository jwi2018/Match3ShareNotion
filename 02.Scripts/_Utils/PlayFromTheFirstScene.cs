using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class PlayFromTheFirstScene
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoadRuntimeMethod()
	{
		Action DeactivateAllObject = () =>
		{
			GameObject[] objects = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject go in objects)
			{
				if (go.transform.parent == null)
				{
					go.SetActive(false);
				}
			}
		};

		Scene scene = SceneManager.GetActiveScene();
		
		if (scene.name == "GameScene" || scene.name == "MainScene")
		{
			DeactivateAllObject();
			SceneManager.LoadScene("LoadScene");
		}
	}
}
