using System.Collections;
using UnityEngine;

public class AnimationLightning : MonoBehaviour
{
    [SerializeField] private Vector3 mStartPosition;

    [SerializeField] private Vector3 mEndPosition;

    [SerializeField] private float ParticleEndTime;

    [SerializeField] private float Speed;

    [SerializeField] private GameObject Particle;

    private GameTile _tile;

    private LineRenderer mLineRenderer;

    // Use this for initialization
    private void Awake()
    {
        mLineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
    }

    public void StartLightningAnimation()
    {
        mLineRenderer.SetPosition(0, mStartPosition);
        mLineRenderer.SetPosition(1, mStartPosition);
        StartCoroutine("RendererMoves");
    }

    private IEnumerator RendererMoves()
    {
        yield return new WaitForEndOfFrame();
        var StartTime = 0.0f;
        while (StartTime < ParticleEndTime)
        {
            if ((mLineRenderer.GetPosition(1) - mEndPosition).magnitude > Speed * Time.deltaTime)
            {
                var Dir = -(mLineRenderer.GetPosition(1) - mEndPosition).normalized;
                var NewLinePosition = mLineRenderer.GetPosition(1);
                NewLinePosition += new Vector3(Speed * Time.deltaTime * Dir.x, Speed * Time.deltaTime * Dir.y, 0);
                mLineRenderer.SetPosition(1, NewLinePosition);
            }
            else
            {
                mLineRenderer.SetPosition(1, mEndPosition);
                Particle.transform.localPosition = mEndPosition;
                Particle.SetActive(true);
            }

            StartTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (mLineRenderer.GetPosition(0) != mEndPosition)
        {
            if ((mLineRenderer.GetPosition(0) - mEndPosition).magnitude > Speed * Time.deltaTime)
            {
                var Dir = -(mLineRenderer.GetPosition(0) - mEndPosition).normalized;
                var NewLinePosition = mLineRenderer.GetPosition(0);
                NewLinePosition += new Vector3(Speed * Time.deltaTime * Dir.x, Speed * Time.deltaTime * Dir.y, 0);
                mLineRenderer.SetPosition(0, NewLinePosition);
            }
            else
            {
                mLineRenderer.SetPosition(0, mEndPosition);
            }

            yield return new WaitForEndOfFrame();
        }

        //mLineRenderer.SetPosition(1, mStartPosition);
        gameObject.SetActive(false);
        Particle.SetActive(false);
    }
}