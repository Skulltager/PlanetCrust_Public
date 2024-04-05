namespace SheetCodes
{
	//Generated code, do not edit!

	public enum EnvironmentIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Level Bounds Wall")] LevelBoundsWall = 1,
		[Identifier("Bowl")] Bowl = 2,
	}

	public static class EnvironmentIdentifierExtension
	{
		public static EnvironmentRecord GetRecord(this EnvironmentIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.EnvironmentModel.GetRecord(identifier, editableRecord);
		}
	}
}
