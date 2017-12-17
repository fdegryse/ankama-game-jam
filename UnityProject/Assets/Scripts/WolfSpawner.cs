using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
	[UsedImplicitly] public GameObject startGameUI;

	[UsedImplicitly] public float delayBetweenSpawns = 2f;

	private Wolf[] m_wolves;
	
	[UsedImplicitly]
	private void Awake()
	{
		m_wolves = GetComponentsInChildren<Wolf>();
		foreach (Wolf wolf in m_wolves)
		{
			wolf.gameObject.SetActive(false);
		}
	}

	[UsedImplicitly]
	private IEnumerator Start()
	{
		startGameUI.SetActive(true);
		
		yield return new WaitForSecondsRealtime(3f);

		startGameUI.SetActive(false);

		do
		{
			int wolfCount = m_wolves.Length;
			int random = Random.Range(0, wolfCount);

			Wolf wolf = m_wolves[random];
			wolf.gameObject.SetActive(true);

			do
			{
				yield return null;
			}
			while (wolf.gameObject.activeSelf);

			if (delayBetweenSpawns > 0f)
			{
				yield return new WaitForSeconds(delayBetweenSpawns);
			}
		}
		while (enabled);
	}
}
