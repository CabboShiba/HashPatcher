﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using HarmonyLib;
using System.Collections.Specialized;

namespace HashPatcher
{
    internal class Program
    {
        private static string Content = "Assembly patched by https://github.com/CabboShiba/";
        private static string File = null;
        private static string HookFilePath = null;
        private static string PatchID = $"{Utils.RandomString(10)}.cabbo.patch.https://github.com/CabboShiba/";
        static void Main(string[] args)
        {
            while (!System.IO.File.Exists(File))
            {
                Log("Please insert your Assembly to patch: ");
                File = Console.ReadLine();
            }
            while (!System.IO.File.Exists(HookFilePath))
            {
                Log("Please insert your HookedFilePath: ");
                HookFilePath = Console.ReadLine();
            }
            try
            {
                var assembly = Assembly.LoadFile(Path.GetFullPath(File));
                var paraminfo = assembly.EntryPoint.GetParameters();
                object[] parameters = new object[paraminfo.Length];
                Harmony patch = new Harmony(PatchID);
                Console.Title = Content;
                patch.PatchAll(Assembly.GetExecutingAssembly());
                assembly.EntryPoint.Invoke(null, parameters);
                Log("Assembly succesfully loaded and patched!");
            }
            catch (Exception ex)
            {
                Log($"Could not load {File}\n{ex.Message}");
            }
            Console.ReadLine();
        }
        [HarmonyPatch(typeof(System.IO.File), nameof(System.IO.File.OpenRead), MethodType.Normal)]
        class HashPatch
        {
            static bool Prefix(ref string path)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                try
                {
                    Log($"Original FilePath: GetHash.exe");
                    Log("Patching...");
                    path = HookFilePath;
                    Log($"Patched FilePath: YullySpoofer.exe");
                }
                catch (Exception ex)
                {
                    Log($"Error during Patch:\n {ex}");
                }
                Console.ResetColor();
                return true;
            }
        }
        public static void Log(string Data)
        {
            string Log = $"[{DateTime.Now} - Patcher] {Data}";
            Console.WriteLine(Log);
        }
    }
}
