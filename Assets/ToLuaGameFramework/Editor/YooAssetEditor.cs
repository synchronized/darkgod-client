using UnityEngine;
using UnityEditor;

public static class YooAssetEditor
{


    [MenuItem("ToLuaGameFramework/YooAsset Build Windows AssetBundle", false, 100)]
    public static void BuildWindowsResource()
    {
        var luaPackageName = "LuaPackage";
        YooAssetExtend.RemoveLuaPackage(luaPackageName);
        YooAssetExtend.CreateLuaPackage(luaPackageName);
        var builder = new ScriptableBuildPipelineBuilder(luaPackageName);
        var buildResult = builder.BuildInternal(BuildTarget.StandaloneWindows64, builder.GetDefaultPackageVersion());
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
    }

}

