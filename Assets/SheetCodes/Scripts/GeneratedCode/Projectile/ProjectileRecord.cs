using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SheetCodes
{
	//Generated code, do not edit!

	[Serializable]
	public class ProjectileRecord : BaseRecord<ProjectileIdentifier>
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

		[ColumnName("Min Damage")] [SerializeField] private int _minDamage = default;
		public int MinDamage { get { return _minDamage; } set { if(!CheckEdit()) return; _minDamage = value; }}

		[ColumnName("Max Damage")] [SerializeField] private int _maxDamage = default;
		public int MaxDamage { get { return _maxDamage; } set { if(!CheckEdit()) return; _maxDamage = value; }}

		//Does this type no longer exist? Delete from here..
		[ColumnName("Damage Type")] [SerializeField] private DamageType _damageType = default;
		public DamageType DamageType { get { return _damageType; } set { if(!CheckEdit()) return; _damageType = value; }}
		//..To here

		[ColumnName("Life Distance")] [SerializeField] private float _lifeDistance = default;
		public float LifeDistance { get { return _lifeDistance; } set { if(!CheckEdit()) return; _lifeDistance = value; }}

		[ColumnName("Life Time")] [SerializeField] private float _lifeTime = default;
		public float LifeTime { get { return _lifeTime; } set { if(!CheckEdit()) return; _lifeTime = value; }}

		[ColumnName("Speed")] [SerializeField] private float _speed = default;
		public float Speed { get { return _speed; } set { if(!CheckEdit()) return; _speed = value; }}

        protected bool runtimeEditingEnabled { get { return originalRecord != null; } }
        public ProjectileModel model { get { return ModelManager.ProjectileModel; } }
        private ProjectileRecord originalRecord = default;

        public override void CreateEditableCopy()
        {
#if UNITY_EDITOR
            if (runtimeEditingEnabled)
                return;

            ProjectileRecord editableCopy = new ProjectileRecord();
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

        private void CopyData(ProjectileRecord record)
        {
            record._prefab = _prefab;
            record._minDamage = _minDamage;
            record._maxDamage = _maxDamage;
            record._damageType = _damageType;
            record._lifeDistance = _lifeDistance;
            record._lifeTime = _lifeTime;
            record._speed = _speed;
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
