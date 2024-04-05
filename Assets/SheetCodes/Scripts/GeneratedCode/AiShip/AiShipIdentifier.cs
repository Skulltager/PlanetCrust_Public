namespace SheetCodes
{
	//Generated code, do not edit!

	public enum AiShipIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Ship Base")] ShipBase = 1,
	}

	public static class AiShipIdentifierExtension
	{
		public static AiShipRecord GetRecord(this AiShipIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.AiShipModel.GetRecord(identifier, editableRecord);
		}
	}
}
