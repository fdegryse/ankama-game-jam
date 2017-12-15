using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Wolf : MonoBehaviour
{
	[UsedImplicitly]
	public SpriteSheetAnimation apparition;
	
	[UsedImplicitly]
	public SpriteSheetAnimation loop;

	[UsedImplicitly]
	public RagdollWolf ragdollWolf;

	[UsedImplicitly]
	private IEnumerator Start()
	{
		apparition.enabled = true;

		do
		{
			yield return null;
		}
		while (apparition.enabled);

		loop.enabled = true;

		do
		{
			yield return null;
		}
		while (loop.enabled);
	}

	public RagdollWolf GetTrapped()
	{
		gameObject.SetActive(false);

		GameObject copyGameObject = Instantiate(ragdollWolf.gameObject);
		copyGameObject.transform.position = ragdollWolf.transform.position;
		copyGameObject.transform.rotation = ragdollWolf.transform.rotation;
		
		return copyGameObject.GetComponent<RagdollWolf>();
	}
}
