using Petrichor.Common.Containers;

namespace Petrichor.Common.Utilities
{
	public interface IDataRegionParser<out T> where T : new()
	{
		static Func<IndexedString[], int, T, ProcessedRegionData<T>> InertHandler => ( IndexedString[] regionData, int tokenStartIndex, T result ) => new() { Value = result };


		int LinesParsed { get; }
		DataToken RegionToken { get; }
		Dictionary<string, int> TokenInstancesParsed { get; }


		T Parse( IndexedString[] regionData );

		void Reset();
	}
}
