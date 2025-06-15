using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    [SerializeField] private Drop dropscript;
    private ParticleSystem particleSystem;
    [SerializeField] private GameObject dust;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem = dust.GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("goede middag");
        particleSystem.Play();
        dropscript.DropIt();
    }
}
