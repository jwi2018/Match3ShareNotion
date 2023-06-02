using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TMPAnimator : MonoBehaviour
{
    public float charAnimDuration = 0.5f;
    public float charAnimOffset = 0.05f;
    public TMP_Text tmp;

    IEnumerator Start()
    {
        // Prepare the tween and leave it paused
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(tmp);
        Sequence sequence = DOTween.Sequence().Pause();
        for (int i = 0; i < animator.textInfo.characterCount; ++i) {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            Vector3 currCharOffset = animator.GetCharOffset(i);
            float timeOffset = i * charAnimOffset;
            sequence
                .Insert(timeOffset, animator.DOFadeChar(i, 0, charAnimDuration).From())
                .Insert(timeOffset, animator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), charAnimDuration))
                .Insert(timeOffset, animator.DORotateChar(i, new Vector3(0, 0, -60), charAnimDuration).From());
        }

        // Wait the usual editor initialization time
        yield return new WaitForSeconds(0.8f);

        // Play
        sequence.Play();
    }
}