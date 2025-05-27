using System.Collections.Generic;
using UnityEngine;


public enum KeyType { MAIN_MENU, NEXT, CLEAR }

public class ControlKeyManager : MonoBehaviour
{
    

    [System.Serializable]
    public class ControlKeyPair
    {
        public TileController keyPlayer1;
        public TileController keyPlayer2;
        public KeyType keyType;
    }

    public List<ControlKeyPair> controlKeyPairs = new List<ControlKeyPair>();
    public delegate void KeyCombinationHandler(KeyType keyType);
    public event KeyCombinationHandler OnKeyCombinationPressed;

    private Dictionary<KeyType, int> activeKeys = new Dictionary<KeyType, int>();
    private Dictionary<KeyType, bool> keyPairsPressed = new Dictionary<KeyType, bool>();

    void Start()
    {
        // Inicializar diccionarios
        foreach (KeyType type in System.Enum.GetValues(typeof(KeyType)))
        {
            activeKeys.Add(type, 0);
            keyPairsPressed.Add(type, false);
        }

        // Suscribir eventos
        foreach (var pair in controlKeyPairs)
        {
            if (pair.keyPlayer1 != null && pair.keyPlayer1.colorChanger != null)
            {
                pair.keyPlayer1.colorChanger.OnTileLit += (_) => OnKeyActivated(pair.keyType);
                pair.keyPlayer1.colorChanger.OnTileUnlit += (_) => OnKeyDeactivated(pair.keyType);
            }

            if (pair.keyPlayer2 != null && pair.keyPlayer2.colorChanger != null)
            {
                pair.keyPlayer2.colorChanger.OnTileLit += (_) => OnKeyActivated(pair.keyType);
                pair.keyPlayer2.colorChanger.OnTileUnlit += (_) => OnKeyDeactivated(pair.keyType);
            }
        }
    }

    void OnKeyActivated(KeyType keyType)
    {
        activeKeys[keyType]++;
        CheckKeyCombination(keyType);
    }

    void OnKeyDeactivated(KeyType keyType)
    {
        activeKeys[keyType]--;
        keyPairsPressed[keyType] = false;
    }

    void CheckKeyCombination(KeyType keyType)
    {
        if (activeKeys[keyType] >= 2 && !keyPairsPressed[keyType])
        {
            keyPairsPressed[keyType] = true;
            OnKeyCombinationPressed?.Invoke(keyType);
        }
    }

    void OnDestroy()
    {
        // Limpiar suscripciones
        foreach (var pair in controlKeyPairs)
        {
            if (pair.keyPlayer1 != null && pair.keyPlayer1.colorChanger != null)
            {
                pair.keyPlayer1.colorChanger.OnTileLit -= (_) => OnKeyActivated(pair.keyType);
                pair.keyPlayer1.colorChanger.OnTileUnlit -= (_) => OnKeyDeactivated(pair.keyType);
            }

            if (pair.keyPlayer2 != null && pair.keyPlayer2.colorChanger != null)
            {
                pair.keyPlayer2.colorChanger.OnTileLit -= (_) => OnKeyActivated(pair.keyType);
                pair.keyPlayer2.colorChanger.OnTileUnlit -= (_) => OnKeyDeactivated(pair.keyType);
            }
        }
    }
}