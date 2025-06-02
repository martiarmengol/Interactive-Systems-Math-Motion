using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OperationGenerator2 : MonoBehaviour
{
    public enum SubMode { Addition, Multiplication, Mixed }

    [Header("Mode Settings")]
    public SubMode subMode = SubMode.Multiplication; // Current operation mode
    public int minOperand = 2; // Minimum operand value
    public int maxOperand = 6; // Maximum operand value

    // Output
    public int operandA { get; private set; } // First operand
    public int operandB { get; private set; } // Second operand
    public SubMode CurrentMode { get; private set; } // Current operation mode

    // Event triggered when new operation is generated
    public event Action<int, int, SubMode> OnOperationGenerated;

    // Generate a new random operation
    public void Generate()
    {
        // Allow 'mixed' to randomly pick Addition or Multiplication
        CurrentMode = subMode == SubMode.Mixed
            ? (UnityEngine.Random.value < 0.5f ? SubMode.Addition : SubMode.Multiplication)
            : subMode;

        operandA = UnityEngine.Random.Range(minOperand, maxOperand + 1);
        operandB = UnityEngine.Random.Range(minOperand, maxOperand + 1);

        OnOperationGenerated?.Invoke(operandA, operandB, CurrentMode);
    }

    // Get the operation as a string (e.g., "3 + 2" or "4 × 5")
    public string GetOperationString()
    {
        var op = CurrentMode == SubMode.Addition ? "+" : "×";
        return $"{operandA} {op} {operandB}";
    }

    // Calculate the result of the operation
    public int GetResult()
    {
        return CurrentMode == SubMode.Addition
            ? (operandA + operandB)
            : (operandA * operandB);
    }
}
