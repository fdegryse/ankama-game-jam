using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [UsedImplicitly] public Sprite activeSprite;

    [UsedImplicitly] public Image[] images;

    public void SetScore(int score)
    {
        if (score > 10)
        {
            return;
        }
        images[score - 1].sprite = activeSprite;
    }
}
