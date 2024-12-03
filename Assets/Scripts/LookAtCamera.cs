using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	Transform mainCameraTransform;
	[SerializeField][Range(0f, 1f)] float worldUpToCameraUp;
	void Start()
	{
		mainCameraTransform = Camera.main.transform;
	}

	void Update()
	{
		if (worldUpToCameraUp <= Mathf.Epsilon)
			transform.LookAt(mainCameraTransform.position, Vector3.up);
		else if (worldUpToCameraUp >= 1f - Mathf.Epsilon)
			transform.LookAt(mainCameraTransform.position, mainCameraTransform.up);
		else
			transform.LookAt(mainCameraTransform.position,
			Vector3.Lerp(Vector3.up, mainCameraTransform.up, worldUpToCameraUp));
	}
}
