using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollWolf : MonoBehaviour
{
	[UsedImplicitly]
	public float washerActivationDelay = 1f;

	private readonly Dictionary<Collider2D, List<Joint2D>> m_activeTrapJoints = new Dictionary<Collider2D, List<Joint2D>>(2);

	public bool AttachToTrap(Collider2D hit, Collider2D trapCollider)
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

		return true;
	}

	public bool ReleaseFromTrap(Collider2D trapCollider)
	{
		List<Joint2D> jointList;
		if (m_activeTrapJoints.TryGetValue(trapCollider, out jointList))
		{
			foreach (Joint2D joint2D in jointList)
			{
				Destroy(joint2D);
			}

			jointList.Clear();

			return true;
		}

		return false;
	}

	public void PutInWasher(WolfWasher wolfWasher)
	{
		StartCoroutine(PutInWasherRoutine(wolfWasher));
	}

	private IEnumerator PutInWasherRoutine(WolfWasher wolfWasher)
	{
		yield return new WaitForSeconds(washerActivationDelay);

		wolfWasher.enabled = true;

		do
		{
			yield return null;
		}
		while (wolfWasher.enabled);

		Destroy(gameObject);
	}
}