using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Wolf : MonoBehaviour
{
	public enum WolfPart
	{
		Head,
		Leg,
		Tail
	}

	[UsedImplicitly]
	public RagdollWolf ragdollWolf;

	[UsedImplicitly]
	public ParticleSystem leavesParticleSystem;

	[UsedImplicitly]
	public SpriteSheetAnimation apparition;
	
	[UsedImplicitly]
	public SpriteSheetAnimation loop;

	[UsedImplicitly]
	public WolfPart wolfPart;

	[UsedImplicitly]
	private void Awake()
	{
		leavesParticleSystem.transform.SetParent(transform.parent, true);
		leavesParticleSystem.transform.up = Vector3.up;
	}

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

		leavesParticleSystem.gameObject.SetActive(true);
		leavesParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		leavesParticleSystem.Play(true);

		GameObject copyGameObject = Instantiate(ragdollWolf.gameObject);
		copyGameObject.transform.position = ragdollWolf.transform.position;
		copyGameObject.transform.rotation = ragdollWolf.transform.rotation;
		
		copyGameObject.SetActive(true);

		return copyGameObject.GetComponent<RagdollWolf>();
	}
}
