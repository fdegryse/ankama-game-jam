using JetBrains.Annotations;
using Rewired;
using UnityEngine;

public class RobotMiniThruster : MonoBehaviour
{
	[UsedImplicitly]
	public AnimationCurve verticalScaleCurve;

	[UsedImplicitly]
	public AnimationCurve alphaCurve;

	[UsedImplicitly]
	public bool reverseX;

	private SpriteRenderer m_spriteRenderer;
	private PlayerController m_playerController;
	private Vector3 m_originalLocalScale;

	[UsedImplicitly]
	private void Awake()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_playerController = GetComponentInParent<PlayerController>();

		m_originalLocalScale = transform.localScale;
	}

	[UsedImplicitly]
	private void Update ()
	{
		Player player = m_playerController.player;
		
		float x = player.GetAxis("MoveHorizontal");
		float y = player.GetAxis("MoveVertical");
		float f = Mathf.Max(0f, reverseX ? -x : x) + y;

		float scaleY = verticalScaleCurve.Evaluate(f);
		float alpha = alphaCurve.Evaluate(f);

		Vector3 localScale = m_originalLocalScale;
		localScale.y *= scaleY;
		transform.localScale = localScale;

		Color c = m_spriteRenderer.color;
		c.a = alpha;
		m_spriteRenderer.color = c;


	}
}
