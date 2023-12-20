namespace Petrichor.ShortcutScriptGeneration.Containers
{
	public class ShortcutScriptMetadata
	{
		private string defaultIconPath = string.Empty;


		public string DefaultIconPath
		{
			get => defaultIconPath;
			set => defaultIconPath = value;
		}


		public ShortcutScriptMetadata() { }
		public ShortcutScriptMetadata( string defaultIconPath )
		{
			DefaultIconPath = defaultIconPath;
		}


		public static bool operator ==( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return left.DefaultIconPath.Equals( right.DefaultIconPath );
		}

		public static bool operator !=( ShortcutScriptMetadata left, ShortcutScriptMetadata right )
		{
			return !left.DefaultIconPath.Equals( right.DefaultIconPath );
		}

		public override bool Equals( object? obj )
		{
			if ( obj == null || GetType() != obj.GetType() )
			{
				return false;
			}
			return this == ( ShortcutScriptMetadata )obj;
		}

		public override int GetHashCode()
		{
			return DefaultIconPath.GetHashCode();
		}
	}
}
