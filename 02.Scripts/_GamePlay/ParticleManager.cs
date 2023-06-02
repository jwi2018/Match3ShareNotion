using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticleColorMatrix
{
    public EColor color;
    public List<GameObject> particleObj;
}

[Serializable]
public class ParticlePrefab
{
    public string name;
    public EID id;
    public List<ParticleColorMatrix> particleColors;
}

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private Transform particleTransform;
    [SerializeField] private ParticlePrefab[] particles;
    [SerializeField] private GameObject hintParticle;
    [SerializeField] private GameObject scoreStarParticle;
    [SerializeField] private GameObject activeTimeBombIce;
    [SerializeField] private GameObject activeTimeBombLava;
    [SerializeField] private GameObject rainbowActiveParticle;

    [SerializeField] private ParticlePrefab[] _objectPrefabs;
    [SerializeField] public List<GameObject>[] _pooledObjects;
    [SerializeField] private int[] _amountToBuffer;
    [SerializeField] private int _defaultBufferAmount = 3;

    public void ShowParticle(EID id, EColor color, int hp, Vector3 position)
    {
        if (particles == null) return;

        if (id == EID.NORMAL)
        {
            var colorNumber = (int) color;
            var obj = DynamicObjectPool.GetInstance.GetObjectForType(string.Format("Particle_Jewel{0}", colorNumber),
                true);

            obj.transform.position = position;
            return;
        }

        foreach (var item in particles)
            if (item.id == id)
                foreach (var spriteWithColor in item.particleColors)
                    if (spriteWithColor.color == color)
                    {
                        var particleNum = 0;
                        foreach (var obj in spriteWithColor.particleObj)
                        {
                            if (particleNum == hp)
                            {
                                var o = Instantiate(obj, particleTransform);
                                o.transform.position = position;
                                return;
                            }

                            particleNum++;
                        }
                    }
    }

    public GameObject CreateParticles(Transform blocktransform, EDirection direction)
    {
        var particle = Instantiate(hintParticle);
        if (direction == EDirection.UP)
        {
            particle.transform.position = blocktransform.position + new Vector3(0, 0.4f, 0);
        }
        else if (direction == EDirection.LEFT)
        {
            particle.transform.position = blocktransform.position + new Vector3(-0.4f, 0, 0);
            particle.transform.Rotate(new Vector3(0, 0, 90));
        }
        else if (direction == EDirection.RIGHT)
        {
            particle.transform.position = blocktransform.position + new Vector3(0.4f, 0, 0);
            particle.transform.Rotate(new Vector3(0, 0, -90));
        }
        else if (direction == EDirection.DOWN)
        {
            particle.transform.position = blocktransform.position + new Vector3(0, -0.4f, 0);
            particle.transform.Rotate(new Vector3(0, 0, 180));
        }

        return particle;
    }

    public void DestroyParticle(GameObject obj)
    {
        Destroy(obj);
    }

    public void CreateStarPartice(Vector3 position)
    {
        var starObj = Instantiate(scoreStarParticle);
        starObj.transform.position = position;
    }

    public void CreateActiveTimeIcePartice(Vector3 position)
    {
        var starObj = Instantiate(activeTimeBombIce);
        starObj.transform.position = position;
    }

    public void CreateActiveTimeLavaPartice(Vector3 position)
    {
        var starObj = Instantiate(activeTimeBombLava);
        starObj.transform.position = position;
    }

    public void CreateRainbowActivePartice(Vector3 position)
    {
        var starObj = Instantiate(rainbowActiveParticle);
        starObj.transform.position = position;
    }
}