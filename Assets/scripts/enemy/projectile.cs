using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float lifetime = 2f;

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<NPC_Controller>(out NPC_Controller npc))
        {
            npc.curHealth -= 30;
            Destroy(gameObject);
        }
    }
}
