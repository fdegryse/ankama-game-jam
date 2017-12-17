using JetBrains.Annotations;
using Rewired;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[UsedImplicitly] public int playerId;

	[UsedImplicitly] public PlayerScore scoreUI;

	[UsedImplicitly] public GameEndUI gameEndUI;

	[Header("Movement")]

	[UsedImplicitly] public Rigidbody2D mainRigidbody;

	[UsedImplicitly] public Transform body;

	[UsedImplicitly] public Vector2 moveFactor;

	[UsedImplicitly] public Vector2 trapThrusterBoost;

	[UsedImplicitly] public float torqueFactor;

	[UsedImplicitly] [Range(0f, 1f)] public float bodyStabilizationFactor;

	[Header("Trap")]

	[UsedImplicitly] public GameObject trapOpen;

	[UsedImplicitly] public GameObject trapClosed;

	[UsedImplicitly] public Collider2D trapCollider;

	[UsedImplicitly] [Range(0f, 10f)] public float trapCooldown;

	[UsedImplicitly] public SpriteSheetAnimation openAnimation;

	[UsedImplicitly] public SpriteSheetAnimation closeAnimation;

	private Player m_player;
	public Player player
	{
		get { return m_player; }
	}

	private bool m_initialized;
	private int m_score;

	public enum TrapState
	{
		Open,
		Closed,
		ClosedTrapped
	}

	private TrapState m_trapState;
	public TrapState trapState
	{
		get { return m_trapState; }
	}

	private float m_trapCooldown;
	private RagdollWolf m_trappedWolf;

	private readonly Collider2D[] m_trapHitBuffer = new Collider2D[32];

	private void Initialize()
	{
		int playerControllerIndex = PlayerAssignation.GetPlayerControllerIndex(playerId);

		m_player = ReInput.players.GetPlayer(playerControllerIndex);
		m_initialized = true;

		SetTrapState(TrapState.Open);
	}

	private void SetTrapState(TrapState value)
	{
		switch (value)
		{
			case TrapState.Open:
				trapOpen.SetActive(true);
				trapClosed.SetActive(false);
				m_trapCooldown = 0f;
				openAnimation.enabled = true;
				break;
			case TrapState.Closed:
				trapOpen.SetActive(false);
				trapClosed.SetActive(true);
				trapClosed.layer = LayerMask.NameToLayer("Robots");
				m_trapCooldown = trapCooldown;
				closeAnimation.enabled = true;
				break;
			case TrapState.ClosedTrapped:
				trapOpen.SetActive(false);
				trapClosed.SetActive(true);
				trapClosed.layer = LayerMask.NameToLayer("Blocker");
				closeAnimation.enabled = true;
				break;
			default:
				throw new ArgumentOutOfRangeException("value", value, null);
		}

		m_trapState = value;
	}

	[UsedImplicitly]
	private void FixedUpdate()
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

		if (m_trapState == TrapState.ClosedTrapped)
		{
			x *= trapThrusterBoost.x;
			y *= trapThrusterBoost.y;
		}

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
		switch (m_trapState)
		{
			case TrapState.Open:
			{
				bool closeTrap = m_player.GetButtonDown("CloseTrap");
				if (closeTrap)
				{
					RagdollWolf ragdollWolf;
					Collider2D ragdollWolfCollider;
					if (TryCatchWolf(out ragdollWolf, out ragdollWolfCollider))
					{
						if (ragdollWolf.AttachToTrap(this, ragdollWolfCollider))
						{
							m_trappedWolf = ragdollWolf;
							SetTrapState(TrapState.ClosedTrapped);
						}
					}
					else
					{
						SetTrapState(TrapState.Closed);
					}
				}
			}
			break;

			case TrapState.Closed:
			{
				m_trapCooldown -= Time.deltaTime;
				if (m_trapCooldown <= float.Epsilon)
				{
					bool holdTrap = m_player.GetButton("CloseTrap");
					if (!holdTrap)
					{
						SetTrapState(TrapState.Open);
					}
				}
			}
			break;

			case TrapState.ClosedTrapped:
			{
				m_trapCooldown -= Time.deltaTime;
				if (m_trapCooldown <= float.Epsilon)
				{
					bool holdTrap = m_player.GetButton("CloseTrap");
					if (!holdTrap)
					{
						ReleaseTrappedWolf();
					}
				}
			}
			break;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void ReleaseTrappedWolf()
	{
		if (null != m_trappedWolf)
		{
			m_trappedWolf.ReleaseFromTrap(this);
			m_trappedWolf = null;
		}

		SetTrapState(TrapState.Open);
	}

	public void IncrementScore()
	{
		if (gameEndUI.gameObject.activeSelf)
		{
			return;
		}

		++m_score;
		scoreUI.SetScore(m_score);
		gameEndUI.SetPlayerScore(playerId, m_score);
	}

	private bool TryCatchWolf(out RagdollWolf caughtRagdollWolf, out Collider2D hitCollider)
	{
		var contactFilter2D = new ContactFilter2D
		{
			useTriggers = true,
			useLayerMask = true,
			layerMask = LayerMask.GetMask("Wolves", "WolvesPart")
		};

		int trapHitCount = trapCollider.OverlapCollider(contactFilter2D, m_trapHitBuffer);

		// Find a hiding wolf

		for (int i = 0; i < trapHitCount; ++i)
		{
			Collider2D hit = m_trapHitBuffer[i];

			Rigidbody2D hitRigidbody = hit.attachedRigidbody;
			if (null != hitRigidbody)
			{
				continue;
			}
			
			var wolf = hit.gameObject.GetComponent<Wolf>();
			if (null == wolf)
			{
				continue;
			}
			
			RagdollWolf ragdollWolf = wolf.GetTrapped();

			Collider2D wolfCollider;
			switch (wolf.wolfPart)
			{
				case Wolf.WolfPart.Head:
					wolfCollider = ragdollWolf.headCollider;
					break;
				case Wolf.WolfPart.Leg:
					wolfCollider = ragdollWolf.legCollider;
					break;
				case Wolf.WolfPart.Tail:
					wolfCollider = ragdollWolf.tailCollider;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			caughtRagdollWolf = ragdollWolf;
			hitCollider = wolfCollider;
			return true;
		}

		// Find a ragdoll wolf

		RagdollWolf closestHitWolf = null;
		Collider2D closestHitCollider = null;
		float closestHitDistance = float.MaxValue;

		for (int i = 0; i < trapHitCount; ++i)
		{
			Collider2D hit = m_trapHitBuffer[i];

			Rigidbody2D hitRigidbody = hit.attachedRigidbody;
			if (null == hitRigidbody)
			{
				continue;
			}
			
			var hitWolfPart = hitRigidbody.GetComponent<RagdollWolfPart>();
			if (null == hitWolfPart)
			{
				continue;
			}

			RagdollWolf hitRagdollWolf = hitWolfPart.ragdollWolf;
			if (hitRagdollWolf.isDead)
			{
				continue;
			}

			ColliderDistance2D colliderDistance = hit.Distance(trapCollider);
			if (!colliderDistance.isValid)
			{
				continue;
			}

			float distance = colliderDistance.distance;
			if (distance >= closestHitDistance)
			{
				continue;
			}

			closestHitWolf = hitRagdollWolf;
			closestHitCollider = hit;
			closestHitDistance = distance;
		}

		if (null != closestHitCollider)
		{
			caughtRagdollWolf = closestHitWolf;
			hitCollider = closestHitCollider;
			return true;
		}

		caughtRagdollWolf = null;
		hitCollider = null;
		return false;
	}
}