using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
using UnityEngine;

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

        if (commandLineArgs.Remove("--glot"))
            try
            {
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
                                Console.WriteLine(
                                    "========================================================================");
                                var parameterInfos = m.GetParameters();
                                object result = null;
                                if (parameterInfos.Length == 0)
                                    result = m.Invoke(null, Array.Empty<object>());
                                else if (parameterInfos.Length == 1 &&
                                         parameterInfos[0].ParameterType == typeof(string[]))
                                    result = m.Invoke(null, new object[] { Array.Empty<string>() });
                                Console.WriteLine(
                                    "========================================================================");
                                Console.WriteLine($"result: {result}");

                                break;
                            }
                        }
                    });
                Application.Quit(0);
            }
            catch (Exception e)
            {
                Application.Quit(1);
            }
    }
}