using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    /// <summary>
    /// Player-facing gump for purchasing storage keys - Modern Style
    /// </summary>
    public class KeyVendorGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _page;
        private const int EntriesPerPage = 8;

        // Modern Color Palette
        private const int HeaderColor = 0x00BFFF;    // Deep Sky Blue
        private const int TitleColor = 0xFFFFFF;     // White
        private const int LabelColor = 0xE0E0E0;     // Light Gray
        private const int PriceColor = 0x7CFC00;     // Lawn Green
        private const int AccentColor = 0xFFD700;    // Gold
        private const int SubtleColor = 0x808080;    // Gray

        public KeyVendorGump(Mobile from, KeyVendorStone stone, int page) : base(50, 50)
        {
            _from = from;
            _stone = stone;
            _page = page;

            from.CloseGump<KeyVendorGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            int width = 520;
            int height = 480;

            AddPage(0);

            // Main background with dark theme
            AddBackground(0, 0, width, height, 9200);
            
            // Inner panel
            AddBackground(10, 10, width - 20, height - 20, 9270);
            
            // Header area
            AddBackground(20, 20, width - 40, 50, 9200);
            AddAlphaRegion(22, 22, width - 44, 46);
            
            // Title with icon
            AddImage(30, 28, 0x1F14); // Key icon
            AddHtml(70, 32, width - 100, 25, Center(Color($"⚿ {_stone.VendorName} ⚿", HeaderColor)), false, false);

            // Subtitle
            AddHtml(20, 75, width - 40, 20, Center(Color("Select a storage key to purchase", SubtleColor)), false, false);

            // Column headers with modern styling
            int y = 100;
            AddBackground(20, y, width - 40, 25, 9200);
            AddHtml(50, y + 4, 200, 20, Color("Key Type", AccentColor), false, false);
            AddHtml(280, y + 4, 100, 20, Color("Price", AccentColor), false, false);
            AddHtml(400, y + 4, 80, 20, Color("Action", AccentColor), false, false);

            // Get enabled entries only for players
            List<int> enabledIndices = new List<int>();
            for (int i = 0; i < _stone.Entries.Count; i++)
            {
                if (_stone.Entries[i].Enabled)
                    enabledIndices.Add(i);
            }

            int totalPages = (enabledIndices.Count + EntriesPerPage - 1) / EntriesPerPage;
            if (totalPages == 0) totalPages = 1;

            int startIndex = _page * EntriesPerPage;
            int endIndex = Math.Min(startIndex + EntriesPerPage, enabledIndices.Count);

            y += 35;

            // Display entries with alternating backgrounds
            for (int i = startIndex; i < endIndex; i++)
            {
                int entryIndex = enabledIndices[i];
                KeyVendorEntry entry = _stone.Entries[entryIndex];

                // Alternating row background
                if ((i - startIndex) % 2 == 0)
                {
                    AddAlphaRegion(22, y - 2, width - 44, 36);
                }

                // Key icon
                AddItem(30, y - 2, entry.ItemId, entry.Hue);

                // Key name
                AddHtml(75, y + 5, 190, 20, Color(entry.Name, LabelColor), false, false);

                // Price with gold icon
                AddImage(275, y + 2, 0x0E79); // Gold pile icon
                AddHtml(300, y + 5, 90, 20, Color($"{entry.Price:N0}", PriceColor), false, false);

                // Modern purchase button
                AddButton(410, y + 2, 4029, 4031, 100 + entryIndex, GumpButtonType.Reply, 0);
                AddHtml(445, y + 5, 50, 20, Color("Buy", LabelColor), false, false);

                y += 38;
            }

            // Footer section
            y = height - 90;
            AddBackground(20, y, width - 40, 2, 9274);

            y += 15;

            // Page navigation with modern styling
            AddHtml(width / 2 - 60, y, 120, 20, Center(Color($"Page {_page + 1} of {totalPages}", SubtleColor)), false, false);

            y += 25;

            // Navigation buttons
            if (_page > 0)
            {
                AddButton(100, y, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddHtml(135, y + 2, 80, 20, Color("◄ Previous", LabelColor), false, false);
            }

            if (_page < totalPages - 1)
            {
                AddButton(width - 150, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtml(width - 115, y + 2, 80, 20, Color("Next ►", LabelColor), false, false);
            }

            // Close button
            AddButton(width / 2 - 40, height - 45, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtml(width / 2 - 5, height - 43, 50, 20, Color("Close", LabelColor), false, false);

            // Gold balance display
            int balance = Mobiles.Banker.GetBalance(_from);
            AddHtml(20, height - 45, 200, 20, Color($"💰 Your Gold: {balance:N0}", AccentColor), false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || _stone == null || _stone.Deleted)
                return;

            if (!from.InRange(_stone.GetWorldLocation(), 3))
            {
                from.SendMessage("You are too far away.");
                return;
            }

            int buttonId = info.ButtonID;

            switch (buttonId)
            {
                case 0: // Close
                    break;

                case 1: // Previous page
                    from.SendGump(new KeyVendorGump(from, _stone, _page - 1));
                    break;

                case 2: // Next page
                    from.SendGump(new KeyVendorGump(from, _stone, _page + 1));
                    break;

                default: // Purchase buttons (100+)
                    if (buttonId >= 100)
                    {
                        int entryIndex = buttonId - 100;
                        KeyVendorEntry entry = _stone.GetEntry(entryIndex);

                        if (entry != null && entry.Enabled)
                        {
                            // Show confirmation gump
                            from.SendGump(new KeyVendorConfirmGump(from, _stone, entryIndex, _page));
                        }
                        else
                        {
                            from.SendMessage("That key is not available.");
                            from.SendGump(new KeyVendorGump(from, _stone, _page));
                        }
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
    /// Modern confirmation gump for purchasing a key
    /// </summary>
    public class KeyVendorConfirmGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _entryIndex;
        private int _returnPage;

        public KeyVendorConfirmGump(Mobile from, KeyVendorStone stone, int entryIndex, int returnPage) : base(150, 150)
        {
            _from = from;
            _stone = stone;
            _entryIndex = entryIndex;
            _returnPage = returnPage;

            from.CloseGump<KeyVendorConfirmGump>();

            BuildGump();
        }

        private void BuildGump()
        {
            KeyVendorEntry entry = _stone.GetEntry(_entryIndex);
            if (entry == null)
                return;

            int width = 320;
            int height = 220;

            AddPage(0);

            // Modern dark background
            AddBackground(0, 0, width, height, 9200);
            AddBackground(10, 10, width - 20, height - 20, 9270);

            // Header
            AddBackground(20, 20, width - 40, 35, 9200);
            AddAlphaRegion(22, 22, width - 44, 31);
            AddHtml(20, 28, width - 40, 25, "<CENTER><BASEFONT COLOR=#00BFFF>⚿ Confirm Purchase ⚿</BASEFONT></CENTER>", false, false);

            // Key display area
            AddBackground(20, 65, width - 40, 70, 9350);
            AddItem(35, 75, entry.ItemId, entry.Hue);
            AddHtml(85, 78, 180, 20, $"<BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT>", false, false);
            
            AddImage(85, 100, 0x0E79); // Gold icon
            AddHtml(110, 103, 150, 20, $"<BASEFONT COLOR=#7CFC00>{entry.Price:N0} gold</BASEFONT>", false, false);

            // Confirmation message
            AddHtml(20, 145, width - 40, 25, "<CENTER><BASEFONT COLOR=#E0E0E0>Proceed with purchase?</BASEFONT></CENTER>", false, false);

            // Modern buttons
            AddButton(50, height - 50, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddHtml(85, height - 48, 60, 20, "<BASEFONT COLOR=#7CFC00>✓ Yes</BASEFONT>", false, false);

            AddButton(180, height - 50, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtml(215, height - 48, 60, 20, "<BASEFONT COLOR=#FF6B6B>✗ No</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || _stone == null || _stone.Deleted)
                return;

            if (!from.InRange(_stone.GetWorldLocation(), 3))
            {
                from.SendMessage("You are too far away.");
                return;
            }

            if (info.ButtonID == 1) // Confirm purchase
            {
                KeyVendorEntry entry = _stone.GetEntry(_entryIndex);
                if (entry != null)
                {
                    _stone.TryPurchase(from, entry);
                }
            }

            // Return to main gump
            from.SendGump(new KeyVendorGump(from, _stone, _returnPage));
        }
    }
}
