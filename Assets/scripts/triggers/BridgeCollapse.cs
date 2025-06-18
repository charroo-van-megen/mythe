using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    [SerializeField] private GameObject stairs;      // Stairs GameObject with Drop.cs
    [SerializeField] private GameObject dust;        // ParticleSystem GameObject
    [SerializeField] private AudioSource BridgeCollapseSFX; // AudioSource assigned in Inspector

    private ParticleSystem particleSystem;
    private Drop dropScript;
    private bool hasActivated = false;

    void Start()
    {
        if (dust != null)
        {
            particleSystem = dust.GetComponent<ParticleSystem>();
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (stairs != null)
        {
            dropScript = stairs.GetComponent<Drop>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return; // Prevent re-triggering
        if (!other.CompareTag("Player")) return;

        hasActivated = true;

        Debug.Log("Player triggered bridge collapse");

        if (BridgeCollapseSFX != null)
            BridgeCollapseSFX.Play();

        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        if (dropScript != null)
            dropScript.DropIt();

        if (stairs != null)
            Destroy(stairs, 1.5f);
    }
}
