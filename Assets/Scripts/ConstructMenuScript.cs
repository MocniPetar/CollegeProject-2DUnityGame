using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructMenuScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textListObject;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _win;
    [SerializeField] private GameObject _lose;
    [SerializeField] private Button _submitButton;
    private List<string> _collectedWords;

    private void OnEnable()
    {
        DatabaseManagerScript.TestEndLeve += Test;
    }

    private void OnDisable()
    {
        DatabaseManagerScript.TestEndLeve -= Test;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _win.SetActive(false);
        _lose.SetActive(false);
        _submitButton.onClick.AddListener(async () => await TestQuery());
        string stringOfCollectedWords = "";
        _collectedWords = new List<string>(SpawningSystemScript.ListOfCollectedWords);
        foreach (var word in _collectedWords)
        {
            stringOfCollectedWords += word + " ";
        }
        
        textListObject.text = stringOfCollectedWords;
    }

    private void Test(string query)
    {
        string stringOfCollectedWords = "";
        _collectedWords = new List<string>(query.Split(' '));
        foreach (var word in _collectedWords)
        {
            stringOfCollectedWords += word + " ";
        }
        textListObject.text = stringOfCollectedWords;
    }

    private async Task TestQuery()
    {
        var isCorrect = await DatabaseManagerScript.CheckIfQueryIsCorrectAsync(_inputField.text);
        if(isCorrect) _win.SetActive(true);
        else _lose.SetActive(true);
    }
}
