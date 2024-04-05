using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class PlayerShipRecord : BaseRecord<PlayerShipIdentifier>
	{
		//Does this type no longer exist? Delete from here..
		[ColumnName("Prefab")] [SerializeField] private UnityEngine.GameObject _prefab = default;
		public UnityEngine.GameObject Prefab 
		{ 
			get { return _prefab; } 
            set
            {
                if (!CheckEdit())
                    return;
#if UNITY_EDITOR
                if (value != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(value);
                    if(string.IsNullOrEmpty(assetPath))
                    {
                        Debug.LogError("SheetCodes: Reference Objects must be a direct reference from your project folder.");
                        return;
                    }
                }
                _prefab = value;
#endif
            }
        }
		//..To here

		[ColumnName("Hull")] [SerializeField] private int _hull = default;
		public int Hull { get { return _hull; } set { if(!CheckEdit()) return; _hull = value; }}

		[ColumnName("Armor")] [SerializeField] private int _armor = default;
		public int Armor { get { return _armor; } set { if(!CheckEdit()) return; _armor = value; }}

		[ColumnName("Shield")] [SerializeField] private int _shield = default;
		public int Shield { get { return _shield; } set { if(!CheckEdit()) return; _shield = value; }}

		[ColumnName("Energy")] [SerializeField] private int _energy = default;
		public int Energy { get { return _energy; } set { if(!CheckEdit()) return; _energy = value; }}

		[ColumnName("Energy Regeneration")] [SerializeField] private float _energyRegeneration = default;
		public float EnergyRegeneration { get { return _energyRegeneration; } set { if(!CheckEdit()) return; _energyRegeneration = value; }}

		[ColumnName("Radius")] [SerializeField] private float _radius = default;
		public float Radius { get { return _radius; } set { if(!CheckEdit()) return; _radius = value; }}

		[ColumnName("Acceleration")] [SerializeField] private float _acceleration = default;
		public float Acceleration { get { return _acceleration; } set { if(!CheckEdit()) return; _acceleration = value; }}

		[ColumnName("Top Speed")] [SerializeField] private float _topSpeed = default;
		public float TopSpeed { get { return _topSpeed; } set { if(!CheckEdit()) return; _topSpeed = value; }}

		[ColumnName("Rotation Acceleration")] [SerializeField] private float _rotationAcceleration = default;
		public float RotationAcceleration { get { return _rotationAcceleration; } set { if(!CheckEdit()) return; _rotationAcceleration = value; }}

		[ColumnName("Aim Min Distance")] [SerializeField] private float _aimMinDistance = default;
		public float AimMinDistance { get { return _aimMinDistance; } set { if(!CheckEdit()) return; _aimMinDistance = value; }}

		[ColumnName("Aim Max Distance")] [SerializeField] private float _aimMaxDistance = default;
		public float AimMaxDistance { get { return _aimMaxDistance; } set { if(!CheckEdit()) return; _aimMaxDistance = value; }}

		[ColumnName("Impact Resistance")] [SerializeField] private int _impactResistance = default;
		public int ImpactResistance { get { return _impactResistance; } set { if(!CheckEdit()) return; _impactResistance = value; }}

		[ColumnName("Base Min Impact Damage")] [SerializeField] private int _baseMinImpactDamage = default;
		public int BaseMinImpactDamage { get { return _baseMinImpactDamage; } set { if(!CheckEdit()) return; _baseMinImpactDamage = value; }}

		[ColumnName("Base Max Impact Damage")] [SerializeField] private int _baseMaxImpactDamage = default;
		public int BaseMaxImpactDamage { get { return _baseMaxImpactDamage; } set { if(!CheckEdit()) return; _baseMaxImpactDamage = value; }}

		[ColumnName("Min Velocity Impact Damage")] [SerializeField] private float _minVelocityImpactDamage = default;
		public float MinVelocityImpactDamage { get { return _minVelocityImpactDamage; } set { if(!CheckEdit()) return; _minVelocityImpactDamage = value; }}

		[ColumnName("Max Velocity Impact Damage")] [SerializeField] private float _maxVelocityImpactDamage = default;
		public float MaxVelocityImpactDamage { get { return _maxVelocityImpactDamage; } set { if(!CheckEdit()) return; _maxVelocityImpactDamage = value; }}

		[ColumnName("Min Velocity Impact Speed")] [SerializeField] private float _minVelocityImpactSpeed = default;
		public float MinVelocityImpactSpeed { get { return _minVelocityImpactSpeed; } set { if(!CheckEdit()) return; _minVelocityImpactSpeed = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public PlayerShipModel model { get { return ModelManager.PlayerShipModel; } }
        private PlayerShipRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            PlayerShipRecord editableCopy = new PlayerShipRecord();
            editableCopy.Identifier = Identifier;
            editableCopy.originalRecord = this;
            CopyData(editableCopy);
            model.SetEditableCopy(editableCopy);
#else
            Debug.LogError("SheetCodes: Creating an editable record does not work in buolds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        public override void SaveToScriptableObject()
        {
#if UNITY_EDITOR
            if (!runtimeEditingEnabled)
            {
                Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
                return;
            }
            CopyData(originalRecord);
            model.SaveModel();
#else
            Debug.LogError("SheetCodes: Saving to ScriptableObject does not work in builds. See documentation 'Editing your data at runtime' for more information.");
#endif
        }

        private void CopyData(PlayerShipRecord record)
        {
            record._prefab = _prefab;
            record._hull = _hull;
            record._armor = _armor;
            record._shield = _shield;
            record._energy = _energy;
            record._energyRegeneration = _energyRegeneration;
            record._radius = _radius;
            record._acceleration = _acceleration;
            record._topSpeed = _topSpeed;
            record._rotationAcceleration = _rotationAcceleration;
            record._aimMinDistance = _aimMinDistance;
            record._aimMaxDistance = _aimMaxDistance;
            record._impactResistance = _impactResistance;
            record._baseMinImpactDamage = _baseMinImpactDamage;
            record._baseMaxImpactDamage = _baseMaxImpactDamage;
            record._minVelocityImpactDamage = _minVelocityImpactDamage;
            record._maxVelocityImpactDamage = _maxVelocityImpactDamage;
            record._minVelocityImpactSpeed = _minVelocityImpactSpeed;
        }

        private bool CheckEdit()
        {
            if (runtimeEditingEnabled)
                return true;

            Debug.LogWarning("SheetCodes: Runtime Editing is not enabled for this object. Either you are not using the editable copy or you're trying to edit in a build.");
            return false;
        }
    }
}
