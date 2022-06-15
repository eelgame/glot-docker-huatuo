using System;
using System.IO;
using System.Linq;
using HuaTuo.Generators;
using NiceIO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HuaTuo.Editor
{
    /// <summary>
    ///     这里仅仅是一个流程展示
    ///     简单说明如果你想将huatuo的dll做成自动化的简单实现
    /// </summary>
    public class HuaTuoEditorHelper
    {
        public static string DllBuildOutputDir => Path.GetFullPath($"{Application.streamingAssetsPath}");

        [MenuItem("HuaTuo/BuildForGlot")]
        private static void BuildForGlot()
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.locationPathName = "Glot-Windows/huatuo.exe";

#if UNITY_EDITOR_LINUX
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.locationPathName = "Glot-Linux/huatuo";
            
            new NPath(EditorApplication.applicationContentsPath).Combine("il2cpp").CopyFiles("il2cpp_env/il2cpp", true);
            new NPath(EditorApplication.applicationContentsPath).Combine("MonoBleedingEdge").CopyFiles("il2cpp_env/MonoBleedingEdge", true);
            new NPath(EditorApplication.applicationContentsPath).Combine("PlaybackEngines/LinuxStandaloneSupport/Variations/linux64_headless_nondevelopment_il2cpp/baselib.a").Copy("il2cpp_env/");
#endif

            
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("构建成功");
            }
            else
            {
                Debug.LogError("构建失败");
                if (Application.isBatchMode) Application.Quit(1);
            }
        }

        [MenuItem("HuaTuo/ClearBuildCache")]
        public static void ClearBuildCache()
        {
            foreach (var dir in new NPath("Library").Directories("il2cpp*")) dir.Delete();
        }

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            // PlayerSettings.SetAdditionalIl2CppArgs("--generate-cmake=il2cpp");
            PlayerSettings.SetAdditionalIl2CppArgs("");
        }

        [MenuItem("HuaTuo/Generate/MethodBridge_X64")]
        public static void MethodBridge_X86()
        {
            //var target = EditorUserBuildSettings.activeBuildTarget;
            var outputFile = "huatuo/huatuo/interpreter/MethodBridge_x64.cpp";
            GenMethodBridge_X86(outputFile);
        }

        public static void GenMethodBridge_X86(string outputFile)
        {
            var g = new MethodBridgeGenerator(new MethodBridgeGeneratorOptions
            {
                CallConvention = CallConventionType.X64,
                Assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList(),
                OutputFile = outputFile
            });

            g.PrepareMethods();
            g.Generate();
            Debug.LogFormat("== output:{0} ==", outputFile);
        }

        [MenuItem("HuaTuo/Generate/MethodBridge_Arm64")]
        public static void MethodBridge_Arm64()
        {
            var outputFile = "huatuo/huatuo/interpreter/MethodBridge_arm64.cpp";
            GenMethodBridge_Arm64(outputFile);
        }

        public static void GenMethodBridge_Arm64(string outputFile)
        {
            var g = new MethodBridgeGenerator(new MethodBridgeGeneratorOptions
            {
                CallConvention = CallConventionType.Arm64,
                Assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList(),
                OutputFile = outputFile
            });

            g.PrepareMethods();
            g.Generate();
            Debug.LogFormat("== output:{0} ==", outputFile);
        }


        private static string GetExt()
        {
            var ext = "";

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    ext = ".apk";
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    ext = ".exe";
                    break;
            }

            return ext;
        }

        private static void CreateDirIfNotExists(string dirName)
        {
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
        }

        private static void CompileDll(string buildDir, BuildTarget target)
        {
            var group = BuildPipeline.GetBuildTargetGroup(target);

            var scriptCompilationSettings = new ScriptCompilationSettings();
            scriptCompilationSettings.group = group;
            scriptCompilationSettings.target = target;
            CreateDirIfNotExists(buildDir);
            var scriptCompilationResult =
                PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
            foreach (var ass in scriptCompilationResult.assemblies) Debug.LogFormat("compile assemblies:{0}", ass);
        }

        public static string GetDllBuildOutputDirByTarget(BuildTarget target)
        {
            // return $"{DllBuildOutputDir}/{target}";
            return $"{DllBuildOutputDir}";
        }

        [MenuItem("HuaTuo/CompileDll/ActiveBuildTarget")]
        public static void CompileDllActiveBuildTarget()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        [MenuItem("HuaTuo/CompileDll/Win64")]
        public static void CompileDllWin64()
        {
            var target = BuildTarget.StandaloneWindows64;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        [MenuItem("HuaTuo/CompileDll/Linux64")]
        public static void CompileDllLinux()
        {
            var target = BuildTarget.StandaloneLinux64;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        [MenuItem("HuaTuo/CompileDll/OSX")]
        public static void CompileDllOSX()
        {
            var target = BuildTarget.StandaloneOSX;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        [MenuItem("HuaTuo/CompileDll/Android")]
        public static void CompileDllAndroid()
        {
            var target = BuildTarget.Android;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }

        [MenuItem("HuaTuo/CompileDll/IOS")]
        public static void CompileDllIOS()
        {
            //var target = EditorUserBuildSettings.activeBuildTarget;
            var target = BuildTarget.iOS;
            CompileDll(GetDllBuildOutputDirByTarget(target), target);
        }
    }
}