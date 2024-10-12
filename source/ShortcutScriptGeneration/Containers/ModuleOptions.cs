namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public sealed class ModuleOptions : IEquatable<ModuleOptions>
	{
		/// <summary>
		/// Gets or sets the file path for the shortcut script's default icon.
		/// </summary>
		public string DefaultIconFilePath { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the keyboard shortcut to reload the shortcut script.
		/// </summary>
		public string ReloadShortcut { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the file path for the shortcut script's suspend icon.
		/// </summary>
		public string SuspendIconFilePath { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the keyboard shortcut to suspend and resume the shortcut script.
		/// </summary>
		public string SuspendShortcut { get; set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleOptions"/> class.
		/// </summary>
		public ModuleOptions() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleOptions"/> class with the specified parameters.
		/// </summary>
		/// <param name="defaultIconPath">The file path for the default icon.</param>
		/// <param name="suspendIconPath">The file path for the suspend icon.</param>
		/// <param name="reloadShortcut">The keyboard shortcut to reload the script.</param>
		/// <param name="suspendShortcut">The keyboard shortcut to suspend and resume the script.</param>
		public ModuleOptions( string defaultIconPath, string suspendIconPath, string reloadShortcut, string suspendShortcut )
		{
			DefaultIconFilePath = defaultIconPath;
			ReloadShortcut = reloadShortcut;
			SuspendIconFilePath = suspendIconPath;
			SuspendShortcut = suspendShortcut;
		}


		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return Equals( ( ModuleOptions ) obj );
		}

		/// <summary>
		/// Determines whether the specified <see cref="ModuleOptions"/> is equal to the current <see cref="ModuleOptions"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals( ModuleOptions? other )
		{
			if ( other is null )
			{
				return false;
			}
			return DefaultIconFilePath.Equals( other.DefaultIconFilePath ) && ReloadShortcut.Equals( other.ReloadShortcut ) && SuspendIconFilePath.Equals( other.SuspendIconFilePath ) && SuspendShortcut.Equals( other.SuspendShortcut );
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => DefaultIconFilePath.GetHashCode() ^ ReloadShortcut.GetHashCode() ^ SuspendIconFilePath.GetHashCode() ^ SuspendShortcut.GetHashCode();

		/// <summary>
		/// Determines whether two specified instances of <see cref="ModuleOptions"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(ModuleOptions a, ModuleOptions b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="ModuleOptions"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(ModuleOptions a, ModuleOptions b) => !a.Equals(b);
	}
}
