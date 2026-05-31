using System.Collections.Generic;
using ObjectClasses.SpawningClasses;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawningSystemScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject playerNextLevelTrigger;
    
    //    - Using a dictionary for now but in the future will have a dedicated DataBase for storing all of these positions
    //       and upon loading in the level will fetch all of this data from there
    
    private readonly Dictionary<int, MainObjectSpawnerClass> _playerSpawnLocations = new ()
    {
        {
            1, new ()
            {
                LevelName = "IntroductionScene",
                HasDoor = false,
                PlayerSpawnClass = new ()
                {
                    SpawnPosition = new Vector3()
                },
                EnemySpawnClass =  new ()
            }
        },
        {
            2, new ()
            {
                LevelName = "Area1Level1",
                HasDoor = false,
                PlayerSpawnClass = new ()
                {
                    SpawnPosition = new Vector3()
                },
                EnemySpawnClass =  new ()
            }
        },
        {
            3, new MainObjectSpawnerClass()
            {
                LevelName = "Area1Level2",
                HasDoor = false,
                PlayerSpawnClass = new ()
                {
                    SpawnPosition = new Vector3()
                },
                EnemySpawnClass =  new ()
            }
        },
        {
            4, new MainObjectSpawnerClass()
            {
                LevelName = "Area1Level3",
                HasDoor = true,
                PlayerSpawnClass = new ()
                {
                    SpawnPosition = new Vector3()
                },
                EnemySpawnClass =  new ()
            }
        },
        {
            5, new MainObjectSpawnerClass()
            {
                LevelName = "Area1Level4",
                HasDoor = false,
                PlayerSpawnClass = new ()
                {
                    SpawnPosition = new Vector3()
                },
                EnemySpawnClass =  new ()
            }
        },
    };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        foreach (var playerSpawnLocation in _playerSpawnLocations)
        {
            if (currentSceneIndex == playerSpawnLocation.Key)
            {
                HandlePlayerSpawning();
            }
        }
    }

    private void HandlePlayerSpawning()
    {
        player.transform.position = playerSpawnPoint.transform.position;
    }
}