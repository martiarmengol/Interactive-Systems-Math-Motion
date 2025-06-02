using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text operationText; // Reference to the text UI element

    // Sets the operation text in the UI
    public void SetOperation(int a, char op, int b) => operationText.text = $"{a} {op} {b} = ?";
}

