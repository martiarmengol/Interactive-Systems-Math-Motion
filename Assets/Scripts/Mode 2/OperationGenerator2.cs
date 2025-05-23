using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OperationGenerator2 : MonoBehaviour
{
    public enum SubMode { Addition, Multiplication, Mixed }

    [Header("Mode Settings")]
    public SubMode subMode = SubMode.Multiplication;
    public int minOperand = 2;
    public int maxOperand = 6;

    // output
    public int operandA { get; private set; }
    public int operandB { get; private set; }
    public SubMode CurrentMode { get; private set; }

    /// <summary>
    /// Fired immediately after Generate() picks new operands.
    /// </summary>
    public event Action<int, int, SubMode> OnOperationGenerated;

    /// <summary>
    /// Call this to pick a new operation.
    /// </summary>
    public void Generate()
    {
        // allow “mixed” to randomly pick A or M
        CurrentMode = subMode == SubMode.Mixed
            ? (UnityEngine.Random.value < 0.5f ? SubMode.Addition : SubMode.Multiplication)
            : subMode;

        operandA = UnityEngine.Random.Range(minOperand, maxOperand + 1);
        operandB = UnityEngine.Random.Range(minOperand, maxOperand + 1);

        OnOperationGenerated?.Invoke(operandA, operandB, CurrentMode);
    }

    /// <summary>
    /// Returns a display string like “3 + 2” or “4 × 5”
    /// </summary>
    public string GetOperationString()
    {
        var op = CurrentMode == SubMode.Addition ? "+" : "×";
        return $"{operandA} {op} {operandB}";
    }

    /// <summary>
    /// Compute the numerical result.
    /// </summary>
    public int GetResult()
    {
        return CurrentMode == SubMode.Addition
            ? (operandA + operandB)
            : (operandA * operandB);
    }
}
