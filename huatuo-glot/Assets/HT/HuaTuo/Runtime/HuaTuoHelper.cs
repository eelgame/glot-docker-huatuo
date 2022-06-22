using System;
using System.Reflection.Emit;
using Huatuo;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace HuaTuo.Runtime
{
    public enum RuntimeBackend
    {
        FULL_AOT,
        HUATUO_AOT,
        MONO
    }

    public static class HuaTuoHelper
    {
        public static string[] aotDlls =
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll" // 如果使用了Linq，需要这个
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Startup()
        {
            BetterStreamingAssets.Initialize();
            LoadMetadataForAOTAssembly();
        }

        public static int SizeOf(Type t)
        {
            var method = new DynamicMethod("ComputeSizeOfImpl", typeof(int), Type.EmptyTypes, typeof(Utils), false);
            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Sizeof, t);
            gen.Emit(OpCodes.Ret);
            var size = ((Func<int>)method.CreateDelegate(typeof(Func<int>)))();
            return size;
        }

        public static bool IsInterpreterType(this Type t)
        {
            return t.Module.MetadataToken >= 1 << 26;
        }

        /// <summary>
        ///     为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        ///     一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        public static unsafe void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用
            // 原始dll。
            // 这里以最常用的mscorlib.dll举例
            //
            foreach (var dllName in aotDlls)
                if (BetterStreamingAssets.FileExists(dllName))
                {
                    var dllBytes = BetterStreamingAssets.ReadAllBytes(dllName);

                    fixed (byte* ptr = dllBytes)
                    {
                        // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                        var err = HuatuoApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                        Debug.Log("LoadMetadataForAOTAssembly. ret:" + err);
                    }
                }
        }
    }
}