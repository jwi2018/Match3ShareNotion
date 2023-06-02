using System.Collections;
using UnityEngine;

public class LightningLine : MonoBehaviour
{
    [SerializeField] private LineRenderer mLineRenderer;

    [SerializeField] private LineRenderer mLineRenderer_Child;

    [SerializeField] private GameObject rainbowParticle;

    [SerializeField] private Transform mStartTransform;

    [SerializeField] private Transform mTargetTransform;

    private GameBlock bossBlock;
    private EID changeID = EID.NORMAL;
    private Vector2 mEndPosition;
    private float moveTime = 0.4f;
    private Vector2 mStartPosition;
    private GameBlock targetBlock;

    private void Update()
    {
        var colorKeys = mLineRenderer.colorGradient.colorKeys;
        var alphaKeys = mLineRenderer.colorGradient.alphaKeys;
        var gradient = new Gradient();

        for (var i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].time -= 0.1f;
            if (colorKeys[i].time < 0) colorKeys[i].time = 1 + colorKeys[i].time;
        }

        gradient.SetKeys(colorKeys, alphaKeys);
        mLineRenderer.colorGradient = gradient;

        mStartPosition = mStartTransform.position;
        mEndPosition = mTargetTransform.position;
    }

    /*private void Start()
    {
        RendererSetting(mStartTransform, mTargetTransform);
    }*/
    public void SetBossBlock(GameBlock bBlock, GameBlock tBlock, EID id)
    {
        bossBlock = bBlock;
        targetBlock = tBlock;
        changeID = id;
    }

    public void RendererSetting(Transform StartTransform, Transform EndTransform, float MoveTime = 0.4f)
    {
        mStartTransform = StartTransform;
        mTargetTransform = EndTransform;
        moveTime = MoveTime;

        mStartPosition = mStartTransform.position;
        mEndPosition = mTargetTransform.position;
        mLineRenderer.SetPosition(0, mStartPosition);
        mLineRenderer_Child.SetPosition(0, mStartPosition);
        mLineRenderer.SetPosition(1, mStartPosition);
        mLineRenderer_Child.SetPosition(1, mStartPosition);
        StartCoroutine("RendererMoves");
    }

    private IEnumerator RendererMoves()
    {
        yield return new WaitForEndOfFrame();
        var StartTime = 0.0f;


        while (StartTime < moveTime)
        {
            yield return new WaitForEndOfFrame();
            StartTime += Time.deltaTime;
            //Vector2 NowPos = mLineRenderer.GetPosition(1);
            var MovePos = mEndPosition - mStartPosition;
            var TruePos = MovePos * (StartTime / moveTime);
            Debug.Log(MovePos);
            mLineRenderer.SetPosition(0, mStartPosition);
            mLineRenderer_Child.SetPosition(0, mStartPosition);
            mLineRenderer.SetPosition(1, mStartPosition + TruePos);
            mLineRenderer_Child.SetPosition(1, mStartPosition + TruePos);
        }

        targetBlock.SetIDWhenActiveRainbow(changeID);
        var obj = Instantiate(rainbowParticle);
        obj.transform.position = mEndPosition;
        obj.SetActive(true);

        StartTime = 0.0f;
        while (true)
        {
            StartTime += Time.deltaTime;
            mLineRenderer.SetPosition(0, mStartPosition);
            mLineRenderer_Child.SetPosition(0, mStartPosition);
            mLineRenderer.SetPosition(1, mEndPosition);
            mLineRenderer_Child.SetPosition(1, mEndPosition);

            var colorKeys = mLineRenderer.colorGradient.colorKeys;
            var alphaKeys = mLineRenderer.colorGradient.alphaKeys;
            var gradient = new Gradient();

            var Width = mLineRenderer.endWidth;
            var Width_Child = mLineRenderer_Child.endWidth;

            if (StartTime > 0.3f) break;

            yield return new WaitForEndOfFrame();
        }

        if (bossBlock != null) bossBlock.SetEndLightning();
        //this.gameObject.SetActive(false);
    }
}