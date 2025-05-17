using UnityEngine;

public class OperationGenerator : MonoBehaviour
{
    [Header("Multiplier Range")]
    public int minValue = 1;
    public int maxValue = 8;

    public struct Operation
    {
        public int a, b, result;
        public char op;
    }

    public Operation GetNextMultiplication()
    {
        int a = Random.Range(minValue, maxValue + 1);
        int b = Random.Range(minValue, maxValue + 1);
        int res = a * b;
        return new Operation { a = a, b = b, result = res, op = '×' };
    }
}
