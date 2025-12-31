using System;
using Server;

namespace Server.Items
{
    /// <summary>
    /// Represents a single key entry in the vendor stone with its price and availability status
    /// </summary>
    public class KeyVendorEntry
    {
        private Type _keyType;
        private string _name;
        private int _price;
        private bool _enabled;
        private int _itemId;
        private int _hue;

        public Type KeyType { get => _keyType; set => _keyType = value; }
        public string Name { get => _name; set => _name = value; }
        public int Price { get => _price; set => _price = value; }
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public int ItemId { get => _itemId; set => _itemId = value; }
        public int Hue { get => _hue; set => _hue = value; }

        public KeyVendorEntry(Type keyType, string name, int price, int itemId = 0x176B, int hue = 0, bool enabled = true)
        {
            _keyType = keyType;
            _name = name;
            _price = price;
            _itemId = itemId;
            _hue = hue;
            _enabled = enabled;
        }

        public KeyVendorEntry(IGenericReader reader)
        {
            Deserialize(reader);
        }

        /// <summary>
        /// Creates an instance of the key
        /// </summary>
        public Item CreateKey()
        {
            try
            {
                return (Item)Activator.CreateInstance(_keyType);
            }
            catch
            {
                return null;
            }
        }

        public void Serialize(IGenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(_keyType?.FullName ?? "");
            writer.Write(_name);
            writer.Write(_price);
            writer.Write(_enabled);
            writer.Write(_itemId);
            writer.Write(_hue);
        }

        public void Deserialize(IGenericReader reader)
        {
            int version = reader.ReadInt();

            string typeName = reader.ReadString();
            _keyType = !string.IsNullOrEmpty(typeName) ? AssemblyHandler.FindTypeByFullName(typeName) : null;
            _name = reader.ReadString();
            _price = reader.ReadInt();
            _enabled = reader.ReadBool();
            _itemId = reader.ReadInt();
            _hue = reader.ReadInt();
        }
    }
}
