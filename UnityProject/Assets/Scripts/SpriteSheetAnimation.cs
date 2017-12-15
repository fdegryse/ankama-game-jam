using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSheetAnimation : MonoBehaviour
{
	[UsedImplicitly]
	public Sprite[] m_sprites;

	[UsedImplicitly]
	public int frameRate = 25;

	[UsedImplicitly]
	public bool loop = true;

	private SpriteRenderer m_spriteRenderer;

	private int m_currentFrame;
	private float m_timer;

	[UsedImplicitly]
	private void Awake()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	[UsedImplicitly]
	private void OnEnable()
	{
		m_currentFrame = 0;
		m_timer = 0f;

		m_spriteRenderer.enabled = true;
		m_spriteRenderer.sprite = m_sprites[0];
	}

	[UsedImplicitly]
	private void OnDisable()
	{
		if (loop)
		{
			m_spriteRenderer.sprite = m_sprites[0];
		}
		else
		{
			m_spriteRenderer.enabled = false;
		}
	}
	
	[UsedImplicitly]
	private void Update ()
	{
		m_timer += Time.deltaTime;

		float frameTime = 1f / frameRate;

		int currentFrame = Mathf.FloorToInt(m_timer / frameTime);
		if (m_currentFrame != currentFrame)
		{
			int frameCount = m_sprites.Length;
			if (currentFrame >= frameCount)
			{
				currentFrame = currentFrame % frameCount;

				if (!loop)
				{
					enabled = false;
				}
			}
			
			m_spriteRenderer.sprite = m_sprites[currentFrame];
			m_currentFrame = currentFrame;
		}
	}
}
