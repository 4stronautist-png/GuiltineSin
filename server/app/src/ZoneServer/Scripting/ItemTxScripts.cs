using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

namespace GuiltineSin.Zone.Scripting
{
	/// <summary>
	/// A function that handles a transaction.
	/// </summary>
	/// <param name="character"></param>
	/// <param name="item"></param>
	/// <param name="numArgs"></param>
	/// <returns></returns>
	public delegate ItemTxResult ItemTxNumScriptFunc(Character character, Item item, int[] numArgs);

	/// <summary>
	/// Specifies the result of the transaction.
	/// </summary>
	public enum ItemTxResult
	{
		/// <summary>
		/// The transaction was successful.
		/// </summary>
		Okay,

		/// <summary>
		/// The transaction failed.
		/// </summary>
		Fail,
	}
}
