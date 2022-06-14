using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class Options
{
    [Option('a', "assembly", Required = true, HelpText = "dll or exe")]
    public string assembly { get; set; }
}

public class TestDriver : MonoBehaviour
{
    private void Start()
    {
        Dispatch();
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Startup()
    {
        var o = new GameObject("TestDriver");
        o.AddComponent<TestDriver>();
    }

    public static void Dispatch()
    {
        var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());
        if (commandLineArgs.Count > 0) commandLineArgs.RemoveAt(0);
        try
        {
            Console.WriteLine(
                "################ignore the above output################");
            Console.WriteLine(
                "========================================================================");
            if (commandLineArgs.Remove("--glot"))
                Parser.Default.ParseArguments<Options>(commandLineArgs)
                    .WithParsed(o =>
                    {
                        var bytes = File.ReadAllBytes(o.assembly);
                        var ass = Assembly.Load(bytes);

                        var types = ass.GetTypes();

                        foreach (var type in types)
                        {
                            var m = type.GetMethod("Main",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            if (m != null)
                            {
                                var parameterInfos = m.GetParameters();
                                object result = null;
                                if (parameterInfos.Length == 0)
                                    result = m.Invoke(null, Array.Empty<object>());
                                else if (parameterInfos.Length == 1 &&
                                         parameterInfos[0].ParameterType == typeof(string[]))
                                    result = m.Invoke(null, new object[] { Array.Empty<string>() });

                                Console.WriteLine($"result: {result}");
                                break;
                            }
                        }
                    });
            else if (commandLineArgs.Remove("--ilruntime"))
                Parser.Default.ParseArguments<Options>(commandLineArgs)
                    .WithParsed(o =>
                    {
                        var appDomain = new AppDomain();
                        using var stream = File.Open(o.assembly, FileMode.Open);
                        appDomain.LoadAssembly(stream);
                        object result = null;

                        foreach (var kvp in appDomain.LoadedTypes.ToList())
                        {
                            var m = kvp.Value.GetMethod("Main", 0);
                            if (m is { IsStatic: true })
                            {
                                result = appDomain.Invoke(m, null);
                                break;
                            }

                            m = kvp.Value.GetMethod("Main", 1);
                            if (m is { IsStatic: true })
                                if (m.Parameters[0] == appDomain.GetType(typeof(string[])))
                                {
                                    result = appDomain.Invoke(m, null, new object[] { Array.Empty<string>() });
                                    break;
                                }
                        }

                        Console.WriteLine($"result: {result}");
                    });

            Console.WriteLine(
                "end========================================================================");
            Application.Quit(0);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Application.Quit(1);
        }
    }
}