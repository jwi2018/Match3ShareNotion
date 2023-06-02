using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tiinoo.CutAndPaste
{
	public class CPMenu
	{
		[MenuItem("Assets/Cut Assets", true)]
		static bool ValidateCutAssets()
		{
			return CPAssetsMover.ValidateCutAssets();
		}

		[MenuItem("Assets/Cut Assets", false, 60)]
		static void CutAssets()
		{
			CPAssetsMover.CutAssets();
		}
		
		[MenuItem("Assets/Paste Assets", true)]
		static bool ValidatePasteAssets()
		{
			return CPAssetsMover.ValidatePasteAssets();
		}
		
		[MenuItem("Assets/Paste Assets", false, 61)]
		static void PasteAssets()
		{
			CPAssetsMover.PasteAssets();
		}
	}
}


