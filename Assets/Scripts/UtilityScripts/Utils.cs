using UnityEngine;

namespace PaziUtils
{
	public static class Utils
	{
		public static float Vector3InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
		}

	}

}
