using System.Collections.Generic;
using UnityEngine;

namespace Pinky.Localization.Utility
{
    public abstract class SerializableHashMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private TKey[] keys;
        [SerializeField]
        private TValue[] values;

        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Length != values.Length)
                throw new System.ArgumentOutOfRangeException($"Keys count is different from values count. The length of {nameof(keys)} is {keys.Length}, the length of {nameof(values)} is {values.Length}");

            for (int i = 0; i < keys.Length; i++)
                Add(keys[i], values[i]);

            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            keys = new TKey[Count];
            values = new TValue[Count];

            int index = 0;

            foreach(KeyValuePair<TKey, TValue> kvp in this)
            {
                keys[index] = kvp.Key;
                values[index] = kvp.Value;
                index++;
            }
        }
    }
}