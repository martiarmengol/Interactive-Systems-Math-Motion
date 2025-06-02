using System.Collections.Generic;
using UnityEngine;

public enum KeyType { MAIN_MENU, NEXT, CLEAR }

public class ControlKeyManager : MonoBehaviour
{
    [System.Serializable]
    public class ControlKeyPair
    {
        public TileController keyPlayer1; // Key for player 1
        public TileController keyPlayer2; // Key for player 2
        public KeyType keyType;           // Type of key
    }

    public List<ControlKeyPair> controlKeyPairs = new List<ControlKeyPair>(); // List of key pairs
    public delegate void KeyCombinationHandler(KeyType keyType); // Delegate for key combination event
    public event KeyCombinationHandler OnKeyCombinationPressed;  // Event for key combination

    private Dictionary<KeyType, int> activeKeys = new Dictionary<KeyType, int>(); // Tracks active keys
    private Dictionary<KeyType, bool> keyPairsPressed = new Dictionary<KeyType, bool>(); // Tracks if pair pressed

    // Stores delegates for each tile
    private class KeyEventDelegates
    {
        public ColorChangeOnTouch.TileLitHandler onLit;
        public ColorChangeOnTouch.TileLitHandler onUnlit;
    }

    private Dictionary<TileController, KeyEventDelegates> keyDelegates = new Dictionary<TileController, KeyEventDelegates>();

    void Start()
    {
        // Initialize dictionaries for each key type
        foreach (KeyType type in System.Enum.GetValues(typeof(KeyType)))
        {
            activeKeys[type] = 0;
            keyPairsPressed[type] = false;
        }

        // Subscribe to events for each key pair
        foreach (var pair in controlKeyPairs)
        {
            SubscribeTile(pair.keyPlayer1, pair.keyType);
            SubscribeTile(pair.keyPlayer2, pair.keyType);
        }
    }

    // Subscribe to tile events
    void SubscribeTile(TileController tile, KeyType keyType)
    {
        if (tile == null || tile.colorChanger == null) return;

        var delegates = new KeyEventDelegates
        {
            onLit = (_) => OnKeyActivated(keyType),
            onUnlit = (_) => OnKeyDeactivated(keyType)
        };

        tile.colorChanger.OnTileLit += delegates.onLit;
        tile.colorChanger.OnTileUnlit += delegates.onUnlit;

        keyDelegates[tile] = delegates;
    }

    // Unsubscribe from tile events
    void UnsubscribeTile(TileController tile)
    {
        if (tile == null || tile.colorChanger == null || !keyDelegates.ContainsKey(tile)) return;

        var delegates = keyDelegates[tile];
        tile.colorChanger.OnTileLit -= delegates.onLit;
        tile.colorChanger.OnTileUnlit -= delegates.onUnlit;

        keyDelegates.Remove(tile);
    }

    // Called when a key is activated (lit)
    void OnKeyActivated(KeyType keyType)
    {
        activeKeys[keyType]++;
        CheckKeyCombination(keyType);
    }

    // Called when a key is deactivated (unlit)
    void OnKeyDeactivated(KeyType keyType)
    {
        activeKeys[keyType]--;
        keyPairsPressed[keyType] = false;

        // Cancel check if NEXT key is released
        if (keyType == KeyType.NEXT)
        {
            FindObjectOfType<GameMode1Manager>()?.CancelCheck();
            FindObjectOfType<GameMode2Manager>()?.CancelCheck();
        }
    }

    // Check if both keys in a pair are pressed
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
        // Unsubscribe from all tile events
        foreach (var pair in controlKeyPairs)
        {
            UnsubscribeTile(pair.keyPlayer1);
            UnsubscribeTile(pair.keyPlayer2);
        }
    }
}
