using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class EnvironmentRecord : BaseRecord<EnvironmentIdentifier>
	{
		[ColumnName("Base Min Impact Damage")] [SerializeField] private int _baseMinImpactDamage = default;
		public int BaseMinImpactDamage { get { return _baseMinImpactDamage; } set { if(!CheckEdit()) return; _baseMinImpactDamage = value; }}

		[ColumnName("Base Max Impact Damage")] [SerializeField] private int _baseMaxImpactDamage = default;
		public int BaseMaxImpactDamage { get { return _baseMaxImpactDamage; } set { if(!CheckEdit()) return; _baseMaxImpactDamage = value; }}

		[ColumnName("Min Velocity Impact Speed")] [SerializeField] private float _minVelocityImpactSpeed = default;
		public float MinVelocityImpactSpeed { get { return _minVelocityImpactSpeed; } set { if(!CheckEdit()) return; _minVelocityImpactSpeed = value; }}

		[ColumnName("Min Velocity Impact Damage")] [SerializeField] private float _minVelocityImpactDamage = default;
		public float MinVelocityImpactDamage { get { return _minVelocityImpactDamage; } set { if(!CheckEdit()) return; _minVelocityImpactDamage = value; }}

		[ColumnName("Max Velocity Impact Damage")] [SerializeField] private float _maxVelocityImpactDamage = default;
		public float MaxVelocityImpactDamage { get { return _maxVelocityImpactDamage; } set { if(!CheckEdit()) return; _maxVelocityImpactDamage = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public EnvironmentModel model { get { return ModelManager.EnvironmentModel; } }
        private EnvironmentRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            EnvironmentRecord editableCopy = new EnvironmentRecord();
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

        private void CopyData(EnvironmentRecord record)
        {
            record._baseMinImpactDamage = _baseMinImpactDamage;
            record._baseMaxImpactDamage = _baseMaxImpactDamage;
            record._minVelocityImpactSpeed = _minVelocityImpactSpeed;
            record._minVelocityImpactDamage = _minVelocityImpactDamage;
            record._maxVelocityImpactDamage = _maxVelocityImpactDamage;
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
