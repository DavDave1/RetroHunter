using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RaSetMaker.Utils
{
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue>
    {
        [XmlElement("Item")]
        public List<SerializableKeyValuePair<TKey, TValue>> Items { get; set; } = [];

        public SerializableDictionary()
        {
        }
        public SerializableDictionary(Dictionary<TKey, TValue> dict)
        {
            foreach (var (key, value) in dict)
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            Items.Add(new SerializableKeyValuePair<TKey, TValue> { Key = key, Value = value });
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in Items)
            {
                dictionary[item.Key] = item.Value;
            }
            return dictionary;
        }
    }

    public class SerializableKeyValuePair<TKey, TValue>
    {
        [XmlElement("Key")]
        public TKey Key { get; set; }

        [XmlElement("Value")]
        public TValue Value { get; set; }
    }
}
