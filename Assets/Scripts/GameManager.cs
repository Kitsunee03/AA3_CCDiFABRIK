using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> levels;
    [SerializeField] private List<bool> CCDActivePerLevel;
    [SerializeField] private List<bool> FABRIKActivePerLevel;

    private DronController dronController;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        LoadLevel();
        dronController = FindFirstObjectByType<DronController>();
    }

    public void LoadLevel()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Count)
        {
            Debug.LogError("Invalid level index: " + currentLevelIndex);
            return;
        }

        if (currentLevelIndex > 0) { Destroy(levels[currentLevelIndex - 1]); }
        levels[currentLevelIndex].SetActive(true);

        Vector3 spawnPosition = levels[currentLevelIndex].transform.Find("DronSpawnPoint").position;
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
}