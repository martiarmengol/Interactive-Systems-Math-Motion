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

    private class KeyEventDelegates
    {
        public ColorChangeOnTouch.TileLitHandler onLit;
        public ColorChangeOnTouch.TileLitHandler onUnlit;
    }

    private Dictionary<TileController, KeyEventDelegates> keyDelegates = new Dictionary<TileController, KeyEventDelegates>();


    void Start()
    {
        foreach (KeyType type in System.Enum.GetValues(typeof(KeyType)))
        {
            activeKeys[type] = 0;
            keyPairsPressed[type] = false;
        }

        foreach (var pair in controlKeyPairs)
        {
            SubscribeTile(pair.keyPlayer1, pair.keyType);
            SubscribeTile(pair.keyPlayer2, pair.keyType);
        }
    }

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

    void UnsubscribeTile(TileController tile)
    {
        if (tile == null || tile.colorChanger == null || !keyDelegates.ContainsKey(tile)) return;

        var delegates = keyDelegates[tile];
        tile.colorChanger.OnTileLit -= delegates.onLit;
        tile.colorChanger.OnTileUnlit -= delegates.onUnlit;

        keyDelegates.Remove(tile);
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

        if (keyType == KeyType.NEXT)
        {
            FindObjectOfType<GameMode1Manager>()?.CancelCheck();
            FindObjectOfType<GameMode2Manager>()?.CancelCheck();
        }
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
        foreach (var pair in controlKeyPairs)
        {
            UnsubscribeTile(pair.keyPlayer1);
            UnsubscribeTile(pair.keyPlayer2);
        }
    }
}
