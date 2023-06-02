using System.Collections;
using UnityEngine;

public class ParticleUnit : MonoBehaviour
{
    [SerializeField] private float Timer = 0.5f;
    //[SerializeField]
    //private int PlaySoundNumber = 0;

    private void Start()
    {
        StartCoroutine(ParticleStart());
        //if(this.GetComponent<AnimationListener>() != null)
        //{
        //    this.GetComponent<AnimationListener>().Play(PlaySoundNumber);
        //}
    }

    private IEnumerator ParticleStart()
    {
        yield return new WaitForSeconds(Timer);

        //bool isDynamicObject = DynamicObjectPool.GetInstance.PoolObject(gameObject, false);

        //if (!isDynamicObject)
        {
            Destroy(gameObject);
        }
    }
}