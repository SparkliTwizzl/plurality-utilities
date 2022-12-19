namespace Logging
{
	public static class Log
	{
		static Log() { }


		public static void Write(string message = "")
		{
			Console.Write(message);
		}

		public static void WriteLine(string message = "")
		{
			Write(message + '\n');
		}
	}
}