# Universal Storage Keys v2.0.6 - ModernUO Edition

A comprehensive item storage system for ModernUO servers that allows players to store, organize, and manage various item types using specialized storage keys.

## Overview

Universal Storage Keys provides a flexible and extensible storage solution that supports:
- **Resource Storage**: Store stackable resources like ingots, reagents, gems, wood, and more
- **Item Lists**: Store non-stackable items like treasure maps, instruments, seeds, and BODs
- **Stash Storage**: Store equipment like armor, weapons, clothing, and jewelry with full property preservation
- **Master Keys**: Combine multiple storage keys into a single management interface
- **Vendor Stone**: Sell storage keys to players with configurable prices

## Features

- 🗝️ **Specialized Keys**: Pre-configured keys for common item types (reagents, ingots, potions, etc.)
- 📦 **Dynamic Storage**: Automatically categorizes and stores compatible items
- 🔍 **Advanced Sorting**: Sort stored items by various properties
- 🏠 **House Integration**: Keys can be locked down and secured in houses
- ⚒️ **Craft System Integration**: Automatically withdraws resources during crafting
- 📋 **Bulk Order Deed Support**: Store and organize small and large BODs
- 🏪 **Vendor Stone System**: Admin-configurable vendor stone to sell keys to players

## ModernUO Compatibility

This version has been updated for full compatibility with ModernUO, including:

### API Updates
- `GenericReader`/`GenericWriter` → `IGenericReader`/`IGenericWriter`
- `reader.ReadItem()` → `reader.ReadEntity<Item>()`
- `reader.ReadMobile()` → `reader.ReadEntity<Mobile>()`
- `ScriptCompiler.FindTypeByName()` → `AssemblyHandler.FindTypeByName()`
- `[Constructable]` → `[Constructible]`
- `ObjectPropertyList.UnderlyingStream` → `ObjectPropertyList.Buffer`

### Type Updates
- `Quality` enum → `InstrumentQuality`, `ArmorQuality`, `WeaponQuality` (context-specific)
- `RepairSkillType` → `RepairDeed.RepairSkillType` (nested enum)
- Various scroll and item type name corrections

---

## Vendor Stone System

The Vendor Stone allows server administrators to sell storage keys to players with fully configurable prices.

### Adding a Vendor Stone

Use the GM command to add a vendor stone:
```
[add KeyVendorStone
```

### Player Features

- **Browse Keys**: Double-click the stone to view available keys
- **Purchase**: Click on a key to see price and confirm purchase
- **Payment**: Accepts gold from bank account first, then backpack

### Admin Features (GameMaster+)

When a GM double-clicks the vendor stone, they see the Admin Panel with:

| Feature | Description |
|---------|-------------|
| **Toggle Enable/Disable** | Enable or disable individual keys for sale |
| **Edit Price** | Set custom price for each key |
| **Set All Prices** | Bulk price modification (fixed, percentage, add/subtract) |
| **Enable All / Disable All** | Quick bulk enable/disable |
| **Test Buy** | Get free keys for testing purposes |
| **View as Player** | Preview the player shopping experience |

### Default Key Prices

| Category | Key | Default Price |
|----------|-----|---------------|
| **Resource** | Ingot Key | 10,000 gp |
| **Resource** | Reagent Key | 10,000 gp |
| **Resource** | Gem Key | 10,000 gp |
| **Resource** | Wood Key | 10,000 gp |
| **Resource** | Granite Key | 10,000 gp |
| **Resource** | Potion Key | 15,000 gp |
| **Resource** | Beverage Key | 5,000 gp |
| **Specialized** | BOD Key | 25,000 gp |
| **Specialized** | Bard's Key | 15,000 gp |
| **Specialized** | Scribe's Key | 20,000 gp |
| **Specialized** | Treasure Hunter's Key | 25,000 gp |
| **Specialized** | Gardener's Key | 10,000 gp |
| **Specialized** | Chef's Key | 10,000 gp |
| **Specialized** | Adventurer's Key | 20,000 gp |
| **Specialized** | Fish Key | 10,000 gp |
| **Specialized** | Meat Key | 10,000 gp |
| **Equipment** | Armor Key | 30,000 gp |
| **Equipment** | Weapon Key | 30,000 gp |
| **Equipment** | Clothing Key | 20,000 gp |
| **Equipment** | Jewelry Key | 25,000 gp |
| **Equipment** | Armory Key | 50,000 gp |
| **Crafting** | Smithy Key | 15,000 gp |
| **Crafting** | Tailor Key | 15,000 gp |
| **Crafting** | Tool Key | 10,000 gp |
| **Crafting** | Runic Tool Key | 35,000 gp |
| **Special** | Power Scroll Key | 50,000 gp |
| **Special** | Champion Skull Key | 40,000 gp |
| **Special** | Addon Deed Key | 25,000 gp |
| **Special** | Jeweler's Key | 15,000 gp |
| **Special** | Master Storage Key | 100,000 gp |

### Vendor Stone Properties

| Property | Description |
|----------|-------------|
| `VendorName` | Display name shown on the stone and gump |
| `UseGold` | Whether to use gold as currency (default: true) |

---

## Available Keys

### Resource Keys
| Key | Description |
|-----|-------------|
| `IngotKey` | Stores all types of ingots |
| `ReagentKey` | Stores magic reagents |
| `GemKey` | Stores gems and precious stones |
| `WoodKey` | Stores logs and boards |
| `GraniteKey` | Stores granite types |
| `PotionKey` | Stores potions |
| `BeverageKey` | Stores beverages |

