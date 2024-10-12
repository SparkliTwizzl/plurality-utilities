using Petrichor.Common.Syntax;
using System.Text;


namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Represents a module command with associated data, name, and options.
	/// </summary>
	public sealed class ModuleCommand : IEquatable<ModuleCommand>
	{
		/// <summary>
		/// Indicates that no command was provided.
		/// </summary>
		public static ModuleCommand None => new() { Name = Commands.None };

		/// <summary>
		/// Indicates that an unrecognized command was provided.
		/// </summary>
		public static ModuleCommand Some => new() { Name = Commands.Some };

		/// <summary>
		/// The input data associated with this command.
		/// </summary>
		public IndexedString[] Data { get; set; } = Array.Empty<IndexedString>();

		/// <summary>
		/// The name of the command.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Parameters and corresponding arguments for this command.
		/// </summary>
		public Dictionary<string, string> Options { get; set; } = new();


		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleCommand"/> class.
		/// </summary>
		public ModuleCommand() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleCommand"/> class by copying the values from another instance.
		/// </summary>
		/// <param name="other">The <see cref="ModuleCommand"/> instance to copy values from.</param>
		public ModuleCommand(ModuleCommand other)
		{
			Data = other.Data;
			Name = other.Name;
			Options = other.Options;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object? obj)
		{
			if (obj is null)
			{
				return false;
			}
			var other = obj as ModuleCommand;
			return Equals(other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="ModuleCommand"/> is equal to the current <see cref="ModuleCommand"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals(ModuleCommand? other)
		{
			if (other is null)
			{
				return false;
			}
			return Enumerable.SequenceEqual(Data, other.Data) && Name.Equals(other.Name) && Enumerable.SequenceEqual(Options, other.Options);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Data.GetHashCode() ^ Name.GetHashCode() ^ Options.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString()
		{
			var builder = new StringBuilder();
			_ = builder.Append(Name);
			foreach (var option in Options)
			{
				_ = builder.Append($" [{option.Key}:{option.Value}]");
			}
			foreach (var line in Data)
			{
				_ = builder.Append($" <{line.ToString()}>");
			}
			return builder.ToString();
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="InputData"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(ModuleCommand a, ModuleCommand b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="InputData"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(ModuleCommand a, ModuleCommand b) => !a.Equals(b);
	}
}
