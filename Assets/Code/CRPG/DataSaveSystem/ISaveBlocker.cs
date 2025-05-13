

namespace CRPG.DataSaveSystem
{
	/// <summary>
	/// Determine when object don't need save (it not need spawn on next load)
	/// </summary>
	interface ISaveBlocker
	{
		public bool IsBlockSave { get; }
	}
}