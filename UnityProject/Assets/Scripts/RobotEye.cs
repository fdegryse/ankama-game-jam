using JetBrains.Annotations;
using Rewired;
using UnityEngine;

public class RobotEye : MonoBehaviour
{
	[UsedImplicitly]
	public AnimationCurve horizontalMovementCurve;

	[UsedImplicitly]
	public AnimationCurve horizontalScaleCurve;

	[UsedImplicitly]
	public AnimationCurve verticalMovementCurve;

	private PlayerController m_playerController;
	private Vector3 m_originalLocalPosition;

	[UsedImplicitly]
	private void Awake()
	{
		m_playerController = GetComponentInParent<PlayerController>();
		m_originalLocalPosition = transform.localPosition;
	}

	[UsedImplicitly]
	private void Update ()
	{
		Player player = m_playerController.player;

		float x = player.GetAxis("MoveHorizontal");
		float y = player.GetAxis("MoveVertical");

		float localX = horizontalMovementCurve.Evaluate(x);
		float scaleX = horizontalScaleCurve.Evaluate(x);
		float localY = verticalMovementCurve.Evaluate(y);

		Vector3 deltaPosition = new Vector3(localX, localY, 0f);
		Vector3 localScale = new Vector3(scaleX, 1f, 1f);
		transform.localPosition = m_originalLocalPosition + deltaPosition;
		transform.localScale = localScale;
	}
}
