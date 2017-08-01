using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public enum LogLevel
{
    None,
    Critical,
    Message,
    Verbose,
    Debug
}

public class Logger
{
    public static LogLevel LoggingLevel = LogLevel.Verbose;

    public static void Log(string message)
    {
        if (Logger.LoggingLevel >= LogLevel.Message)
        {
            Debug.Log(message);
        }
    }

    public static void Log(string message, LogLevel level)
    {
        if (Logger.LoggingLevel >= level)
        {
            Debug.Log(message);
        }
    }

    public static void Log(string message, params object[] arguments)
    {
        Logger.Log(string.Format(message, arguments));
    }

    public static void LogError(string message)
    {
        if (Logger.LoggingLevel >= LogLevel.Critical)
        {
            Debug.LogError(message);
        }
    }

    public static void LogWarning(string message)
    {
        if (Logger.LoggingLevel >= LogLevel.Critical)
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogException(Exception ex)
    {
        if (Logger.LoggingLevel >= LogLevel.Critical)
        {
            Debug.LogException(ex);
        }
    }

    private static bool useFileWriter = false; // set to true to debug stand alone apps
    private static TextWriter fileWriter = null;
    public static void LogFile(string message, params object[] arguments)
    {
        if (!useFileWriter)
            return;

        if (null == fileWriter)
        {
            //DateTime t = DateTime.Now;
            //fileWriter = new StreamWriter(string.Format("e:\\SotaPlugin_{0:D2}_{1:D2}_{2:D2}.txt", t.Hour, t.Minute, t.Second), false);
        }
        fileWriter.WriteLine(string.Format(message, arguments));
        fileWriter.Flush();
    }

    public static void LogExceptionFile(Exception ex)
    {
        if (!useFileWriter)
            return;

        Logger.LogFile("EXCEPTION: {0} {1} {2}", ex.GetType().ToString(), ex.Message, ex.StackTrace);
        Exception inex = ex.InnerException;
        while (null != inex)
        {
            Logger.LogFile("FROM: {0} {1} {2}", inex.GetType().ToString(), inex.Message, inex.StackTrace);
            inex = inex.InnerException;
        }
    }

    // this is a trick to wire unmanaged logger to Unity logger
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void SVFPluginLog(string log);

    private static readonly SVFPluginLog pluginLog = Logger.Log;
    public static readonly IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(pluginLog);

}
