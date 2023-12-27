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

			var qualifiedToken = new QualifiedStringToken( token.Trim() );


			switch ( qualifiedToken.Value )
			{
				case "":
				{
					qualifiedToken.Qualifier = StringTokenQualifiers.BlankLine;
					break;
				}

				case CommonSyntax.OpenBracketToken:
				{
					++IndentLevel;
					qualifiedToken.Qualifier = StringTokenQualifiers.OpenBracket;
					break;
				}

				case CommonSyntax.CloseBracketToken:
				{
					--IndentLevel;
					qualifiedToken.Qualifier = StringTokenQualifiers.CloseBracket;
					break;
				}

				default:
				{
					if ( qualifiedToken.Value[ 0..CommonSyntax.LineCommentToken.Length ] == CommonSyntax.LineCommentToken )
					{
						qualifiedToken.Qualifier = StringTokenQualifiers.BlankLine;
						break;
					}

					foreach ( var value in expectedValues )
					{
						if ( string.Compare( qualifiedToken.Value, value ) == 0 )
						{
							qualifiedToken.Qualifier = StringTokenQualifiers.Recognized;
							break;
						}
					}
					break;
				}
			}
			Log.TaskFinished( taskMessage );
			return qualifiedToken;
		}
	}
}
