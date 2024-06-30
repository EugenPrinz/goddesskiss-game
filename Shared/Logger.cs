using System.IO;

namespace Shared
{
	public class Logger
	{
		public static void Log(object msg)
		{
		}

		public static void LogToFile(object msg)
		{
			using StreamWriter streamWriter = new StreamWriter("./debug.log", append: true);
			streamWriter.WriteLine(msg);
			streamWriter.Flush();
		}
	}
}
