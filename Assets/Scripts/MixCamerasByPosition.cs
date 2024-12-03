using UnityEngine;
using Unity.Cinemachine;
using PaziUtils;

[RequireComponent(typeof(CinemachineMixingCamera))]
public class MixCamerasByPosition : MonoBehaviour
{
	public float Weight;
	public Transform targetTransform;
	public Vector3 LowPosition;
	public Vector3 HighPosition;
	public AnimationCurve WeightCurve;
	CinemachineMixingCamera m_Mixer;
	void Start()
	{
		m_Mixer = GetComponent<CinemachineMixingCamera>();
	}

	void OnValidate()
	{
		//Weight = Mathf.Max(1, MaxSpeed);
	}

	void Update()
	{
		float t = Utils.Vector3InverseLerp(LowPosition, HighPosition, targetTransform.position);
		Weight = WeightCurve.Evaluate(t);
		m_Mixer.Weight0 = 1 - Weight;
		m_Mixer.Weight1 = Weight;
		transform.ResetTransformation();
	}
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(LowPosition, .25f);
		Gizmos.DrawSphere(HighPosition, .25f);
	}
}

