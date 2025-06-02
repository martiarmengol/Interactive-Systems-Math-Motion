using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationGenerator : MonoBehaviour
{
    // Operation modes
    public enum Mode { Addition, Subtraction, Mixed }
    public Mode mode;

    // Range for operands
    public int minValue = 1, maxValue = 10;

    // Operation data structure
    public struct Operation
    {
        public int a, b;     // Operands
        public int result;   // Calculation result
        public char op;      // Operator symbol
    }

    // Generates a new math operation based on current mode
    public Operation GetNextOperation()
    {
        int a, b;
        char opChar;

        // Determine operator based on mode
        if (mode == Mode.Mixed)
            opChar = (Random.value < 0.5f ? '+' : '-');
        else
            opChar = (mode == Mode.Addition ? '+' : '-');

        // Generate addition operation
        if (opChar == '+')
        {
            a = Random.Range(minValue, maxValue + 1);
            b = Random.Range(minValue, maxValue + 1);
            return new Operation
            {
                a = a,
                b = b,
                op = '+',
                result = a + b
            };
        }
        // Generate subtraction operation (always positive result)
        else
        {
            b = Random.Range(minValue, maxValue);     // Ensure b < maxValue
            a = Random.Range(b + 1, maxValue + 1);    // Ensure a > b
            return new Operation
            {
                a = a,
                b = b,
                op = '-',
                result = a - b
            };
        }
    }
}