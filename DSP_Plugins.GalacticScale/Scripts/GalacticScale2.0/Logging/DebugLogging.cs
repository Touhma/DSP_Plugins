﻿using GSFullSerializer;
using System.Diagnostics;
using BepInEx.Logging;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void Log(string s)
        {
            if (debugOn) Bootstrap.Debug(GetCaller() +s);
        }
        public static void Error(string message)
        {
            Bootstrap.Debug(GetCaller()+message, LogLevel.Error, true);
            GS2.DumpError(message);
        }
        public static void Warn(string message)
        {
            Bootstrap.Debug(GetCaller() + message, LogLevel.Warning, true);
        }
        public static void LogJson(object o)
        {
            if (!debugOn) return;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(o, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json);
        }
        public static string GetCaller(int depth = 0)
        {
            depth += 2;
            StackTrace stackTrace = new StackTrace();
            if (stackTrace.FrameCount <= depth) return "";
            string methodName = stackTrace.GetFrame(depth).GetMethod().Name;
            string[] classPath = stackTrace.GetFrame(depth).GetMethod().ReflectedType.ToString().Split('.');
            string className = classPath[classPath.Length - 1];
            if (methodName == ".ctor") methodName = "<Constructor>";
            return className+"|"+methodName+"|";
        }
    }
}