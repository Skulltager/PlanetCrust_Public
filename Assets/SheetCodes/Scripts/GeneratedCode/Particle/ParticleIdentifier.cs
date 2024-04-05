namespace SheetCodes
{
	//Generated code, do not edit!

	public enum ParticleIdentifier
	{
		[Identifier("None")] None = 0,
		[Identifier("Projectile Wall Hit")] ProjectileWallHit = 1,
		[Identifier("Projectile Ship Hit")] ProjectileShipHit = 2,
		[Identifier("Projectile Discharge")] ProjectileDischarge = 3,
		[Identifier("Ship Explode")] ShipExplode = 4,
	}

	public static class ParticleBatchIdentifierExtension
	{
		public static ParticleRecord GetRecord(this ParticleIdentifier identifier, bool editableRecord = false)
		{
			return ModelManager.ParticleModel.GetRecord(identifier, editableRecord);
		}
	}
}
