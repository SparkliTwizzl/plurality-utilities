using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public static class ExceptionLogger
	{
		public static void LogAndThrow( Exception exception )
		{
			var stackTrace = new System.Diagnostics.StackTrace();
			var callingMethod = stackTrace.GetFrame( 1 )?.GetMethod();
			var callingClassName = callingMethod?.DeclaringType?.Name ?? string.Empty;
			var callingMethodName = callingMethod?.Name ?? string.Empty;
			Log.Error( $"{callingClassName}.{callingMethodName} >>> {exception.Message}" );
			throw exception;
		}
	}
}
