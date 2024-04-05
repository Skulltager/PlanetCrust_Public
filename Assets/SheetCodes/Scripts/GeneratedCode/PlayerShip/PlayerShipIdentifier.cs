namespace SheetCodes
{
	//Generated code, do not edit!

	public enum PlayerShipIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Test Player")] TestPlayer = 1,
	}

	public static class PlayerShipIdentifierExtension
	{
		public static PlayerShipRecord GetRecord(this PlayerShipIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.PlayerShipModel.GetRecord(identifier, editableRecord);
		}
	}
}
