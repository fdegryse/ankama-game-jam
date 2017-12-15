using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollWolf : MonoBehaviour
{
	[UsedImplicitly]
	public Collider2D headCollider;
	
	[UsedImplicitly]
	public Collider2D legCollider;

	[UsedImplicitly]
	public Collider2D tailCollider;
	
	[UsedImplicitly]
	public float washerActivationDelay = 1f;
	
	private readonly Dictionary<Collider2D, List<Joint2D>> m_activeTrapJoints = new Dictionary<Collider2D, List<Joint2D>>(2);
	private bool m_dead;
	
	public void AttachToTrap(Collider2D hit, Collider2D trapCollider)
	{
		Rigidbody2D hitRigidbody = hit.attachedRigidbody;

		var fixedJoint = hitRigidbody.gameObject.AddComponent<FixedJoint2D>();
		fixedJoint.connectedBody = trapCollider.attachedRigidbody;
		
		List<Joint2D> jointList;
		if (m_activeTrapJoints.TryGetValue(trapCollider, out jointList))
		{
			jointList.Add(fixedJoint);
		}
		else
		{
			jointList = new List<Joint2D>
			{
				fixedJoint
			};
			m_activeTrapJoints.Add(trapCollider, jointList);
		}
	}

	public void ReleaseFromTrap(Collider2D trapCollider)
	{
		List<Joint2D> jointList;
		if (m_activeTrapJoints.TryGetValue(trapCollider, out jointList))
		{
			foreach (Joint2D joint2D in jointList)
			{
				Destroy(joint2D);
			}

			jointList.Clear();
		}
	}

	public void PutInWasher(WolfWasher wolfWasher)
	{
		if (m_dead)
		{
			return;
		}
		m_dead = true;

		foreach (List<Joint2D> jointList in m_activeTrapJoints.Values)
		{
			foreach (Joint2D joint2D in jointList)
			{
				Destroy(joint2D);
			}
		}

		m_activeTrapJoints.Clear();

		wolfWasher.StartCoroutine(PutInWasherRoutine(wolfWasher));
	}

	private IEnumerator PutInWasherRoutine(WolfWasher wolfWasher)
	{
		yield return new WaitForSeconds(washerActivationDelay);
		
		Destroy(gameObject);

		wolfWasher.enabled = true;
	}
}