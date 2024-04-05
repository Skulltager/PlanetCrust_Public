namespace SheetCodes
{
	//Generated code, do not edit!

	public enum ProjectileIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Projectile")] Projectile = 1,
	}

	public static class ProjectileIdentifierExtension
	{
		public static ProjectileRecord GetRecord(this ProjectileIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.ProjectileModel.GetRecord(identifier, editableRecord);
		}
	}
}
