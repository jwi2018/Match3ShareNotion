using UnityEngine;

public class MissileParticleCreater : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject[] missileObject;

    public float particleCreateTime = 0.1f;
    public bool isActive = true;

    private float preTime;

    private void FixedUpdate()
    {
        preTime += Time.deltaTime;
        if (preTime > particleCreateTime)
        {
            preTime = 0;
            CreateParticle();
        }
    }

    public void CreateParticle()
    {
        //if (!isActive) return;

        //for (var i = 0; i < missileObject.Length; i++)
        //{
        //    if (missileObject[i] == null) continue;

        //    var particleObj = Instantiate(particle, missileObject[i].transform);
        //    particleObj.transform.localPosition = Vector2.zero;
        //}
    }
}