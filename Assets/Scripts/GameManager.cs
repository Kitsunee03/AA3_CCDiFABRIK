using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObject> levels;
    private GameObject currentLevel;
    [SerializeField] private List<bool> CCDActivePerLevel;
    [SerializeField] private List<bool> FABRIKActivePerLevel;

    private DronController dronController;
    private AudioSource audioSource;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else
        {
            Destroy(gameObject);
            return;
        }

        TryGetComponent(out audioSource);

        LoadLevel();
        dronController = FindFirstObjectByType<DronController>();
    }

    public void LoadLevel()
    {
        if (currentLevelIndex >= levels.Count)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (currentLevel != null) { Destroy(currentLevel); }
        currentLevel = Instantiate(levels[currentLevelIndex]);

        Vector3 spawnPosition = currentLevel.transform.Find("DronSpawnPoint").position;
        if (dronController != null)
        {
            dronController.transform.position = spawnPosition;
            dronController.SetArmActiveStates(
                CCDActivePerLevel[currentLevelIndex],
                FABRIKActivePerLevel[currentLevelIndex]
            );
        }

        currentLevelIndex++;
    }

    public void EnableLevelExit()
    {
        currentLevel.transform.Find("LevelExit").gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        currentLevelIndex--; // decrement to reload the same level
        LoadLevel();
    }
    public void PlaySFX(AudioClip p_clip) { audioSource?.PlayOneShot(p_clip); }
}