using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class AiShipRecord : BaseRecord<AiShipIdentifier>
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

		[ColumnName("Acceleration")] [SerializeField] private float _acceleration = default;
		public float Acceleration { get { return _acceleration; } set { if(!CheckEdit()) return; _acceleration = value; }}

		[ColumnName("Top Speed")] [SerializeField] private float _topSpeed = default;
		public float TopSpeed { get { return _topSpeed; } set { if(!CheckEdit()) return; _topSpeed = value; }}

		[ColumnName("Rotation Acceleration")] [SerializeField] private float _rotationAcceleration = default;
		public float RotationAcceleration { get { return _rotationAcceleration; } set { if(!CheckEdit()) return; _rotationAcceleration = value; }}

		[ColumnName("Radius")] [SerializeField] private float _radius = default;
		public float Radius { get { return _radius; } set { if(!CheckEdit()) return; _radius = value; }}

		[ColumnName("Path Finding Min Interval")] [SerializeField] private float _pathFindingMinInterval = default;
		public float PathFindingMinInterval { get { return _pathFindingMinInterval; } set { if(!CheckEdit()) return; _pathFindingMinInterval = value; }}

		[ColumnName("Path Finding Max Interval")] [SerializeField] private float _pathFindingMaxInterval = default;
		public float PathFindingMaxInterval { get { return _pathFindingMaxInterval; } set { if(!CheckEdit()) return; _pathFindingMaxInterval = value; }}

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
        public AiShipModel model { get { return ModelManager.AiShipModel; } }
        private AiShipRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            AiShipRecord editableCopy = new AiShipRecord();
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

        private void CopyData(AiShipRecord record)
        {
            record._prefab = _prefab;
            record._hull = _hull;
            record._armor = _armor;
            record._shield = _shield;
            record._acceleration = _acceleration;
            record._topSpeed = _topSpeed;
            record._rotationAcceleration = _rotationAcceleration;
            record._radius = _radius;
            record._pathFindingMinInterval = _pathFindingMinInterval;
            record._pathFindingMaxInterval = _pathFindingMaxInterval;
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
