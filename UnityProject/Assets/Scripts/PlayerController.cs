using JetBrains.Annotations;
using Rewired;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[UsedImplicitly]
	public int playerId;

	[UsedImplicitly]
	public Vector2 moveFactor;

	[UsedImplicitly]
	public float torqueFactor;

	[UsedImplicitly]
	public Rigidbody2D mainRigidbody;

	private Player m_player;
	private bool m_initialized;

	private void Initialize() 
	{
		m_player = ReInput.players.GetPlayer(playerId);
		m_initialized = true;
	}
	
	[UsedImplicitly]
	private void Update () 
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
	}
}
