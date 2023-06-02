using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tiinoo.CutAndPaste
{
	internal class CPAssetsMover 
	{
		private static List<string> m_assetPaths = new List<string>();

		public static bool ValidateCutAssets()
		{
			int selectedCount = GetSelectedAssetsCount();
			bool isValid = (selectedCount > 0) ? true : false;
			return isValid;
		}

		public static void CutAssets()
		{
			m_assetPaths.Clear();

			string[] guids = Selection.assetGUIDs;
			foreach (string guid in guids)
			{
				if (string.IsNullOrEmpty(guid))
				{
					continue;
				}
				
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(assetPath))
				{
					continue;
				}
				
				m_assetPaths.Add(assetPath);
				Debug.Log("Cut: " + assetPath);
			}
		}

		public static bool ValidatePasteAssets()
		{
			if (m_assetPaths.Count <= 0)
			{
				return false;
			}
			
			int selectedCount = GetSelectedAssetsCount();
			if (selectedCount != 1)
			{
				return false;
			}
			
			if (m_assetPaths.Count == 1)
			{
				string assetPath = m_assetPaths[0];
				string dstPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
				if (dstPath.Equals(assetPath))
				{
					return false;
				}
			}
			
			return true;
		}

		public static void PasteAssets()
		{
			string guid = Selection.assetGUIDs[0];
			string dstFolderPath = GetFolderPath(guid);
			
			foreach (string assetPath in m_assetPaths)
			{
				string fileName = Path.GetFileName(assetPath);
				string dstPath = dstFolderPath + "/" + fileName;
				if (assetPath.Equals(dstPath))
				{
					Debug.Log("Paste Succeed: " + assetPath + " -> " + dstPath);
					continue;
				}
				
				string result = AssetDatabase.MoveAsset(assetPath, dstPath);
				if (string.IsNullOrEmpty(result))
				{
					Debug.Log("Paste Succeed: " + assetPath + " -> " + dstPath);
				}
				else
				{
					Debug.LogError("Paste Failed: " + result);
				}
			}
			m_assetPaths.Clear();
		}
		
		private static string GetFolderPath(string guid)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			string folderPath = null;
			if (!Directory.Exists(assetPath))
			{
				folderPath = Path.GetDirectoryName(assetPath);
			}
			else
			{
				folderPath = assetPath;
			}
			return folderPath;
		}

		private static int GetSelectedAssetsCount()
		{
			string[] guids = Selection.assetGUIDs;
			if (guids == null)
			{
				return 0;
			}
			return guids.Length;
		}
	}
}
