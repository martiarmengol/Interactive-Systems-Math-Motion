using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    public TMP_Text operationText;
    public TMP_Text scoreText;

    public void SetOperation(int a, char op, int b)
        => operationText.text = $"{a} {op} {b} = ?";

    public void UpdateScore(int filled, int goal)
        => scoreText.text = $"Tiles: {filled}/{goal}";
}

