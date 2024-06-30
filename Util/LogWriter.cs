using System;
using System.IO;
using UnityEngine;

namespace Util
{
	public class LogWriter
	{
		private string m_logPath = Application.persistentDataPath + "/log/";

		private string m_logFileName = "log_{0}.txt";

		private string m_logFilePath;

		private string m_logStreamPath;

		private FileStream m_fs;

		private StreamWriter m_sw;

		private Action<string, LogLevel, bool> m_logWriter;

		private static readonly object m_locker = new object();

		public LogWriter()
		{
			if (!Directory.Exists(m_logPath))
			{
				Directory.CreateDirectory(m_logPath);
			}
			m_logFilePath = m_logPath + string.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd"));
			try
			{
				m_logWriter = Write;
				m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				m_sw = new StreamWriter(m_fs);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
		}

		public string GetLogFilePath()
		{
			return m_logFilePath;
		}

		public string GetStreamLogPath()
		{
			return m_logPath;
		}

		public void Delete()
		{
			File.Delete(m_logFilePath);
		}

		public void Release()
		{
			lock (m_locker)
			{
				if (m_sw != null)
				{
					m_sw.Close();
					m_sw.Dispose();
				}
				if (m_fs != null)
				{
					m_fs.Close();
					m_fs.Dispose();
				}
			}
		}

		public void UploadTodayLog()
		{
			lock (m_locker)
			{
				using FileStream stream = new FileStream(m_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using StreamReader streamReader = new StreamReader(stream);
				string text = streamReader.ReadToEnd();
				string logFilePath = m_logFilePath;
			}
		}

		public void UploadLogFile(string filename, string content)
		{
		}

		public void WriteLog(string msg, LogLevel level, bool writeEditorLog)
		{
			m_logWriter.BeginInvoke(msg, level, writeEditorLog, null, null);
		}

		private void Write(string msg, LogLevel level, bool writeEditorLog)
		{
			lock (m_locker)
			{
				try
				{
					if (writeEditorLog)
					{
						switch (level)
						{
						case LogLevel.DEBUG:
						case LogLevel.INFO:
							UnityEngine.Debug.Log(msg);
							break;
						case LogLevel.WARNING:
							UnityEngine.Debug.LogWarning(msg);
							break;
						case LogLevel.ERROR:
						case LogLevel.EXCEPT:
						case LogLevel.CRITICAL:
							UnityEngine.Debug.LogError(msg);
							break;
						}
					}
					if (m_sw != null)
					{
						m_sw.WriteLine(msg);
						m_sw.Flush();
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError(ex.Message);
				}
			}
		}
	}
}
