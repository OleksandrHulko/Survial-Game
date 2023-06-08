using UnityEngine;

public class AxCutting : MonoBehaviour, IEffect
{
    #region Serialize Fields
    [SerializeField]
    private ParticleSystem particleSystem = null;
    [SerializeField]
    private AudioSource audioSource = null;
    #endregion


    #region Public Methods
    public void Play(Vector3 position)
    {
        transform.position = position;
        
        particleSystem.Play();
        audioSource.Play();
    }
    #endregion
}