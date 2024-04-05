namespace SheetCodes
{
	//Generated code, do not edit!

	public enum SceneIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Level 1")] Level1 = 2,
		[Identifier("Level 2")] Level2 = 3,
	}

	public static class SceneIdentifierExtension
	{
		public static SceneRecord GetRecord(this SceneIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.SceneModel.GetRecord(identifier, editableRecord);
		}
	}
}
