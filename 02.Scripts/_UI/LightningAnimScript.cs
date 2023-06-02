using System.Collections;
using UnityEngine;

public class LightningAnimScript : MonoBehaviour
{
    [SerializeField] private float Speed;

    [SerializeField] private GameObject Particle;

    private GameTile _tile;
    private Vector3 mEndPosition;
    private LineRenderer mLineRenderer;
    private float ParticleEndTime;

    public Vector2 SetStartPosition
    {
        set =>
            //mLineRenderer.SetPosition(0, value);
            //mLineRenderer.SetPosition(1, value);
            transform.localPosition = value;
    }

    public Vector2 SetEndPosition_Tutorial
    {
        set
        {
            mEndPosition = value;
            StartCoroutine(RendererMove_Tutorial());
        }
    }

    public Vector2 SetEndPosition
    {
        set
        {
            mEndPosition = value;
            LineStart();
        }
    }

    // Use this for initialization
    private void Awake()
    {
        mLineRenderer = GetComponent<LineRenderer>();
    }

    public void Setting(GameTile tile)
    {
        _tile = tile;
    }

    private void LineStart()
    {
        //AnimManager.PlayAnimCount++;
        StartCoroutine("RendererMove");
    }

    public void BoardShaking(float Time = 0.6f, float Strong = 0.05f)
    {
        //if (GameObject.Find("EarthquakeManager") != null)
        //{
        //    GameObject.Find("EarthquakeManager").GetComponent<EarthquakeScript>().SetTime = Time;
        //    GameObject.Find("EarthquakeManager").GetComponent<EarthquakeScript>().SetStrong = Strong;
        //}
    }

    private IEnumerator RendererMove()
    {
        yield return new WaitForEndOfFrame();
        var StartTime = 0.0f;
        ParticleEndTime = GetComponent<TrailRenderer>().time;
        while (StartTime < ParticleEndTime)
        {
            if ((transform.localPosition - mEndPosition).magnitude > Speed * Time.deltaTime)
            {
                var Dir = -(transform.localPosition - mEndPosition).normalized;
                var NewLinePosition = transform.localPosition;
                NewLinePosition += new Vector3(Speed * Time.deltaTime * Dir.x, Speed * Time.deltaTime * Dir.y, 0);
                transform.localPosition = NewLinePosition;
            }
            else
            {
                if (Particle.activeSelf == false) BoardShaking();
                transform.localPosition = mEndPosition;
                Particle.SetActive(true);
            }

            StartTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //ItemPresentationManager.GetInstance.EndLightning(_tile);

        /*while (mLineRenderer.GetPosition(0) != mEndPosition)
        {
            if ((mLineRenderer.GetPosition(0) - mEndPosition).magnitude > Speed * Time.deltaTime)
            {
                Vector3 Dir = -(mLineRenderer.GetPosition(0) - mEndPosition).normalized;
                Vector3 NewLinePosition = mLineRenderer.GetPosition(0);
                NewLinePosition += new Vector3(Speed * Time.deltaTime * Dir.x, Speed * Time.deltaTime * Dir.y, 0);
                mLineRenderer.SetPosition(0, NewLinePosition);
            }
            else
            {
                mLineRenderer.SetPosition(0, mEndPosition);
            }
            yield return new WaitForEndOfFrame();
        }*/

        //AnimManager.PlayAnimCount--;
        /*Vector3 VecInit = new Vector3(0, 0, 0);
        mLineRenderer.SetPosition(0, VecInit);
        mLineRenderer.SetPosition(1, VecInit);*/
        Destroy(this);
    }

    private IEnumerator RendererMove_Tutorial()
    {
        yield return new WaitForEndOfFrame();
        var StartTime = 0.0f;
        ParticleEndTime = GetComponent<TrailRenderer>().time;

        while (StartTime < ParticleEndTime)
        {
            if ((transform.localPosition - mEndPosition).magnitude > Speed * Time.deltaTime)
            {
                var Dir = -(transform.localPosition - mEndPosition).normalized;
                var NewLinePosition = transform.localPosition;
                NewLinePosition += new Vector3(Speed * Time.deltaTime * Dir.x, Speed * Time.deltaTime * Dir.y, 0);
                Debug.Log(NewLinePosition);
                transform.localPosition = NewLinePosition;
            }
            else
            {
                transform.localPosition = mEndPosition;
                Particle.SetActive(true);
            }

            StartTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
        Particle.SetActive(false);
    }
}