using Petrichor.Common.Syntax;
using System.Text;


namespace Petrichor.Common.Containers
{
	public sealed class ModuleCommand : IEquatable<ModuleCommand>
	{
		public static ModuleCommand None => new() { Name = Commands.None };
		public static ModuleCommand Some => new() { Name = Commands.Some };


		public IndexedString[] Data { get; set; } = Array.Empty<IndexedString>();
		public string Name { get; set; } = string.Empty;
		public Dictionary<string, string> Options { get; set; } = new();


		public ModuleCommand() { }
		public ModuleCommand( ModuleCommand other )
		{
			Data = other.Data;
			Name = other.Name;
			Options = other.Options;
		}


		public override bool Equals( object? obj )
		{
			if ( obj is null )
			{
				return false;
			}
			var other = obj as ModuleCommand;
			return Equals( other );
		}

		public bool Equals( ModuleCommand? other )
		{
			if ( other is null )
			{
				return false;
			}
			return Enumerable.SequenceEqual( Data, other.Data ) && Name.Equals( other.Name ) && Enumerable.SequenceEqual( Options, other.Options );
		}

		public override int GetHashCode() => Data.GetHashCode() ^ Name.GetHashCode() ^ Options.GetHashCode();

		public override string ToString()
		{
			var builder = new StringBuilder();
			_ = builder.Append( Name );
			foreach ( var option in Options )
			{
				_ = builder.Append( $" [{option.Key}:{option.Value}]" );
			}
			foreach ( var line in Data )
			{
				_ = builder.Append( $" <{line.ToString}>" );
			}
			return builder.ToString();
		}

		public static bool operator ==( ModuleCommand a, ModuleCommand b ) => a.Equals( b );

		public static bool operator !=( ModuleCommand a, ModuleCommand b ) => !a.Equals( b );
	}
}
