using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class LogManager
{
    private static List<LogEntry> logEntries = new List<LogEntry>();
    private static string logFilePath;

    static LogManager()
    {
        EventManager.Log.OnLogEvent += HandleLogEvent;
        string fileName = "nocomando_log.txt";

        logFilePath = Path.Combine(Application.persistentDataPath, fileName);

        string directoryPath = Path.GetDirectoryName(logFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public static void Init()
    {
        Debug.Log("Log Directory >>> " + logFilePath);
    }

    private static void HandleLogEvent(LogTypeInfo logType, string logText)
    {
        LogEntry entry = new LogEntry
        {
            TimeStamp = DateTime.Now,
            Type = logType,
            Message = logText
        };

        logEntries.Add(entry);

        try
        {
            // Save to file
            string logLine = $"[{entry.TimeStamp:yyyy-MM-dd HH:mm:ss}] [{entry.Type}] {entry.Message}";
            File.AppendAllText(logFilePath, logLine + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write to log file: {ex.Message}");
        }

        Debug.Log($"[{entry.Type}] {entry.Message}");
    }

    public static List<LogEntry> GetLogs()
    {
        return new List<LogEntry>(logEntries);
    }

    public static void ClearLogs()
    {
        logEntries.Clear();
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }
    }
}

public class LogEntry
{
    public DateTime TimeStamp { get; set; }
    public LogTypeInfo Type { get; set; }
    public string Message { get; set; }
}

public enum LogTypeInfo { GAME_START, GAME_END, INFO, EVENT, ACTION }

