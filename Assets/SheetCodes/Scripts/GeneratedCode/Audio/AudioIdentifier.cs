namespace SheetCodes
{
	//Generated code, do not edit!

	public enum AudioIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Weapon Fire")] WeaponFire = 1,
		[Identifier("Ship Explode")] ShipExplode = 2,
		[Identifier("Projectile Hit Ship")] ProjectileHitShip = 3,
		[Identifier("Projectile Hit Environment")] ProjectileHitEnvironment = 4,
	}

	public static class AudioIdentifierExtension
	{
		public static AudioRecord GetRecord(this AudioIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.AudioModel.GetRecord(identifier, editableRecord);
		}
	}
}
