using JetBrains.Annotations;
using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[UsedImplicitly] public int playerId;

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

	private Player m_player;

	public Player player
	{
		get { return m_player; }
	}

	private bool m_initialized;

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
	private readonly HashSet<RagdollWolf> m_trappedWolves = new HashSet<RagdollWolf>();
	private readonly Collider2D[] m_trapHitBuffer = new Collider2D[8];
	private readonly RaycastHit2D[] m_trapHitRaycastBuffer = new RaycastHit2D[8];

	private void Initialize()
	{
		m_player = ReInput.players.GetPlayer(playerId);
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
				foreach (RagdollWolf wolf in m_trappedWolves)
				{
					if (null != wolf)
					{
						wolf.ReleaseFromTrap(trapCollider);
					}
				}
				m_trappedWolves.Clear();
				break;
			case TrapState.Closed:
				trapOpen.SetActive(false);
				trapClosed.SetActive(true);
				trapClosed.layer = LayerMask.NameToLayer("Robots");
				m_trapCooldown = trapCooldown;
				break;
			case TrapState.ClosedTrapped:
				trapOpen.SetActive(false);
				trapClosed.SetActive(true);
				trapClosed.layer = LayerMask.NameToLayer("Blocker");
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
					ContactFilter2D contactFilter2D = new ContactFilter2D
					{
						useTriggers = true
					};

					trapCollider.enabled = true;
					int trapHitCount = trapCollider.OverlapCollider(contactFilter2D, m_trapHitBuffer);
					int raycastHitCount = trapCollider.Raycast(-trapCollider.transform.up, contactFilter2D, m_trapHitRaycastBuffer);
					trapCollider.enabled = false;

					RagdollWolf closestHitWolf = null;
					Collider2D closestHitCollider = null;
					float closestHitDistance = float.MaxValue;

					for (int i = 0; i < trapHitCount; ++i)
					{
						Collider2D hit = m_trapHitBuffer[i];
						
						Rigidbody2D hitRigidbody = hit.attachedRigidbody;
						if (null == hitRigidbody)
						{
							var wolf = hit.gameObject.GetComponent<Wolf>();

							RagdollWolf ragdollWolf = wolf.GetTrapped();
							m_trappedWolves.Add(ragdollWolf);

							SetTrapState(TrapState.ClosedTrapped);

							ragdollWolf.AttachToTrap(ragdollWolf.headCollider, trapCollider);

							ragdollWolf.gameObject.SetActive(true);
						}
						else
						{
							var hitWolfPart = hitRigidbody.GetComponent<RagdollWolfPart>();
							if (null != hitWolfPart)
							{
								for (int j = 0; j < raycastHitCount; ++j)
								{
									RaycastHit2D raycastHit = m_trapHitRaycastBuffer[j];
									if (raycastHit.collider == hit)
									{
										float raycastHitDistance = raycastHit.distance;
										if (raycastHitDistance < closestHitDistance)
										{
											closestHitWolf = hitWolfPart.ragdollWolf;
											closestHitCollider = hit;
											closestHitDistance = raycastHitDistance;
										}
										break;
									}
								}
							}
						}
					}

					if (null != closestHitCollider)
					{
						SetTrapState(TrapState.ClosedTrapped);
						m_trappedWolves.Add(closestHitWolf);

						closestHitWolf.AttachToTrap(closestHitCollider, trapCollider);
					}

					if (m_trapState == TrapState.Open)
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
						SetTrapState(TrapState.Open);
					}
				}
			}
			break;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}