using Petrichor.Common.Containers;

namespace Petrichor.Common.Utilities
{
	public interface IRegionParser<out T> where T : new()
	{
		static Func<IndexedString[], int, T, RegionData<T>> InertHandler => ( IndexedString[] regionData, int tokenStartIndex, T result ) => new() { Value = result };


		int LinesParsed { get; }
		Dictionary<string, int> MaxAllowedTokenInstances { get; }
		Dictionary<string, int> MinRequiredTokenInstances { get; }
		string RegionName { get; }
		Dictionary<string, int> TokenInstancesParsed { get; }


		T Parse( IndexedString[] regionData );

		void Reset();
	}
}
