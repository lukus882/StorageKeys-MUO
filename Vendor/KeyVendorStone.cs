using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// A vendor stone that sells Storage Keys with configurable prices
    /// </summary>
    public class KeyVendorStone : Item
    {
        private List<KeyVendorEntry> _entries;
        private string _vendorName;
        private bool _useGold; // true = gold, false = could be tokens or other currency

        [CommandProperty(AccessLevel.GameMaster)]
        public string VendorName
        {
            get => _vendorName;
            set { _vendorName = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UseGold
        {
            get => _useGold;
            set => _useGold = value;
        }

        public List<KeyVendorEntry> Entries => _entries;

        [Constructible]
        public KeyVendorStone() : base(0xED4) // Stone graphic
        {
            Name = "Storage Key Vendor";
            _vendorName = "Storage Key Vendor";
            Movable = false;
            Hue = 1153; // Ice blue

            _entries = new List<KeyVendorEntry>();
            _useGold = true;

            // Initialize with default keys and prices
            InitializeDefaultKeys();
        }

        public KeyVendorStone(Serial serial) : base(serial)
        {
        }

        /// <summary>
        /// Safely adds a key entry only if the type exists
        /// </summary>
        private void TryAddKeyEntry(string typeName, string displayName, int price, int itemId = 0x176B, int hue = 0)
        {
            Type keyType = AssemblyHandler.FindTypeByName(typeName);
            if (keyType != null)
            {
                _entries.Add(new KeyVendorEntry(keyType, displayName, price, itemId, hue));
            }
            else
            {
                Console.WriteLine($"[KeyVendorStone] Warning: Key type '{typeName}' not found, skipping.");
            }
        }

        private void InitializeDefaultKeys()
        {
            // Resource Keys
            TryAddKeyEntry("IngotKey", "Ingot Key", 10000, 0x176B, 0);
            TryAddKeyEntry("ReagentKey", "Reagent Key", 10000, 0x176B, 0);
            TryAddKeyEntry("GemKey", "Gem Key", 10000, 0x176B, 0);
            TryAddKeyEntry("WoodKey", "Wood Key", 10000, 0x176B, 0);
            TryAddKeyEntry("GraniteKey", "Granite Key", 10000, 0x176B, 0);
            TryAddKeyEntry("PotionKey", "Potion Key", 15000, 0x176B, 0);
            TryAddKeyEntry("BeverageKey", "Beverage Key", 5000, 0x176B, 0);

            // Specialized Keys
            TryAddKeyEntry("BODKey", "BOD Key", 25000, 0x176B, 0);
            TryAddKeyEntry("BardsKey", "Bard's Key", 15000, 0xEB6, 0);
            TryAddKeyEntry("ScribesKey", "Scribe's Key", 20000, 0x176B, 0);
            TryAddKeyEntry("TreasureHuntersKey", "Treasure Hunter's Key", 25000, 0x176B, 0);
            TryAddKeyEntry("GardenersKey", "Gardener's Key", 10000, 0xFB7, 0);
            TryAddKeyEntry("ChefKey", "Chef's Key", 10000, 0x9ED, 0);
            TryAddKeyEntry("AdventurerKey", "Adventurer's Key", 20000, 0x176B, 0);
            TryAddKeyEntry("FishKey", "Fish Key", 10000, 0xFFA, 0);
            TryAddKeyEntry("MeatKey", "Meat Key", 10000, 0x176B, 0);

            // Equipment Keys
            TryAddKeyEntry("ArmorKey", "Armor Key", 30000, 0x176B, 0);
            TryAddKeyEntry("WeaponKey", "Weapon Key", 30000, 0x176B, 0);
            TryAddKeyEntry("ClothingKey", "Clothing Key", 20000, 0x176B, 0);
            TryAddKeyEntry("JewelryKey", "Jewelry Key", 25000, 0x176B, 0);
            TryAddKeyEntry("ArmoryKey", "Armory Key", 50000, 0x3D86, 0);

            // Crafting Keys
            TryAddKeyEntry("SmithyKey", "Smithy Key", 15000, 0x176B, 0);
            TryAddKeyEntry("TailorKey", "Tailor Key", 15000, 0x176B, 0);
            TryAddKeyEntry("ToolKey", "Tool Key", 10000, 0x176B, 0);
            TryAddKeyEntry("RunicToolKey", "Runic Tool Key", 35000, 0x176B, 0);

            // Special Keys
            TryAddKeyEntry("PSKey", "Power Scroll Key", 50000, 0x176B, 0);
            TryAddKeyEntry("ChampSkullKey", "Champion Skull Key", 40000, 0x2203, 0);
            TryAddKeyEntry("AddonDeedKey", "Addon Deed Key", 25000, 0x176B, 0);
            TryAddKeyEntry("JewelersKey", "Jeweler's Key", 15000, 0x176B, 0);

            // Master Key - most expensive
            TryAddKeyEntry("MasterItemStoreKey", "Master Storage Key", 100000, 0x176B, 1153);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;
                
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                // Show admin gump with option to switch to player view
                from.SendGump(new KeyVendorAdminGump(from, this, 0));
            }
            else
            {
                // Show player gump
                from.SendGump(new KeyVendorGump(from, this, 0));
            }
        }

        public override void AddNameProperties(IPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1060663, $"Keys Available\t{GetEnabledCount()}"); // ~1_val~: ~2_val~
            list.Add(1060662, $"Double-Click\tTo Browse"); // ~1_val~: ~2_val~
        }

        private int GetEnabledCount()
        {
            int count = 0;
            foreach (var entry in _entries)
            {
                if (entry.Enabled)
                    count++;
            }
            return count;
        }

        public bool TryPurchase(Mobile from, KeyVendorEntry entry)
        {
            if (from == null)
                return false;
                
            if (entry == null || !entry.Enabled)
            {
                from.SendMessage("That key is not available for purchase.");
                return false;
            }

            if (entry.KeyType == null)
            {
                from.SendMessage("This key type is not available.");
                return false;
            }

            Container pack = from.Backpack;
            if (pack == null)
            {
                from.SendMessage("You don't have a backpack!");
                return false;
            }

            if (_useGold)
            {
                if (Banker.Withdraw(from, entry.Price))
                {
                    from.SendMessage($"You have purchased a {entry.Name} for {entry.Price:N0} gold.");
                }
                else if (pack.ConsumeTotal(typeof(Gold), entry.Price))
                {
                    from.SendMessage($"You have purchased a {entry.Name} for {entry.Price:N0} gold.");
                }
                else
                {
                    from.SendMessage($"You do not have enough gold. You need {entry.Price:N0} gold.");
                    return false;
                }
            }

            Item key = entry.CreateKey();
            if (key == null)
            {
                from.SendMessage("Failed to create the key. Please report this to a GM.");
                // Refund
                if (_useGold)
                {
                    Banker.Deposit(from, entry.Price);
                }
                return false;
            }

            pack.DropItem(key);
            from.SendMessage($"A {entry.Name} has been placed in your backpack.");
            from.PlaySound(0x2E6); // Purchase sound

            return true;
        }

        public KeyVendorEntry GetEntry(int index)
        {
            if (index >= 0 && index < _entries.Count)
                return _entries[index];
            return null;
        }

        public void SetPrice(int index, int price)
        {
            if (index >= 0 && index < _entries.Count)
            {
                _entries[index].Price = Math.Max(0, price);
            }
        }

        public void SetEnabled(int index, bool enabled)
        {
            if (index >= 0 && index < _entries.Count)
            {
                _entries[index].Enabled = enabled;
            }
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_vendorName);
            writer.Write(_useGold);

            writer.Write(_entries.Count);
            foreach (var entry in _entries)
            {
                entry.Serialize(writer);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _vendorName = reader.ReadString();
            _useGold = reader.ReadBool();

            int count = reader.ReadInt();
            _entries = new List<KeyVendorEntry>(count);
            for (int i = 0; i < count; i++)
            {
                var entry = new KeyVendorEntry(reader);
                // Only add if the key type is valid
                if (entry.KeyType != null)
                {
                    _entries.Add(entry);
                }
                else
                {
                    Console.WriteLine($"[KeyVendorStone] Warning: Skipping entry with invalid key type '{entry.Name}'");
                }
            }
        }
    }
}
