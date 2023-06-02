using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Build.Reporting;

public static class AutoBuilder
{
    [MenuItem("AutoBuilder/Set Keystore")]
    private static void SetKeystore()
    {
        var basePath = Application.dataPath.Replace("/Assets", "");
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = Path.Combine(basePath, ".keystore");
        PlayerSettings.Android.keyaliasName = "release";
        PlayerSettings.Android.keyaliasPass = "";
        PlayerSettings.Android.keystorePass = "";
    }

    [MenuItem("AutoBuilder/Android/Release")]
    private static void PerformAndroidBuild()
    {
        Build(BuildTarget.Android, false);
    }

    private static void Build(BuildTarget buildTarget, bool development)
    {
        Debug.Log("Build Start: " + buildTarget);

        string targetPath = string.Empty;
        BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
        bool buildAppBundle = Environment.GetEnvironmentVariable("ANDROID_BUILD_APP_BUNDLE") == "true";
        switch (buildTarget)
        {
            case BuildTarget.Android:
                targetPath = development ? PlayerSettings.productName + "_Dev.apk" : PlayerSettings.productName + ".apk";
                if (buildAppBundle)
                {
                    targetPath = targetPath.Replace(".apk", ".aab");
                }
                buildTargetGroup = BuildTargetGroup.Android;
                break;
            case BuildTarget.iOS:
                targetPath = development ? PlayerSettings.productName + "Debug" : PlayerSettings.productName + "Release";
                buildTargetGroup = BuildTargetGroup.iOS;
                break;
        }

        var productName = PlayerSettings.productName;
        if (development)
        {
            productName += " (development)";
        }
        string[] Scenes = { "Assets\\01.Scenes\\LoadSceneDummy", "Assets\\01.Scenes\\LoadScene", "Assets\\01.Scenes\\GameScene", "Assets\\01.Scenes\\MainScene" };

        if (PerformBuild(Scenes, targetPath, buildTarget, development))
        {
            EditorUtility.DisplayDialog("Build Completed", "Build Success: " + buildTarget, "OK");
        }
        else
        {
            throw new Exception("BuildError!");
        }
    }

    private static bool PerformBuild(string[] scenes, string targetPath, BuildTarget buildTarget, bool development)
    {
        EditorUserBuildSettings.development = development;

        var ret = BuildPipeline.BuildPlayer(scenes, targetPath, buildTarget,
            development ? BuildOptions.Development : BuildOptions.None);
        var summary = ret.summary;

        if (summary.result == BuildResult.Failed)
        {
            EditorUtility.DisplayDialog("Build Error", "Build Error: " + ret, "OK");
            return false;
        }

        return true;
    }
}
