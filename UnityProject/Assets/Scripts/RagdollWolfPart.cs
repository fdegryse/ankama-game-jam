using JetBrains.Annotations;
using UnityEngine;

public class RagdollWolfPart : MonoBehaviour
{
	[UsedImplicitly]
	public RagdollWolf ragdollWolf;

	[UsedImplicitly]
	private void OnTriggerEnter2D(Collider2D other)
	{
		var wolfWasher = other.gameObject.GetComponent<WolfWasher>();
		if (null == wolfWasher)
		{
			return;
		}

		ragdollWolf.PutInWasher(wolfWasher);
	}
}
