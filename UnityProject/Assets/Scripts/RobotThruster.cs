using JetBrains.Annotations;
using Rewired;
using UnityEngine;

public class RobotThruster : MonoBehaviour
{
	[UsedImplicitly]
	public AnimationCurve verticalScaleCurve;

	[UsedImplicitly]
	public AnimationCurve alphaCurve;

    [UsedImplicitly]
    public float boostFactor;

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
		
		float y = player.GetAxis("MoveVertical");

		float scaleY = verticalScaleCurve.Evaluate(y);
		float alpha = alphaCurve.Evaluate(y);

	    if (m_playerController.trapState == PlayerController.TrapState.ClosedTrapped)
	    {
	        scaleY *= boostFactor;
	    }

        Vector3 localScale = m_originalLocalScale;
		localScale.y *= scaleY;
		transform.localScale = localScale;

		Color c = m_spriteRenderer.color;
		c.a = alpha;
		m_spriteRenderer.color = c;


	}
}
