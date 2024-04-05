using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class PlayerWeaponRecord : BaseRecord<PlayerWeaponIdentifier>
	{
		[ColumnName("Projectile Count")] [SerializeField] private int _projectileCount = default;
		public int ProjectileCount { get { return _projectileCount; } set { if(!CheckEdit()) return; _projectileCount = value; }}

		[ColumnName("Spread")] [SerializeField] private float _spread = default;
		public float Spread { get { return _spread; } set { if(!CheckEdit()) return; _spread = value; }}

		[ColumnName("Angle")] [SerializeField] private float _angle = default;
		public float Angle { get { return _angle; } set { if(!CheckEdit()) return; _angle = value; }}

		[ColumnName("Cooldown")] [SerializeField] private float _cooldown = default;
		public float Cooldown { get { return _cooldown; } set { if(!CheckEdit()) return; _cooldown = value; }}

		[ColumnName("Energy Cost")] [SerializeField] private int _energyCost = default;
		public int EnergyCost { get { return _energyCost; } set { if(!CheckEdit()) return; _energyCost = value; }}

		[ColumnName("Projectile")] [SerializeField] private ProjectileIdentifier _projectile = default;
		[NonSerialized] private ProjectileRecord _projectileRecord = default;
		public ProjectileRecord Projectile 
		{ 
			get 
			{ 
				if(_projectileRecord == null)
					_projectileRecord = ModelManager.ProjectileModel.GetRecord(_projectile);
				return _projectileRecord; 
			} 
			set
			{
				if(!CheckEdit())
					return;
					
                if (value == null)
                    _projectile = ProjectileIdentifier.None;
                else
                    _projectile = value.Identifier;
				_projectileRecord = null;
			}
		}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public PlayerWeaponModel model { get { return ModelManager.PlayerWeaponModel; } }
        private PlayerWeaponRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            PlayerWeaponRecord editableCopy = new PlayerWeaponRecord();
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

        private void CopyData(PlayerWeaponRecord record)
        {
            record._projectileCount = _projectileCount;
            record._spread = _spread;
            record._angle = _angle;
            record._cooldown = _cooldown;
            record._energyCost = _energyCost;
            record._projectile = _projectile;
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
