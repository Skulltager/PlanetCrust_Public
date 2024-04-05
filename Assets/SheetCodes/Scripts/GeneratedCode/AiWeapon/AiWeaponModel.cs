using System;
using UnityEngine;
namespace SheetCodes
{
	//Generated code, this script is only generated once and will not be overwritted!
	//You can add code to this script to iterate your records but don't remove the generated code!

	[Serializable]
	public class AiWeaponModel : BaseModel<AiWeaponRecord, AiWeaponIdentifier>
	{
		[SerializeField] private AiWeaponRecord[] records = default;
		protected override AiWeaponRecord[] Records { get { return records; } }

		//Add your code below this line
	}
}
