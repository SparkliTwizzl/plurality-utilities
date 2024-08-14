namespace Petrichor.Logging.Utilities
{
	public static class ExceptionLogger
	{
		private static readonly int callingMethodFrame = 1;


		public static void LogAndThrow( Exception exception, int? lineNumber = null )
		{
			var stackTrace = new System.Diagnostics.StackTrace();
			var callingMethod = stackTrace.GetFrame( callingMethodFrame )?.GetMethod();
			var callingClassName = callingMethod?.DeclaringType?.Name ?? string.Empty;
			var callingMethodName = callingMethod?.Name ?? string.Empty;
			Log.Error( exception.Message, lineNumber );
			Log.Debug( $"[class: {callingClassName}]  [method: {callingMethodName}]  [exception: {exception}]", lineNumber );
			throw exception;
		}
	}
}
