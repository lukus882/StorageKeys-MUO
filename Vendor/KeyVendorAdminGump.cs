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
    /// Admin gump for managing the key vendor stone - Modern Style
    /// </summary>
    public class KeyVendorAdminGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _page;
        private const int EntriesPerPage = 8;

        // Modern Color Palette
        private const int HeaderColor = 0xFF6B6B;    // Coral Red (Admin)
        private const int TitleColor = 0xFFFFFF;     // White
        private const int LabelColor = 0xE0E0E0;    // Light Gray
        private const int PriceColor = 0x7CFC00;     // Lawn Green
        private const int EnabledColor = 0x00FF7F;   // Spring Green
        private const int DisabledColor = 0xFF6B6B;  // Coral Red
        private const int AccentColor = 0xFFD700;    // Gold
        private const int SubtleColor = 0x808080;    // Gray
        private const int ButtonColor = 0x00BFFF;    // Deep Sky Blue

        public KeyVendorAdminGump(Mobile from, KeyVendorStone stone, int page) : base(30, 30)
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
            int width = 650;
            int height = 550;

            AddPage(0);

            // Main background - dark theme
            AddBackground(0, 0, width, height, 9200);
            AddBackground(10, 10, width - 20, height - 20, 9270);

            // Header area with admin styling
            AddBackground(20, 20, width - 40, 50, 9200);
            AddAlphaRegion(22, 22, width - 44, 46);
            
            // Admin badge and title
            AddImage(30, 28, 0x15A9); // Shield icon
            AddHtml(70, 25, width - 100, 20, Center(Color($"⚙ {_stone.VendorName} ⚙", HeaderColor)), false, false);
            AddHtml(70, 45, width - 100, 20, Center(Color("Administrator Control Panel", SubtleColor)), false, false);

            // Column headers
            int y = 80;
            AddBackground(20, y, width - 40, 28, 9200);
            AddHtml(60, y + 5, 140, 20, Color("Key Name", AccentColor), false, false);
            AddHtml(210, y + 5, 80, 20, Color("Price", AccentColor), false, false);
            AddHtml(300, y + 5, 70, 20, Color("Status", AccentColor), false, false);
            AddHtml(380, y + 5, 60, 20, Color("Toggle", AccentColor), false, false);
            AddHtml(450, y + 5, 70, 20, Color("Edit $", AccentColor), false, false);
            AddHtml(530, y + 5, 80, 20, Color("Test", AccentColor), false, false);

            int totalPages = (_stone.Entries.Count + EntriesPerPage - 1) / EntriesPerPage;
            if (totalPages == 0) totalPages = 1;

            int startIndex = _page * EntriesPerPage;
            int endIndex = Math.Min(startIndex + EntriesPerPage, _stone.Entries.Count);

            y += 38;

            // Display all entries with modern styling
            for (int i = startIndex; i < endIndex; i++)
            {
                KeyVendorEntry entry = _stone.Entries[i];

                // Alternating row background
                if ((i - startIndex) % 2 == 0)
                {
                    AddAlphaRegion(22, y - 3, width - 44, 40);
                }

                // Key icon
                AddItem(25, y - 3, entry.ItemId, entry.Hue);

                // Key name (colored based on status)
                int nameColor = entry.Enabled ? LabelColor : SubtleColor;
                AddHtml(65, y + 5, 135, 20, Color(TruncateName(entry.Name, 18), nameColor), false, false);

                // Price
                AddHtml(210, y + 5, 80, 20, Color($"{entry.Price:N0}", PriceColor), false, false);

                // Status indicator with colored badge
                string statusText = entry.Enabled ? "● ON" : "● OFF";
                int statusColor = entry.Enabled ? EnabledColor : DisabledColor;
                AddHtml(300, y + 5, 70, 20, Color(statusText, statusColor), false, false);

                // Toggle button
                AddButton(395, y + 2, entry.Enabled ? 4017 : 4005, entry.Enabled ? 4019 : 4007, 200 + i, GumpButtonType.Reply, 0);

                // Edit price button
                AddButton(465, y + 2, 4011, 4013, 300 + i, GumpButtonType.Reply, 0);

                // Test buy button
                AddButton(545, y + 2, 4029, 4031, 400 + i, GumpButtonType.Reply, 0);
                AddHtml(580, y + 5, 40, 20, Color("Free", ButtonColor), false, false);

                y += 42;
            }

            // Footer section
            y = height - 145;
            AddBackground(20, y, width - 40, 2, 9274);

            y += 15;

            // Page navigation
            AddHtml(width / 2 - 60, y, 120, 20, Center(Color($"Page {_page + 1} of {totalPages}", SubtleColor)), false, false);

            if (_page > 0)
            {
                AddButton(80, y - 2, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddHtml(115, y, 80, 20, Color("◄ Prev", LabelColor), false, false);
            }

            if (_page < totalPages - 1)
            {
                AddButton(width - 130, y - 2, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtml(width - 95, y, 80, 20, Color("Next ►", LabelColor), false, false);
            }

            // Admin action buttons - modern card style
            y += 35;
            AddBackground(20, y, width - 40, 60, 9200);
            AddAlphaRegion(22, y + 2, width - 44, 56);

            int btnY = y + 18;

            // View as player
            AddButton(35, btnY, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddHtml(70, btnY + 2, 90, 20, Color("👁 Preview", ButtonColor), false, false);

            // Enable all
            AddButton(170, btnY, 4005, 4007, 11, GumpButtonType.Reply, 0);
            AddHtml(205, btnY + 2, 80, 20, Color("✓ All On", EnabledColor), false, false);

            // Disable all
            AddButton(295, btnY, 4017, 4019, 12, GumpButtonType.Reply, 0);
            AddHtml(330, btnY + 2, 80, 20, Color("✗ All Off", DisabledColor), false, false);

            // Set all prices
            AddButton(420, btnY, 4011, 4013, 13, GumpButtonType.Reply, 0);
            AddHtml(455, btnY + 2, 100, 20, Color("💰 Bulk Price", AccentColor), false, false);

            // Close button
            AddButton(width / 2 - 40, height - 40, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtml(width / 2 - 5, height - 38, 60, 20, Color("Close", LabelColor), false, false);
        }

        private string TruncateName(string name, int maxLength)
        {
            if (name.Length <= maxLength)
                return name;
            return name.Substring(0, maxLength - 2) + "..";
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
                    from.SendMessage(0x40, "All keys have been enabled.");
                    from.SendGump(new KeyVendorAdminGump(from, _stone, _page));
                    break;

                case 12: // Disable all
                    for (int i = 0; i < _stone.Entries.Count; i++)
                        _stone.SetEnabled(i, false);
                    from.SendMessage(0x22, "All keys have been disabled.");
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
                                from.SendMessage(0x35, $"[TEST] Free {entry.Name} added to your backpack.");
                            }
                            else
                            {
                                from.SendMessage(0x22, "Failed to create the key. Check the key type.");
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
                            int msgColor = entry.Enabled ? 0x40 : 0x22;
                            from.SendMessage(msgColor, $"{entry.Name} has been {(entry.Enabled ? "enabled" : "disabled")}.");
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
    /// Modern gump for editing the price of a single key
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

            int width = 340;
            int height = 230;

            AddPage(0);

            // Modern dark background
            AddBackground(0, 0, width, height, 9200);
            AddBackground(10, 10, width - 20, height - 20, 9270);

            // Header
            AddBackground(20, 20, width - 40, 35, 9200);
            AddAlphaRegion(22, 22, width - 44, 31);
            AddHtml(20, 28, width - 40, 25, "<CENTER><BASEFONT COLOR=#FFD700>💰 Edit Price</BASEFONT></CENTER>", false, false);

            // Key display
            AddBackground(20, 65, width - 40, 55, 9350);
            AddItem(35, 72, entry.ItemId, entry.Hue);
            AddHtml(85, 78, 200, 20, $"<BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT>", false, false);
            AddHtml(85, 98, 200, 20, $"<BASEFONT COLOR=#808080>Current: <BASEFONT COLOR=#7CFC00>{entry.Price:N0}</BASEFONT> gp</BASEFONT>", false, false);

            // New price input
            AddHtml(30, 135, 100, 20, "<BASEFONT COLOR=#E0E0E0>New Price:</BASEFONT>", false, false);
            AddBackground(130, 130, 140, 28, 9350);
            AddTextEntry(138, 135, 125, 20, 0x481, 0, entry.Price.ToString());

            // Buttons
            AddButton(60, height - 50, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddHtml(95, height - 48, 60, 20, "<BASEFONT COLOR=#7CFC00>Save</BASEFONT>", false, false);

            AddButton(190, height - 50, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtml(225, height - 48, 60, 20, "<BASEFONT COLOR=#FF6B6B>Cancel</BASEFONT>", false, false);
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
                    from.SendMessage(0x40, $"Price for {entry?.Name ?? "key"} set to {newPrice:N0} gold.");
                }
                else
                {
                    from.SendMessage(0x22, "Invalid price entered.");
                }
            }

            from.SendGump(new KeyVendorAdminGump(from, _stone, _returnPage));
        }
    }

    /// <summary>
    /// Modern gump for setting all prices at once
    /// </summary>
    public class KeyVendorSetAllPricesGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _returnPage;

        public KeyVendorSetAllPricesGump(Mobile from, KeyVendorStone stone, int returnPage) : base(150, 120)
        {
            _from = from;
            _stone = stone;
            _returnPage = returnPage;

            from.CloseGump<KeyVendorSetAllPricesGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            int width = 380;
            int height = 300;

            AddPage(0);

            // Modern dark background
            AddBackground(0, 0, width, height, 9200);
            AddBackground(10, 10, width - 20, height - 20, 9270);

            // Header
            AddBackground(20, 20, width - 40, 35, 9200);
            AddAlphaRegion(22, 22, width - 44, 31);
            AddHtml(20, 28, width - 40, 25, "<CENTER><BASEFONT COLOR=#FFD700>💰 Bulk Price Adjustment</BASEFONT></CENTER>", false, false);

            // Description
            AddHtml(30, 65, width - 60, 25, "<BASEFONT COLOR=#E0E0E0>Choose how to modify all key prices:</BASEFONT>", false, false);

            int y = 100;

            // Option 1: Fixed price
            AddRadio(30, y, 9727, 9730, true, 1);
            AddHtml(60, y + 3, 150, 20, "<BASEFONT COLOR=#00BFFF>Set fixed price:</BASEFONT>", false, false);
            AddBackground(220, y - 2, 120, 26, 9350);
            AddTextEntry(228, y + 2, 105, 20, 0x481, 0, "10000");

            y += 40;

            // Option 2: Percentage
            AddRadio(30, y, 9727, 9730, false, 2);
            AddHtml(60, y + 3, 150, 20, "<BASEFONT COLOR=#00BFFF>Multiply by %:</BASEFONT>", false, false);
            AddBackground(220, y - 2, 120, 26, 9350);
            AddTextEntry(228, y + 2, 105, 20, 0x481, 1, "100");

            y += 40;

            // Option 3: Add/subtract
            AddRadio(30, y, 9727, 9730, false, 3);
            AddHtml(60, y + 3, 150, 20, "<BASEFONT COLOR=#00BFFF>Add/Subtract:</BASEFONT>", false, false);
            AddBackground(220, y - 2, 120, 26, 9350);
            AddTextEntry(228, y + 2, 105, 20, 0x481, 2, "0");

            // Hint text
            AddHtml(30, y + 35, width - 60, 20, "<BASEFONT COLOR=#808080>Use negative numbers to subtract.</BASEFONT>", false, false);

            // Buttons
            AddButton(90, height - 50, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddHtml(125, height - 48, 60, 20, "<BASEFONT COLOR=#7CFC00>Apply</BASEFONT>", false, false);

            AddButton(220, height - 50, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtml(255, height - 48, 60, 20, "<BASEFONT COLOR=#FF6B6B>Cancel</BASEFONT>", false, false);
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
                            from.SendMessage(0x40, $"All prices set to {fixedPrice:N0} gold.");
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
                            from.SendMessage(0x40, $"All prices multiplied by {percent}%.");
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
                            from.SendMessage(0x40, $"All prices adjusted by {addAmount:N0} gold.");
                        }
                        break;
                }
            }

            from.SendGump(new KeyVendorAdminGump(from, _stone, _returnPage));
        }
    }
}
