using System;
using System.IO;
using NiceIO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Il2Cpp;
using UnityEditorInternal;
using UnityEngine;

namespace HuaTuo.Editor
{
    public class HuaTuo_BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IIl2CppProcessor
    {
        public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {
            try
            {
                var managedPath = "Temp/StagingArea/Data/Managed/";
                if (data.target == BuildTarget.Android) managedPath = "Temp/StagingArea/assets/bin/Data/Managed";

                if (data.target == BuildTarget.Android)
                {
                    File.Copy(Path.Combine(managedPath, "mscorlib.dll"), "Temp/StagingArea/assets/mscorlib.dll");
                }
                else
                {
                    var paths = new NPath("Temp/StagingArea").Directories("StreamingAssets", true);
                    if (paths.Length > 0)
                        File.Copy(Path.Combine(managedPath, "mscorlib.dll"),
                            paths[0].Combine("mscorlib.dll").ToString());
                }
            }
            catch (Exception e)
            {
                throw new BuildFailedException(e);
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
        }

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            HuaTuoEditorHelper.ClearBuildCache();
            Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", "");
            try
            {
                Debug.Log("开始拷贝il2cpp");
                var il2cppPath =
                    new NPath(Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "il2cpp/")));
                il2cppPath.CopyFiles(new NPath("il2cpp_huatuo/"), true,
                    path => !path.IsSameAsOrChildOf(il2cppPath.Combine("libil2cpp")));

                il2cppPath.Parent.Combine("MonoBleedingEdge").CopyFiles(new NPath("MonoBleedingEdge"), true);
                Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH",
                    Path.Combine(Environment.CurrentDirectory, "il2cpp_huatuo"));

                PlayerSettings.SetAdditionalIl2CppArgs("");

                var huatuoPath = new NPath("il2cpp_huatuo/libil2cpp/huatuo");

                if (!huatuoPath.Exists() || !huatuoPath.IsSymbolicLink)
                {
                    huatuoPath.DeleteIfExists();
                    Debug.Log("拷贝huatuo目录");
                    new NPath("huatuo/huatuo").Copy(huatuoPath.Parent);
                }

                var platform = report.summary.platform;
                switch (platform)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneLinux64:
                        HuaTuoEditorHelper.GenMethodBridge_X86(Path.Combine(IL2CPPUtils.GetIl2CppFolder(),
                            "libil2cpp/huatuo/interpreter/MethodBridge_x64.cpp"));
                        PlayerSettings.SetAdditionalIl2CppArgs("--generate-cmake=il2cpp");
                        break;
                    case BuildTarget.Android:
                    case BuildTarget.iOS:
                        HuaTuoEditorHelper.GenMethodBridge_Arm64(Path.Combine(IL2CPPUtils.GetIl2CppFolder(),
                            "libil2cpp/huatuo/interpreter/MethodBridge_arm64.cpp"));
                        break;
                }
            }
            catch (Exception e)
            {
                throw new BuildFailedException(e);
            }
        }

        private static bool IsIL2CPPEnabled()
        {
            return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) ==
                   ScriptingImplementation.IL2CPP;
        }
    }
}