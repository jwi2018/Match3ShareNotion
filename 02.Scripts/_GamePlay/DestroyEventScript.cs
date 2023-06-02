using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEventScript : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
