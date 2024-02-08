using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField]
    float winOrCrashDelay = 2f;

    [SerializeField]
    AudioClip success;

    [SerializeField]
    AudioClip crash;

    [SerializeField]
    ParticleSystem successParticles;

    [SerializeField]
    ParticleSystem crashParticles;

    AudioSource audioSource;

    Rigidbody rocket;

    bool isTransitioning = false;

    // Cheat Codes
    bool shouldDetectCollisions = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rocket = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ListenForCheatCodes();
    }

    void ListenForCheatCodes()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            shouldDetectCollisions = !shouldDetectCollisions;
            rocket.detectCollisions = shouldDetectCollisions;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (isTransitioning)
        {
            return;
        }

        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        audioSource.Stop();
        isTransitioning = true;
        audioSource.PlayOneShot(success);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        rocket.isKinematic = true;
        Invoke("LoadNextLevel", winOrCrashDelay);
    }

    void StartCrashSequence()
    {
        // TODO: Add particle effect upon crash
        audioSource.Stop();
        isTransitioning = true;
        audioSource.PlayOneShot(crash);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", winOrCrashDelay);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        isTransitioning = false;
        rocket.isKinematic = false;
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
        isTransitioning = false;
    }
}
