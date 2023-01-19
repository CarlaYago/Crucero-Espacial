using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToUppercase : MonoBehaviour
{
    InputField userInput;
    TMP_InputField userInputTMP;

    string inputUpperCase;

    void Start()
    {
        if (TryGetComponent(out InputField input))
        {
            userInput = input;
            userInput.onValueChanged.AddListener(delegate { ToUpperCase(); });
        }
        else
        {
            userInputTMP = GetComponent<TMP_InputField>();
            userInputTMP.onValueChanged.AddListener(delegate { ToUpperCaseTMP(); });
        }
    }

    private void Update()
    {
        if (userInput != null)
        {
            userInput.text = inputUpperCase;
        }
        else
        {
            userInputTMP.text = inputUpperCase;
        }
    }

    void ToUpperCase()
    {
        inputUpperCase = userInput.text.ToUpper();
    }

    void ToUpperCaseTMP()
    {
        inputUpperCase = userInputTMP.text.ToUpper();
    }
}