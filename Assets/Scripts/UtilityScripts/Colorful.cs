using UnityEngine;

namespace PaziUtils
{
	public static class Colorful
	{
		public static Color orange = new(1f, .45f, 0f, 1f);
		public static Color lime = new(1f, .54f, 0f, 1f);
		public static Color moss = new(0f, .5f, .27f, 1f);
		public static Color turquoise = new(0f, .1f, .52f, 1f);
		public static Color teal = new(0f, .38f, .38f, 1f);
		public static Color skyblue = new(0f, .69f, 1f, 1f);
		public static Color violet = new(.53f, .29f, 1f, 1f);
		public static Color purple = new(.58f, .16f, 1f, 1f);
		public static Color pink = new(1f, .5f, .8f, 1f);
		public static Color blood = new(.5f, 0f, 0f, 1f);
		public static Color brown = new(.38f, .14f, 0f, 1f);

		public static Color Random()
		{
			Color randomColor = new(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
			return randomColor;
		}

		public static Color Random(float min, float max)
		{
			Color randomColor = new(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), 1f);
			return randomColor;
		}

		public static Color RandomGray(float min, float max)
		{
			float randomValue = UnityEngine.Random.Range(min, max);
			Color randomColor = new(randomValue, randomValue, randomValue, 1f);
			return randomColor;
		}

		public static Color Random(float minRed, float maxRed, float minGreen, float maxGreen, float minBlue, float maxBlue)
		{
			Color randomColor = new(UnityEngine.Random.Range(minRed, maxRed), UnityEngine.Random.Range(minGreen, maxGreen), UnityEngine.Random.Range(minBlue, maxBlue), 1f);
			return randomColor;
		}

		public static Color RandomHSV(float minHue, float maxHue, float minSaturation, float maxSaturation, float minValue, float maxValue)
		{
			Color randomColor = Color.HSVToRGB(UnityEngine.Random.Range(minHue, maxHue), UnityEngine.Random.Range(minSaturation, maxSaturation), UnityEngine.Random.Range(minValue, maxValue));
			return randomColor;
		}
	}

}
