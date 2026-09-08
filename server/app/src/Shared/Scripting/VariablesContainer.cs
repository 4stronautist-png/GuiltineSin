using Yggdrasil.Util;

namespace GuiltineSin.Shared.Scripting
{
	/// <summary>
	/// Holds references to a set of permanent and temporary variables.
	/// </summary>
	public class VariablesContainer
	{
		/// <summary>
		/// Returns the permanent variables.
		/// </summary>
		public Variables Perm { get; } = new();

		/// <summary>
		/// Returns the temporary variables.
		/// </summary>
		public Variables Temp { get; } = new();
	}
}