### Specialized Keys
| Key | Description |
|-----|-------------|
| `BODKey` | Stores Bulk Order Deeds |
| `BardsKey` | Stores musical instruments |
| `ScribesKey` | Stores scrolls |
| `TreasureHuntersKey` | Stores treasure maps and SOS bottles |
| `GardenersKey` | Stores seeds and gardening supplies |
| `ChefKey` | Stores cooking ingredients |
| `AdventurerKey` | Stores adventure-related items |

### Equipment Keys
| Key | Description |
|-----|-------------|
| `ArmorKey` | Stores armor pieces |
| `WeaponKey` | Stores weapons |
| `ClothingKey` | Stores clothing items |
| `JewelryKey` | Stores jewelry |
| `ArmoryKey` | Combined armor, weapons, clothing, and jewelry storage |

### Crafting Keys
| Key | Description |
|-----|-------------|
| `SmithyKey` | Stores blacksmithing resources |
| `TailorKey` | Stores tailoring resources |
| `ToolKey` | Stores crafting tools |
| `RunicToolKey` | Stores runic tools |

### Special Keys
| Key | Description |
|-----|-------------|
| `PSKey` | Stores Power Scrolls and Stat Scrolls |
| `ChampSkullKey` | Stores Champion Skulls |
| `AddonDeedKey` | Stores Addon Deeds |
| `MasterItemStoreKey` | Combines multiple keys into one interface |

---

## Usage

### Adding Items
1. Double-click the key to open the storage interface
2. Use the context menu and select "Add" to target items
3. Or use "Fill" to automatically add all compatible items from your backpack

### Withdrawing Items
1. Open the storage interface
2. Click on the item entry to withdraw
3. Specify quantity for stackable items

### Context Menu Options
- **Open**: Opens the storage interface
- **Add**: Target a single item to add
- **Fill**: Add all compatible items from backpack

### House Security
Keys can be locked down in houses and secured with the standard security levels:
- Owner
- Co-Owner
- Friends
- Anyone

---

## Installation

1. Copy the `StorageKeys-MUO` folder to your `Projects/UOContent/CUSTOM/Items/` directory
2. Ensure the folder structure is preserved
3. Build the project

## File Structure

```
StorageKeys-MUO/
├── Base Items/
│   ├── BaseStoreKey.cs      # Base class for all storage keys
│   └── MasterKey.cs         # Master key implementation
├── Main Data Management/
│   ├── ItemStore.cs         # Core storage functionality
│   ├── StoreEntries.cs      # Entry type definitions
│   ├── StashEntry.cs        # Stash storage for equipment
│   ├── ListEntry.cs         # List storage for unique items
│   ├── ItemListEntries.cs   # Item list entry types
│   ├── BODListEntries.cs    # BOD-specific entries
│   ├── StashListEntry.cs    # Stash list entry type
│   └── ContextMenus.cs      # Context menu definitions
├── Gumps/
│   ├── ItemStoreGump.cs     # Main storage interface
│   ├── ListEntryGump.cs     # List entry interface
│   ├── StashEntryGump.cs    # Stash entry interface
│   ├── MasterKeyGump.cs     # Master key interface
│   └── AddStashColumnGump.cs # Column customization
├── Items/
│   └── [Various Key Files]  # Individual key implementations
├── Vendor/
│   ├── KeyVendorStone.cs    # Vendor stone item
│   ├── KeyVendorEntry.cs    # Vendor entry data class
│   ├── KeyVendorGump.cs     # Player shopping interface
│   └── KeyVendorAdminGump.cs # Admin configuration panel
├── CliLoc Handler/
│   ├── Data/                # Localization data handling
│   └── Gumps/               # Localization viewer gumps
├── Extras/
│   ├── Extras.cs            # Additional utilities
│   └── ExtrasKey.cs         # Extra storage key
└── Commands/
    └── Commands.cs          # Admin commands
```

---

## Creating Custom Keys

To create a custom storage key, extend `BaseStoreKey` and override `EntryStructure`:

```csharp
public class MyCustomKey : BaseStoreKey
{
    public override List<StoreEntry> EntryStructure
    {
        get
        {
            List<StoreEntry> entry = base.EntryStructure;
            
            // Add resource entries for stackable items
            entry.Add(new ResourceEntry(typeof(MyItem), "My Item"));
            
            // Add list entries for unique items
            entry.Add(new ListEntry(typeof(MyUniqueItem), typeof(MyListEntry), "Unique Items"));
            
            return entry;
        }
    }

    [Constructible]
    public MyCustomKey() : base(0x0)
    {
        ItemID = 0x1234;
        Name = "My Custom Key";
    }

    protected override ItemStore GenerateItemStore()
    {
        ItemStore store = base.GenerateItemStore();
        store.Label = "My Custom Storage";
        store.Dynamic = false;
        store.OfferDeeds = false;
        return store;
    }

    public MyCustomKey(Serial serial) : base(serial) { }

    public override void Serialize(IGenericWriter writer)
    {
        base.Serialize(writer);
        writer.Write(0); // version
    }

    public override void Deserialize(IGenericReader reader)
    {
        base.Deserialize(reader);
        int version = reader.ReadInt();
    }
}
```

---

## Credits

- Original Universal Storage Keys system design
- ModernUO compatibility updates

## Version History

### v2.0.6 - ModernUO Edition
- Full compatibility with ModernUO .NET 10
- Updated all serialization to use interface types
- Fixed type references for ModernUO API changes
- Updated ObjectPropertyList handling
- **NEW**: Vendor Stone system for selling keys to players
- **NEW**: Admin panel for configuring key prices and availability
- Code cleanup and optimization

## License

This script is provided as-is for use with ModernUO-based Ultima Online servers.
