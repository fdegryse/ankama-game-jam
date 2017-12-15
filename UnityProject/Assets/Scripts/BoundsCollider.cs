using JetBrains.Annotations;
using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(EdgeCollider2D))]
public class BoundsCollider : MonoBehaviour
{
	public float bottomOffset;

	[UsedImplicitly]
	private void Awake()
	{
		var edgeCollider = GetComponent<EdgeCollider2D>();
		
		Camera cam = Camera.main;
		float orthoSize = cam.orthographicSize;
		float ratio = cam.aspect;
		float edge = edgeCollider.edgeRadius;

		float x = orthoSize * ratio;

		Vector2[] points =
		{
			new Vector2(-x - edge, -orthoSize - edge + bottomOffset),
			new Vector2( x + edge, -orthoSize - edge + bottomOffset),
			new Vector2( x + edge,  orthoSize + edge),
			new Vector2(-x - edge,  orthoSize + edge),
			new Vector2(-x - edge, -orthoSize - edge + bottomOffset),
		};
		edgeCollider.points = points;

		#if UNITY_EDITOR
		m_previousAspect = ratio;
		m_previousOrthoSize = orthoSize;
		#endif
	}

#if UNITY_EDITOR

	private float m_previousAspect;
	private float m_previousOrthoSize;

	[UsedImplicitly]
	private void Update()
	{
		Camera cam = Camera.main;
		if (Math.Abs(m_previousOrthoSize - cam.orthographicSize) > float.Epsilon || Math.Abs(m_previousAspect - cam.aspect) > float.Epsilon)
		{
			Awake();
		}
	}

	[UsedImplicitly]
	private void OnValidate()
	{
		Awake();
	}

#endif
}
