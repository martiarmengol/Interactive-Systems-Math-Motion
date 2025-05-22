using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    public TMP_Text operationText;

    public void SetOperation(int a, char op, int b)
        => operationText.text = $"{a} {op} {b} = ?";
}

