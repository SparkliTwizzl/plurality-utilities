using Petrichor.Common.Syntax;


namespace Petrichor.Common.Containers
{
	public struct DataToken
	{
		public string Key { get; set; } = string.Empty;
		public int MaxAllowed { get; set; } = int.MaxValue;
		public int MinRequired { get; set; } = 0;


		public DataToken() { }

		public string Qualify() => $"{Key}{OperatorChars.TokenValueDivider}";
	}
}
