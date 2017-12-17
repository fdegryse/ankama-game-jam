using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class RagdollWolfPart : MonoBehaviour
{
	[UsedImplicitly]
	public RagdollWolf ragdollWolf;

	private Coroutine m_effectorRoutine;

	[UsedImplicitly]
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.usedByEffector)
		{
			m_effectorRoutine = StartCoroutine(EffectorRoutine());
			return;
		}
		
		var wolfWasher = other.gameObject.GetComponent<WolfWasher>();
		if (null == wolfWasher)
		{
			return;
		}

		if (null != m_effectorRoutine)
		{
			StopCoroutine(m_effectorRoutine);
			m_effectorRoutine = null;
		}

		ragdollWolf.PutInWasher(wolfWasher);
	}

	private IEnumerator EffectorRoutine()
	{
		yield return new WaitForSeconds(2f);

		ragdollWolf.RemoveHingeJointsLimits();
	}
}
