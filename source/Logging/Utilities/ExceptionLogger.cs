namespace Petrichor.Logging.Utilities
{
	/// <summary>
	/// A static class providing utilities to log and throw exceptions.
	/// </summary>
	public static class ExceptionLogger
	{
		private static readonly int CallingMethodFrame = 1;


		/// <summary>
		/// Logs an error corresponding to an exception, then throws that exception.
		/// </summary>
		/// <param name="exception">The exception to log and throw.</param>
		/// <param name="lineNumber">Optional line number for debugging purposes.</param>
		public static void LogAndThrow( Exception exception, int? lineNumber = null )
		{
			var stackTrace = new System.Diagnostics.StackTrace();
			var callingMethod = stackTrace.GetFrame( CallingMethodFrame )?.GetMethod();
			var callingClassName = callingMethod?.DeclaringType?.Name ?? string.Empty;
			var callingMethodName = callingMethod?.Name ?? string.Empty;
			Logger.Error( exception.Message, lineNumber );
			Logger.Debug( $"[class: {callingClassName}]  [method: {callingMethodName}]  [exception: {exception}]", lineNumber );
			throw exception;
		}
	}
}
