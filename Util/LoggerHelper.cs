using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Util
{
	public class LoggerHelper
	{
		public static LogLevel CurrentLogLevels;

		private const bool SHOW_STACK = true;

		private static LogWriter m_logWriter;

		public static string DebugFilterStr;

		public static string sLogPathFile;

		private static StreamWriter m_Writer;

		private static ulong index;

		static LoggerHelper()
		{
			CurrentLogLevels = LogLevel.DEBUG | LogLevel.INFO | LogLevel.WARNING | LogLevel.ERROR | LogLevel.EXCEPT | LogLevel.CRITICAL;
			DebugFilterStr = string.Empty;
			index = 0uL;
			m_logWriter = new LogWriter();
			Application.RegisterLogCallback(ProcessExceptionReport);
			string streamLogPath = GetStreamLogPath();
			sLogPathFile = Path.Combine(streamLogPath, "unityexceptions.txt");
			m_Writer = new StreamWriter(sLogPathFile);
			m_Writer.AutoFlush = true;
			Application.logMessageReceived += HandlerProcess;
		}

		private static void HandlerProcess(string condition, string stackTrace, LogType type)
		{
			switch (type)
			{
			case LogType.Exception:
				m_Writer.WriteLine("{0}: {1}\n{2}", type, condition, stackTrace);
				Application.Quit();
				break;
			}
		}

		public static void Release()
		{
			Application.logMessageReceived -= HandlerProcess;
			m_logWriter.Release();
		}

		public static string GetFileName()
		{
			return m_logWriter.GetLogFilePath();
		}

		public static string GetStreamLogPath()
		{
			return m_logWriter.GetStreamLogPath();
		}

		public static void UploadLogFile()
		{
			m_logWriter.UploadTodayLog();
		}

		public static void DeleteFile()
		{
			m_logWriter.Delete();
		}

		public static void Debug(object message, bool isShowStack = true, int user = 0)
		{
			if (!(DebugFilterStr != string.Empty) && (CurrentLogLevels & LogLevel.DEBUG) == LogLevel.DEBUG)
			{
				Log(string.Concat(" [DEBUG]: ", (!isShowStack) ? string.Empty : GetStackInfo(), message, " Index = ", index++), LogLevel.DEBUG);
			}
		}

		public static void Debug(string filter, object message, bool isShowStack = true)
		{
			if ((!(DebugFilterStr != string.Empty) || !(DebugFilterStr != filter)) && (CurrentLogLevels & LogLevel.DEBUG) == LogLevel.DEBUG)
			{
				Log(" [DEBUG]: " + ((!isShowStack) ? string.Empty : GetStackInfo()) + message, LogLevel.DEBUG);
			}
		}

		public static void Info(object message, bool isShowStack = true)
		{
			if ((CurrentLogLevels & LogLevel.INFO) == LogLevel.INFO)
			{
				Log(" [INFO]: " + ((!isShowStack) ? string.Empty : GetStackInfo()) + message, LogLevel.INFO);
			}
		}

		public static void Warning(object message, bool isShowStack = true)
		{
			if ((CurrentLogLevels & LogLevel.WARNING) == LogLevel.WARNING)
			{
				Log(" [WARNING]: " + ((!isShowStack) ? string.Empty : GetStackInfo()) + message, LogLevel.WARNING);
			}
		}

		public static void Error(object message, bool isShowStack = true)
		{
			if ((CurrentLogLevels & LogLevel.ERROR) == LogLevel.ERROR)
			{
				Log(string.Concat(" [ERROR]: ", message, '\n', (!isShowStack) ? string.Empty : GetStacksInfo()), LogLevel.ERROR);
			}
		}

		public static void Critical(object message, bool isShowStack = true)
		{
			if ((CurrentLogLevels & LogLevel.CRITICAL) == LogLevel.CRITICAL)
			{
				Log(string.Concat(" [CRITICAL]: ", message, '\n', (!isShowStack) ? string.Empty : GetStacksInfo()), LogLevel.CRITICAL);
			}
		}

		public static void Except(Exception ex, object message = null)
		{
			if ((CurrentLogLevels & LogLevel.EXCEPT) == LogLevel.EXCEPT)
			{
				Exception ex2 = ex;
				while (ex2.InnerException != null)
				{
					ex2 = ex2.InnerException;
				}
				Log(" [EXCEPT]: " + ((message != null) ? string.Concat(message, "\n") : string.Empty) + ex.Message + ex2.StackTrace, LogLevel.CRITICAL);
			}
		}

		private static string GetStacksInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StackTrace stackTrace = new StackTrace();
			StackFrame[] frames = stackTrace.GetFrames();
			for (int i = 2; i < frames.Length; i++)
			{
				stringBuilder.AppendLine(frames[i].ToString());
			}
			return stringBuilder.ToString();
		}

		private static void Log(string message, LogLevel level, bool writeEditorLog = true)
		{
		}

		private static string GetStackInfo()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame frame = stackTrace.GetFrame(2);
			MethodBase method = frame.GetMethod();
			return $"{method.ReflectedType.Name}.{method.Name}(): ";
		}

		private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
		{
			LogLevel logLevel = LogLevel.DEBUG;
			switch (type)
			{
			case LogType.Assert:
				logLevel = LogLevel.DEBUG;
				break;
			case LogType.Error:
				logLevel = LogLevel.ERROR;
				break;
			case LogType.Exception:
				logLevel = LogLevel.EXCEPT;
				break;
			case LogType.Log:
				logLevel = LogLevel.DEBUG;
				break;
			case LogType.Warning:
				logLevel = LogLevel.WARNING;
				break;
			}
			if (logLevel == (CurrentLogLevels & logLevel))
			{
				Log(string.Concat(" [SYS_", logLevel, "]: ", message, '\n', stackTrace), logLevel, writeEditorLog: false);
			}
		}
	}
}
