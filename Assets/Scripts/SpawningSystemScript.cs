using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObjectClasses.SpawningClasses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawningSystemScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject playerNextLevelTrigger;
    [SerializeField] [CanBeNull] private GameObject queryWordObject;
    [SerializeField] public List<QueryWordPosition> possibleWordPositions;

    public static bool RestartingLevel;
    private static List<string> _possibleWords;
    public static int NumberOfWords;
    public static List<string> ListOfCollectedWords = new List<string>();
    
    private static List<int> _indexList = new List<int>();
    private static int _difference;

    private void OnEnable()
    {
        LevelLoaderScript.TriggerRemovingWordsFromList += RemovePickedUpWords;
        DatabaseManagerScript.SliceTheQuery += HandleQuerySlicing;
    }
    
    private void OnDisable()
    {
        LevelLoaderScript.TriggerRemovingWordsFromList -= RemovePickedUpWords;
        DatabaseManagerScript.SliceTheQuery -= HandleQuerySlicing;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 9) return;
        HandlePlayerSpawning();
 
        if (InputScript.IsSelectedLevel || queryWordObject == null) return;
        
        TriggerDetectionScript.IsAllowedToNextLevel = false;
        HandleQueryWordSpawning();
    }

    private void HandlePlayerSpawning()
    {
        player.transform.position = playerSpawnPoint.transform.position;
    }
    
    private void HandleQueryWordSpawning()
    {
        if (queryWordObject == null) return;
        _indexList.Clear();
        for (var i = 0; i < NumberOfWords; i++)
        {
            QueryWordPosition positionIndex = GetRandomPosition();
            GameObject copy = Instantiate(queryWordObject, new Vector3(positionIndex.x, positionIndex.y, queryWordObject.transform.position.z),
                queryWordObject.transform.rotation);
            TextMeshPro text = copy.GetComponentInChildren<TextMeshPro>();
            var randomWord = GetRandomWord();
            if (text) text.text = randomWord;
        }
    }
    
    private QueryWordPosition GetRandomPosition()
    {
        var randomIndex = UnityEngine.Random.Range(0, possibleWordPositions.Count);
        while (possibleWordPositions[randomIndex].isTaken)
        {
            randomIndex = UnityEngine.Random.Range(0, possibleWordPositions.Count);
        }

        possibleWordPositions[randomIndex].isTaken = true;
        return possibleWordPositions[randomIndex];
    }
    
    private static string GetRandomWord()
    {
        var randomIndex = UnityEngine.Random.Range(0, _possibleWords.Count);
        while (_indexList.Contains(randomIndex))
        {
            randomIndex = UnityEngine.Random.Range(0, _possibleWords.Count);
        }
        
        _indexList.Add(randomIndex);
        var word = _possibleWords[randomIndex];
        return word;
    }

    private static void RemovePickedUpWords()
    {
        _indexList.Sort();
        
        for (int i = 0; i < _indexList.Count; i++)
        {
            if (i > 0) _indexList[i]--;
            ListOfCollectedWords.Add(_possibleWords[_indexList[i]]);
            _possibleWords.RemoveAt(_indexList[i]);
        }
        
        if (_difference < 0 && NumberOfWords == 1)
        {
            if (SceneManager.GetActiveScene().buildIndex - 1 > (_difference * -1))
                NumberOfWords += 2;
        }
        else if (_difference > 0 && NumberOfWords == 1)
        {
            if (SceneManager.GetActiveScene().buildIndex - 1 > _difference)
                NumberOfWords += 1;
        }
    }

    private void HandleQuerySlicing(string queryWord)
    {
        _possibleWords = new List<string>(queryWord.Split(' '));

        _difference = 6 - (_possibleWords.Count - 6); // 6 is the number of scenes
        NumberOfWords = 1;
    }
}