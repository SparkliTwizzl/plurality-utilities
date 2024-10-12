namespace Petrichor.Common.Containers
{
	/// <summary>
	/// Represents a processed data token.
	/// </summary>
	/// <typeparam name="T">The type of the value stored in the processed token.</typeparam>
	public sealed class ProcessedTokenData<T> : IEquatable<ProcessedTokenData<T>> where T : class, new()
	{
		/// <summary>
		/// The size of the token's body.
		/// </summary>
		public int BodySize { get; set; } = 0;

		/// <summary>
		/// The value processed from the token.
		/// </summary>
		public T Value { get; set; } = new();


		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessedTokenData{T}"/> class.
		/// </summary>
		public ProcessedTokenData() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessedTokenData{T}"/> class with the specified value and body size.
		/// </summary>
		/// <param name="value">The value processed from the token.</param>
		/// <param name="bodySize">The size of the token's body. Defaults to 0.</param>
		public ProcessedTokenData(T value, int bodySize = 0)
		{
			BodySize = bodySize;
			Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessedTokenData{T}"/> class by copying the values from another instance.
		/// </summary>
		/// <param name="other">The instance of <see cref="ProcessedTokenData{T}"/> to copy values from.</param>
		public ProcessedTokenData(ProcessedTokenData<T> other)
		{
			BodySize = other.BodySize;
			Value = other.Value;
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
			var other = obj as ProcessedTokenData<T>;
			return Equals(other!);
		}

		/// <summary>
		/// Determines whether the specified <see cref="ProcessedTokenData{T}"/> is equal to the current <see cref="ProcessedTokenData{T}"/>.
		/// </summary>
		/// <param name="other">The instance to compare with the current instance.</param>
		/// <returns>true if the specified instance is equal to the current instance; otherwise, false.</returns>
		public bool Equals(ProcessedTokenData<T>? other)
		{
			if (other is null)
			{
				return false;
			}
			var bodySizeEqual = BodySize.Equals(other.BodySize);
			var valueEqual = EqualityComparer<T>.Default.Equals(Value, other.Value);
			return bodySizeEqual && valueEqual;
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => BodySize.GetHashCode() ^ Value!.GetHashCode();

		/// <summary>
		/// Serves as the default stringification function.
		/// </summary>
		/// <returns>This object's data as a formatted string.</returns>
		public override string ToString() => $"{nameof(BodySize)}={BodySize}, {nameof(Value)}={Value!}";

		/// <summary>
		/// Determines whether two specified instances of <see cref="ProcessedTokenData{T}"/> are equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(ProcessedTokenData<T> a, ProcessedTokenData<T> b) => a.Equals(b);

		/// <summary>
		/// Determines whether two specified instances of <see cref="ProcessedTokenData{T}"/> are not equal.
		/// </summary>
		/// <param name="a">The first instance to compare.</param>
		/// <param name="b">The second instance to compare.</param>
		/// <returns>true if the instances are not equal; otherwise, false.</returns>
		public static bool operator !=(ProcessedTokenData<T> a, ProcessedTokenData<T> b) => !a.Equals(b);
	}
}
