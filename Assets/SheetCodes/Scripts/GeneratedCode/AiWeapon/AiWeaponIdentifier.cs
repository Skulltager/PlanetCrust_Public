namespace SheetCodes
{
	//Generated code, do not edit!

	public enum AiWeaponIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Base Weapon")] BaseWeapon = 1,
	}

	public static class AiWeaponIdentifierExtension
	{
		public static AiWeaponRecord GetRecord(this AiWeaponIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.AiWeaponModel.GetRecord(identifier, editableRecord);
		}
	}
}
