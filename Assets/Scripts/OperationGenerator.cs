using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationGenerator : MonoBehaviour
{
    public enum Mode { Addition, Subtraction, Mixed }
    public Mode mode;
    public int minValue = 1, maxValue = 10;

    public struct Operation { public int a, b, result; public char op; }
    public Operation GetNextOperation()
    {
        int a = Random.Range(minValue, maxValue + 1);
        int b = Random.Range(minValue, maxValue + 1);
        char opChar;
        if (mode == Mode.Mixed)
            opChar = (Random.value < 0.5f ? '+' : '-');
        else opChar = (mode == Mode.Addition ? '+' : '-');

        int res = (opChar == '+') ? a + b : Mathf.Max(0, a - b);
        return new Operation { a = a, b = b, op = opChar, result = res };
    }
}

