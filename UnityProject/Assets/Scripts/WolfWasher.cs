using JetBrains.Annotations;
using UnityEngine;

public class WolfWasher : MonoBehaviour
{
	[UsedImplicitly] public PlayerController associatedPlayer;

	[UsedImplicitly] public int frameRate = 15;

	[UsedImplicitly] public int fullAnimationFrameCount;

	[UsedImplicitly] public int loopCount;

	[UsedImplicitly] public SpriteSheetAnimation[] spriteSheetAnimations;

	[UsedImplicitly] public SpriteSheetAnimation bloodSpriteSheetAnimation;

	[UsedImplicitly] public SpriteSheetAnimation trapSpriteSheetAnimation;

	[UsedImplicitly][Range(0f, 1f)] public float bloodRandomFactor;

	[UsedImplicitly] public float bloodPositionRandomRange;

	[UsedImplicitly] public float bloodAngleRandomRange;

	[UsedImplicitly] public ParticleSystem furParticleSystem;
	
	private int m_currentFrame;
	private int m_loopCount;
	private float m_timer;

	[UsedImplicitly]
	private void OnEnable()
	{
		foreach (SpriteSheetAnimation spriteSheetAnimation in spriteSheetAnimations)
		{
			spriteSheetAnimation.enabled = true;
		}

		m_currentFrame = 0;
		m_loopCount = 0;
		m_timer = 0f;

		furParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		furParticleSystem.Play(true);
	}

	[UsedImplicitly]
	private void OnDisable()
	{
		foreach (SpriteSheetAnimation spriteSheetAnimation in spriteSheetAnimations)
		{
			spriteSheetAnimation.enabled = false;
		}

		trapSpriteSheetAnimation.enabled = true;
	}

	[UsedImplicitly]
	private void Update()
	{
		m_timer += Time.deltaTime;

		float frameTime = 1f / frameRate;

		int currentFrame = Mathf.FloorToInt(m_timer / frameTime);
		if (m_currentFrame != currentFrame)
		{
			if (currentFrame >= fullAnimationFrameCount)
			{
				m_timer -= currentFrame * frameTime;
				currentFrame = currentFrame % fullAnimationFrameCount;

				++m_loopCount;
				if (m_loopCount >= loopCount)
				{
					trapSpriteSheetAnimation.enabled = true;
					enabled = false;
				}
			}
			m_currentFrame = currentFrame;
			
			if (!bloodSpriteSheetAnimation.enabled)
			{
				if (Random.value < bloodRandomFactor)
				{
					Transform bloodTransform = bloodSpriteSheetAnimation.transform;

					float bloodXPosition = Random.Range(-bloodPositionRandomRange, bloodPositionRandomRange);
					Vector3 bloodPosition = bloodTransform.localPosition;
					bloodPosition.x = bloodXPosition;
					bloodTransform.localPosition = bloodPosition;

					float bloodAngle = Random.Range(-bloodAngleRandomRange, bloodAngleRandomRange);
					Vector3 bloodRotation = bloodTransform.localEulerAngles;
					bloodRotation.z = bloodAngle;
					bloodTransform.localEulerAngles = bloodRotation;

					bloodSpriteSheetAnimation.enabled = true;
				}
			}
		}
	}
}
