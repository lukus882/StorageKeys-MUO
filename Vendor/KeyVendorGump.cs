using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    /// <summary>
    /// Player-facing gump for purchasing storage keys
    /// </summary>
    public class KeyVendorGump : Gump
    {
        private Mobile _from;
        private KeyVendorStone _stone;
        private int _page;
        private const int EntriesPerPage = 10;

        // Colors
        private const int TitleColor = 0xFFFFFF;
        private const int LabelColor = 0xFFFFFF;
        private const int PriceColor = 0x00FF00;
        private const int DisabledColor = 0x808080;

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
            int width = 500;
            int height = 450;

            AddPage(0);

            // Background
            AddBackground(0, 0, width, height, 9270);

            // Title bar
            AddBackground(10, 10, width - 20, 30, 9270);
            AddHtml(20, 17, width - 40, 20, Center(Color(_stone.VendorName, TitleColor)), false, false);

            // Column headers
            int y = 50;
            AddHtml(30, y, 200, 20, Color("Key Name", LabelColor), false, false);
            AddHtml(250, y, 100, 20, Color("Price", LabelColor), false, false);
            AddHtml(380, y, 80, 20, Color("Purchase", LabelColor), false, false);

            // Divider
            y += 25;
            AddImageTiled(20, y, width - 40, 2, 9274);

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

            y += 10;

            // Display entries
            for (int i = startIndex; i < endIndex; i++)
            {
                int entryIndex = enabledIndices[i];
                KeyVendorEntry entry = _stone.Entries[entryIndex];

                // Key icon
                AddItem(30, y - 5, entry.ItemId, entry.Hue);

                // Key name
                AddHtml(70, y, 170, 20, Color(entry.Name, LabelColor), false, false);

                // Price
                AddHtml(250, y, 100, 20, Color($"{entry.Price:N0} gp", PriceColor), false, false);

                // Purchase button
                AddButton(400, y, 4005, 4007, 100 + entryIndex, GumpButtonType.Reply, 0);

                y += 30;
            }

            // Footer with page navigation
            y = height - 50;
            AddImageTiled(20, y - 10, width - 40, 2, 9274);

            // Page info
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

            // Close button
            AddButton(width / 2 - 30, height - 30, 4017, 4019, 0, GumpButtonType.Reply, 0);
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
    /// Confirmation gump for purchasing a key
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

            int width = 300;
            int height = 180;

            AddPage(0);

            AddBackground(0, 0, width, height, 9270);

            AddHtml(20, 20, width - 40, 20, "<CENTER><BASEFONT COLOR=#FFFFFF>Confirm Purchase</BASEFONT></CENTER>", false, false);

            AddImageTiled(20, 45, width - 40, 2, 9274);

            // Key info
            AddItem(30, 60, entry.ItemId, entry.Hue);
            AddHtml(70, 65, 200, 20, $"<BASEFONT COLOR=#FFFFFF>{entry.Name}</BASEFONT>", false, false);

            AddHtml(70, 90, 200, 20, $"<BASEFONT COLOR=#00FF00>Price: {entry.Price:N0} gold</BASEFONT>", false, false);

            AddHtml(20, 115, width - 40, 20, "<CENTER><BASEFONT COLOR=#FFFFFF>Are you sure you want to purchase this key?</BASEFONT></CENTER>", false, false);

            // Confirm button
            AddButton(60, height - 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(95, height - 38, 50, 20, "<BASEFONT COLOR=#00FF00>Yes</BASEFONT>", false, false);

            // Cancel button
            AddButton(180, height - 40, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(215, height - 38, 50, 20, "<BASEFONT COLOR=#FF0000>No</BASEFONT>", false, false);
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
