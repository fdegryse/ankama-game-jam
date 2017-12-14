using JetBrains.Annotations;
using Rewired;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[UsedImplicitly]
	public int playerId;

	[Header("Movement")]

	[UsedImplicitly]
	public Rigidbody2D mainRigidbody;

	[UsedImplicitly]
	public Transform body;

	[UsedImplicitly]
	public Vector2 moveFactor;

	[UsedImplicitly]
	public float torqueFactor;

	[UsedImplicitly]
	[Range(0f, 1f)]
	public float bodyStabilizationFactor;

	[Header("Trap")]

	[UsedImplicitly]
	public GameObject trapOpen;

	[UsedImplicitly]
	public GameObject trapClosed;

	[UsedImplicitly]
	public Collider2D trapCollider;

	[UsedImplicitly]
	[Range(0f, 10f)]
	public float trapCooldown;
	
	private Player m_player;
	public Player player
	{
		get { return m_player; }
	}

	private bool m_initialized;
	
	private enum TrapState
	{
		Open,
		Closed,
	}
	private TrapState m_trapState;
	private float m_trapCooldown;

	private void Initialize() 
	{
		m_player = ReInput.players.GetPlayer(playerId);
		m_initialized = true;

		SetTrapState(TrapState.Open);
	}

	private void SetTrapState(TrapState trapState)
	{
		switch (trapState)
		{
			case TrapState.Open:
				trapOpen.SetActive(true);
				trapClosed.SetActive(false);
				m_trapCooldown = 0f;
				break;
			case TrapState.Closed:
				trapOpen.SetActive(false);
				trapClosed.SetActive(true);
				m_trapCooldown = trapCooldown;
				break;
			default:
				throw new ArgumentOutOfRangeException("trapState", trapState, null);
		}
	}

	[UsedImplicitly]
	private void FixedUpdate () 
	{
		if (!ReInput.isReady)
		{
			return;
		}
		if (!m_initialized)
		{
			Initialize();
		}

		float x = m_player.GetAxis("MoveHorizontal");
		float y = m_player.GetAxis("MoveVertical");

		x *= moveFactor.x;
		y *= moveFactor.y;

		Vector2 moveVector = new Vector2(x, y);

		if (moveVector.sqrMagnitude > float.Epsilon)
		{
			Vector2 force = moveVector * Time.deltaTime;
			mainRigidbody.AddForce(force, ForceMode2D.Force);
		}

		float angle = Vector3.SignedAngle(Vector3.up, mainRigidbody.transform.up, Vector3.forward);
		if (Mathf.Abs(angle) > float.Epsilon)
		{
			float torque = -angle * torqueFactor * Time.deltaTime;
			mainRigidbody.AddTorque(torque, ForceMode2D.Force);
		}

		body.up = Vector3.Lerp(Vector3.up, mainRigidbody.transform.up, bodyStabilizationFactor);
	}

	[UsedImplicitly]
	private void Update()
	{
		if (m_trapCooldown <= float.Epsilon)
		{
			bool closeTrap = m_player.GetButtonDown("CloseTrap");
			if (closeTrap)
			{
				SetTrapState(TrapState.Closed);

				// TODO: check trap collider
			}
		}
		else
		{
			m_trapCooldown -= Time.deltaTime;
			if (m_trapCooldown <= float.Epsilon)
			{
				SetTrapState(TrapState.Open);
			}
		}
	}
}
