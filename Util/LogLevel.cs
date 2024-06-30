using System;

namespace Util
{
	[Flags]
	public enum LogLevel
	{
		NONE = 0,
		DEBUG = 1,
		INFO = 2,
		WARNING = 4,
		ERROR = 8,
		EXCEPT = 0x10,
		CRITICAL = 0x20
	}
}
