using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public float lifeTime = 2.0f;
    void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }
}
