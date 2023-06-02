using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cell
{
    public Sprite[] frames;
}

public class SpriteAnimationCustom : MonoBehaviour
{
    public List<Cell> frames;
    public bool loop = false;
    public float frameDelay = 0.13f;
    private Image imageRend;
    private SpriteRenderer spriteRend;
    private int currentFrame;
    public bool playing = false;
    public bool stopped = false; // flag set to interrupt animation; restart from beginning
    public bool paused = false;
    public int cachedFrame = 0; // save the index of the paused frame. May be used in a "Resume" function
    public bool playOnStart = false;
    public bool disableRendererOnFinish = false; // disable the sprite renderer when the animation completes
    private Coroutine animCoroutine;
    public int idxFrame = 0;
    public bool isYoyo = false;
    private bool isDown = true;

    public void Awake()
    {
        if (GetComponent<Image>() != null)
        {
            imageRend = GetComponent<Image>();
        }

        if (GetComponent<SpriteRenderer>())
        {
            spriteRend = GetComponent<SpriteRenderer>();
        }

        if (imageRend == null && spriteRend == null)
            Debug.LogError("SpriteRenderer missing!");
        else
        {
            if (playOnStart)
                Play();
        }
    }

    public void Play()
    {
        // should probably do a StopCoroutine here, just to be safe
        if (!playing)
        {
            stopped = false;
            paused = false;
            animCoroutine = StartCoroutine(PlayAnim());
            //StartCoroutine(PlayAnim());
        }
    }

    public void Pause()
    {
        if (playing)
        {
            paused = true;
            playing = false;
        }
    }


    //public void Resume()
    //{
    //    if (!playing)
    //    {
    //        paused = false;
    //       // StartCoroutine(PlayAnim(cachedFrame));
    //    }
    //}

    public void Stop()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        stopped = true;
        paused = false;
        playing = false;
    }

    private IEnumerator PlayAnim()
    {
        int initialFrame = 0;
        int direction = 1;
        if (!playing)
        {
            playing = true;
            //spriteRend.enabled = true; // make sure sprite renderer is enabled

            currentFrame = 0;

            while (!stopped && !paused)
            {
                yield return new WaitForSeconds(frameDelay);

                if (!paused && !stopped)
                {
                    if (imageRend != null)
                    {
                        imageRend.overrideSprite = frames[idxFrame].frames[currentFrame];
                    }

                    if (spriteRend != null)
                    {
                        spriteRend.sprite = frames[idxFrame].frames[currentFrame];
                    }
                }
                else
                    yield break;

                if (isYoyo == true)
                {
                    if (isDown == true)
                    {
                        if (currentFrame >= frames[idxFrame].frames.Length - 1)
                        {
                            isDown = false;

                            if (!loop)
                                Stop();
                        }
                        else
                        {
                            currentFrame += 1;
                        }
                    }
                    else
                    {
                        if (currentFrame <= 0)
                        {
                            isDown = true;

                            if (!loop)
                                Stop();
                        }
                        else
                        {
                            currentFrame -= 1;
                        }
                    }
                    
                    
                }
                else
                {
                    // if the last frame is already displayed, go back to the first
                    if (currentFrame >= frames[idxFrame].frames.Length - 1)
                    {
                        currentFrame = initialFrame;

                        if (!loop)
                            Stop();
                    }
                    else
                    {
                        currentFrame += 1;
                        Debug.Log(currentFrame);
                    }
                }
            }
        }
    }
}