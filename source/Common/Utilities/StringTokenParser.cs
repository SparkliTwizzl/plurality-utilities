using Petrichor.Common.Containers;
using Petrichor.Common.Enums;
using Petrichor.Common.Info;
using Petrichor.Logging;


namespace Petrichor.Common.Utilities
{
	public class StringTokenParser
	{
		public int IndentLevel { get; set; } = 0;


		public StringTokenParser() { }


		public QualifiedStringToken ParseToken( string token, string[] expectedValues )
		{
			var taskMessage = $"parsing token \"{token}\"";
			Log.TaskStarted( taskMessage );

			var qualifiedToken = new QualifiedStringToken( token );

			if ( qualifiedToken.Value == string.Empty )
			{
				qualifiedToken.Qualifier = StringTokenQualifiers.BlankLine;
			}

			else if ( qualifiedToken.Value == CommonSyntax.OpenBracketToken )
			{
				++IndentLevel;
				qualifiedToken.Qualifier = StringTokenQualifiers.OpenBracket;
			}

			else if ( qualifiedToken.Value == CommonSyntax.CloseBracketToken )
			{
				--IndentLevel;
				qualifiedToken.Qualifier = StringTokenQualifiers.CloseBracket;
			}

			else
			{
				if ( qualifiedToken.Value.IndexOf( CommonSyntax.LineCommentToken ) == 0 )
				{
					qualifiedToken.Qualifier = StringTokenQualifiers.BlankLine;
				}
				else
				{
					foreach ( var value in expectedValues )
					{
						if ( string.Compare( qualifiedToken.Value, value ) == 0 )
						{
							qualifiedToken.Qualifier = StringTokenQualifiers.Recognized;
							break;
						}
					}
				}
			}
			Log.TaskFinished( taskMessage );
			return qualifiedToken;
		}
	}
}
