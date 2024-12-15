using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DestroyWithParticleSystems : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public void DestroyAfterParticlesStop()
    {
        if (GetComponent<Image>())GetComponent<Image>().enabled = false;

        if (particleSystem != null)
        {
            particleSystem.transform.SetParent(null);
            
            particleSystem.Stop();
            
            transform.DOKill();
            Destroy(gameObject, particleSystem.main.startLifetimeMultiplier);
            Destroy(particleSystem.gameObject, particleSystem.main.startLifetimeMultiplier);
        } else {
            Debug.Log("No particle system assigned - destroying object");
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
