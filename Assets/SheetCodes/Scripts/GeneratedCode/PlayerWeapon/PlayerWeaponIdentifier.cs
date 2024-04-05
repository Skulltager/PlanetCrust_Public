namespace SheetCodes
{
	//Generated code, do not edit!

	public enum PlayerWeaponIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Test Weapon")] TestWeapon = 1,
	}

	public static class PlayerWeaponIdentifierExtension
	{
		public static PlayerWeaponRecord GetRecord(this PlayerWeaponIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.PlayerWeaponModel.GetRecord(identifier, editableRecord);
		}
	}
}
