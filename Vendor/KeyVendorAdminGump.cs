using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
    /// <summary>
    /// Admin gump for managing the key vendor stone
    /// </summary>
    public class KeyVendorAdminGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _page;
        private const int EntriesPerPage = 8;

        // Colors
        private const int TitleColor = 0xFFFFFF;
        private const int LabelColor = 0xFFFFFF;
        private const int PriceColor = 0x00FF00;
        private const int EnabledColor = 0x00FF00;
        private const int DisabledColor = 0xFF0000;
        private const int AdminColor = 0xFFD700;

        public KeyVendorAdminGump(Mobile from, KeyVendorStone stone, int page) : base(50, 50)
        {
            _from = from;
            _stone = stone;
            _page = page;

            from.CloseGump<KeyVendorAdminGump>();
            from.CloseGump<KeyVendorGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            int width = 600;
            int height = 500;

            AddPage(0);

            // Background
            AddBackground(0, 0, width, height, 9270);

            // Title bar
            AddBackground(10, 10, width - 20, 30, 9270);
            AddHtml(20, 17, width - 40, 20, Center(Color($"{_stone.VendorName} - Admin Panel", AdminColor)), false, false);

            // Column headers
            int y = 50;
            AddHtml(30, y, 150, 20, Color("Key Name", LabelColor), false, false);
            AddHtml(200, y, 80, 20, Color("Price", LabelColor), false, false);
            AddHtml(300, y, 60, 20, Color("Status", LabelColor), false, false);
            AddHtml(380, y, 60, 20, Color("Toggle", LabelColor), false, false);
            AddHtml(450, y, 80, 20, Color("Edit Price", LabelColor), false, false);
            AddHtml(530, y, 50, 20, Color("Test", LabelColor), false, false);

            // Divider
            y += 25;
            AddImageTiled(20, y, width - 40, 2, 9274);

            int totalPages = (_stone.Entries.Count + EntriesPerPage - 1) / EntriesPerPage;
            if (totalPages == 0) totalPages = 1;

            int startIndex = _page * EntriesPerPage;
            int endIndex = Math.Min(startIndex + EntriesPerPage, _stone.Entries.Count);

            y += 10;

            // Display all entries (including disabled ones for admin)
            for (int i = startIndex; i < endIndex; i++)
            {
                KeyVendorEntry entry = _stone.Entries[i];

                // Key icon
                AddItem(25, y - 5, entry.ItemId, entry.Hue);

                // Key name
                int nameColor = entry.Enabled ? LabelColor : DisabledColor;
                AddHtml(60, y, 130, 20, Color(entry.Name, nameColor), false, false);

                // Price
                AddHtml(200, y, 80, 20, Color($"{entry.Price:N0}", PriceColor), false, false);

                // Status
                string status = entry.Enabled ? "Enabled" : "Disabled";
                int statusColor = entry.Enabled ? EnabledColor : DisabledColor;
                AddHtml(300, y, 60, 20, Color(status, statusColor), false, false);

                // Toggle button
                AddButton(395, y, entry.Enabled ? 4017 : 4005, entry.Enabled ? 4019 : 4007, 200 + i, GumpButtonType.Reply, 0);

                // Edit price button
                AddButton(470, y, 4011, 4013, 300 + i, GumpButtonType.Reply, 0);

                // Test buy button (gives free key to admin)
                AddButton(545, y, 4005, 4007, 400 + i, GumpButtonType.Reply, 0);

                y += 35;
            }

            // Footer
            y = height - 100;
            AddImageTiled(20, y, width - 40, 2, 9274);

            y += 10;

            // Page navigation
            AddHtml(width / 2 - 50, y, 100, 20, Center(Color($"Page {_page + 1} of {totalPages}", LabelColor)), false, false);

            // Previous page button
            if (_page > 0)
            {
                AddButton(30, y, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddHtml(65, y, 60, 20, Color("Previous", LabelColor), false, false);
            }

            // Next page button
            if (_page < totalPages - 1)
            {
                AddButton(width - 90, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtml(width - 140, y, 60, 20, Color("Next", LabelColor), false, false);
            }

            y += 30;

            // Admin actions
            // View as player button
            AddButton(30, y, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddHtml(65, y, 120, 20, Color("View as Player", LabelColor), false, false);

            // Enable all button
            AddButton(200, y, 4005, 4007, 11, GumpButtonType.Reply, 0);
            AddHtml(235, y, 80, 20, Color("Enable All", EnabledColor), false, false);

            // Disable all button
            AddButton(330, y, 4017, 4019, 12, GumpButtonType.Reply, 0);
            AddHtml(365, y, 80, 20, Color("Disable All", DisabledColor), false, false);

            // Set all prices button
            AddButton(460, y, 4011, 4013, 13, GumpButtonType.Reply, 0);
            AddHtml(495, y, 90, 20, Color("Set All Prices", LabelColor), false, false);

            // Close button
            AddButton(width / 2 - 30, height - 35, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || _stone == null || _stone.Deleted)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster)
            {
                from.SendMessage("You do not have permission to access the admin panel.");
                return;
            }

            int buttonId = info.ButtonID;

            switch (buttonId)
            {
                case 0: // Close
                    break;

                case 1: // Previous page
                    from.SendGump(new KeyVendorAdminGump(from, _stone, _page - 1));
                    break;

                case 2: // Next page
                    from.SendGump(new KeyVendorAdminGump(from, _stone, _page + 1));
                    break;

                case 10: // View as player
                    from.SendGump(new KeyVendorPlayerViewGump(from, _stone, 0));
                    break;

                case 11: // Enable all
                    for (int i = 0; i < _stone.Entries.Count; i++)
                        _stone.SetEnabled(i, true);
                    from.SendMessage("All keys have been enabled.");
                    from.SendGump(new KeyVendorAdminGump(from, _stone, _page));
                    break;

                case 12: // Disable all
                    for (int i = 0; i < _stone.Entries.Count; i++)
                        _stone.SetEnabled(i, false);
                    from.SendMessage("All keys have been disabled.");
                    from.SendGump(new KeyVendorAdminGump(from, _stone, _page));
                    break;

                case 13: // Set all prices
                    from.SendGump(new KeyVendorSetAllPricesGump(from, _stone, _page));
                    break;

                default:
                    if (buttonId >= 400) // Test buy
                    {
                        int entryIndex = buttonId - 400;
                        KeyVendorEntry entry = _stone.GetEntry(entryIndex);
                        if (entry != null)
                        {
                            Item key = entry.CreateKey();
                            if (key != null)
                            {
                                from.AddToBackpack(key);
                                from.SendMessage($"A free {entry.Name} has been added to your backpack for testing.");
                            }
                            else
                            {
                                from.SendMessage("Failed to create the key. Check the key type.");
                            }
                        }
                        from.SendGump(new KeyVendorAdminGump(from, _stone, _page));
                    }
                    else if (buttonId >= 300) // Edit price
                    {
                        int entryIndex = buttonId - 300;
                        from.SendGump(new KeyVendorEditPriceGump(from, _stone, entryIndex, _page));
                    }
                    else if (buttonId >= 200) // Toggle enabled
                    {
                        int entryIndex = buttonId - 200;
                        KeyVendorEntry entry = _stone.GetEntry(entryIndex);
                        if (entry != null)
                        {
                            _stone.SetEnabled(entryIndex, !entry.Enabled);
                            from.SendMessage($"{entry.Name} has been {(entry.Enabled ? "enabled" : "disabled")}.");
                        }
                        from.SendGump(new KeyVendorAdminGump(from, _stone, _page));
                    }
                    break;
            }
        }

        private string Color(string text, int color)
        {
            return $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }
    }

    /// <summary>
    /// Player view gump for admins to preview the player experience
    /// </summary>
    public class KeyVendorPlayerViewGump : KeyVendorGump
    {
        private KeyVendorStone _stone;

        public KeyVendorPlayerViewGump(Mobile from, KeyVendorStone stone, int page) : base(from, stone, page)
        {
            _stone = stone;
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0) // Close - return to admin gump
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendGump(new KeyVendorAdminGump(from, _stone, 0));
                    return;
                }
            }

            base.OnResponse(sender, info);
        }
    }

    /// <summary>
    /// Gump for editing the price of a single key
    /// </summary>
    public class KeyVendorEditPriceGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _entryIndex;
        private int _returnPage;

        public KeyVendorEditPriceGump(Mobile from, KeyVendorStone stone, int entryIndex, int returnPage) : base(150, 150)
        {
            _from = from;
            _stone = stone;
            _entryIndex = entryIndex;
            _returnPage = returnPage;

            from.CloseGump<KeyVendorEditPriceGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            KeyVendorEntry entry = _stone.GetEntry(_entryIndex);
            if (entry == null)
                return;

            int width = 320;
            int height = 200;

            AddPage(0);

            AddBackground(0, 0, width, height, 9270);

            AddHtml(20, 20, width - 40, 20, "<CENTER><BASEFONT COLOR=#FFD700>Edit Price</BASEFONT></CENTER>", false, false);

            AddImageTiled(20, 45, width - 40, 2, 9274);

            // Key info
            AddItem(30, 60, entry.ItemId, entry.Hue);
            AddHtml(70, 65, 200, 20, $"<BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT>", false, false);

            AddHtml(30, 100, 100, 20, "<BASEFONT COLOR=#FFFFFF>Current Price:</BASEFONT>", false, false);
            AddHtml(140, 100, 100, 20, $"<BASEFONT COLOR=#00FF00>{entry.Price:N0}</BASEFONT>", false, false);

            AddHtml(30, 130, 100, 20, "<BASEFONT COLOR=#FFFFFF>New Price:</BASEFONT>", false, false);
            AddBackground(130, 127, 120, 25, 9350);
            AddTextEntry(135, 130, 110, 20, 0, 0, entry.Price.ToString());

            // Save button
            AddButton(60, height - 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(95, height - 38, 50, 20, "<BASEFONT COLOR=#00FF00>Save</BASEFONT>", false, false);

            // Cancel button
            AddButton(180, height - 40, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(215, height - 38, 60, 20, "<BASEFONT COLOR=#FF0000>Cancel</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || _stone == null || _stone.Deleted)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            if (info.ButtonID == 1) // Save
            {
                string priceText = info.GetTextEntry(0);
                if (!string.IsNullOrEmpty(priceText) && int.TryParse(priceText, out int newPrice))
                {
                    _stone.SetPrice(_entryIndex, newPrice);
                    KeyVendorEntry entry = _stone.GetEntry(_entryIndex);
                    from.SendMessage($"Price for {entry?.Name ?? "key"} has been set to {newPrice:N0} gold.");
                }
                else
                {
                    from.SendMessage("Invalid price entered.");
                }
            }

            from.SendGump(new KeyVendorAdminGump(from, _stone, _returnPage));
        }
    }

    /// <summary>
    /// Gump for setting all prices at once
    /// </summary>
    public class KeyVendorSetAllPricesGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _returnPage;

        public KeyVendorSetAllPricesGump(Mobile from, KeyVendorStone stone, int returnPage) : base(150, 150)
        {
            _from = from;
            _stone = stone;
            _returnPage = returnPage;

            from.CloseGump<KeyVendorSetAllPricesGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            int width = 350;
            int height = 250;

            AddPage(0);

            AddBackground(0, 0, width, height, 9270);

            AddHtml(20, 20, width - 40, 20, "<CENTER><BASEFONT COLOR=#FFD700>Set All Prices</BASEFONT></CENTER>", false, false);

            AddImageTiled(20, 45, width - 40, 2, 9274);

            // Options
            AddHtml(30, 60, 280, 40, "<BASEFONT COLOR=#FFFFFF>Choose how to modify all key prices:</BASEFONT>", false, false);

            // Set fixed price
            AddRadio(30, 100, 9727, 9730, true, 1);
            AddHtml(60, 103, 150, 20, "<BASEFONT COLOR=#FFFFFF>Set all to fixed price:</BASEFONT>", false, false);
            AddBackground(210, 100, 100, 25, 9350);
            AddTextEntry(215, 103, 90, 20, 0, 0, "10000");

            // Multiply by percentage
            AddRadio(30, 135, 9727, 9730, false, 2);
            AddHtml(60, 138, 150, 20, "<BASEFONT COLOR=#FFFFFF>Multiply prices by %:</BASEFONT>", false, false);
            AddBackground(210, 135, 100, 25, 9350);
            AddTextEntry(215, 138, 90, 20, 0, 1, "100");

            // Add/subtract amount
            AddRadio(30, 170, 9727, 9730, false, 3);
            AddHtml(60, 173, 150, 20, "<BASEFONT COLOR=#FFFFFF>Add/Subtract amount:</BASEFONT>", false, false);
            AddBackground(210, 170, 100, 25, 9350);
            AddTextEntry(215, 173, 90, 20, 0, 2, "0");

            // Apply button
            AddButton(80, height - 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(115, height - 38, 50, 20, "<BASEFONT COLOR=#00FF00>Apply</BASEFONT>", false, false);

            // Cancel button
            AddButton(200, height - 40, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(235, height - 38, 60, 20, "<BASEFONT COLOR=#FF0000>Cancel</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || _stone == null || _stone.Deleted)
                return;

            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            if (info.ButtonID == 1) // Apply
            {
                int selectedOption = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (info.IsSwitched(i))
                    {
                        selectedOption = i;
                        break;
                    }
                }

                switch (selectedOption)
                {
                    case 1: // Fixed price
                        string fixedText = info.GetTextEntry(0);
                        if (!string.IsNullOrEmpty(fixedText) && int.TryParse(fixedText, out int fixedPrice))
                        {
                            for (int i = 0; i < _stone.Entries.Count; i++)
                                _stone.SetPrice(i, fixedPrice);
                            from.SendMessage($"All prices have been set to {fixedPrice:N0} gold.");
                        }
                        break;

                    case 2: // Multiply by percentage
                        string percentText = info.GetTextEntry(1);
                        if (!string.IsNullOrEmpty(percentText) && int.TryParse(percentText, out int percent))
                        {
                            for (int i = 0; i < _stone.Entries.Count; i++)
                            {
                                int newPrice = (int)(_stone.Entries[i].Price * (percent / 100.0));
                                _stone.SetPrice(i, newPrice);
                            }
                            from.SendMessage($"All prices have been multiplied by {percent}%.");
                        }
                        break;

                    case 3: // Add/subtract
                        string addText = info.GetTextEntry(2);
                        if (!string.IsNullOrEmpty(addText) && int.TryParse(addText, out int addAmount))
                        {
                            for (int i = 0; i < _stone.Entries.Count; i++)
                            {
                                int newPrice = _stone.Entries[i].Price + addAmount;
                                _stone.SetPrice(i, newPrice);
                            }
                            from.SendMessage($"All prices have been adjusted by {addAmount:N0} gold.");
                        }
                        break;
                }
            }

            from.SendGump(new KeyVendorAdminGump(from, _stone, _returnPage));
        }
    }
}
