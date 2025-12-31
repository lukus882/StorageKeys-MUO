//CliLocDAO.cs - the data access object used to read/write cliloc files
using System;
using System.Collections;
using System.IO;
using Server;
using Server.Items;

namespace Solaris.CliLocHandler
{
    //this is an entry which holds the loaded cliloc index/text pair
    public class CliLocEntry
    {
        protected int _Index;
        protected string _Text;

        public int Index
        {
            get { return _Index; }
        }

        public string Text
        {
            get { return _Text; }
        }

        public override string ToString()
        {
            return _Text;
        }

        public CliLocEntry(int index,string text)
        {
            _Index = index;
            _Text = text;
        }
    }

    //the data access object for reading in the cliloc data file
    class CliLocDAO
    {
        static string _FilePath;
        static string _Filename;

        public static string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }

        //default filename
        public CliLocDAO() : this("cliloc.enu")
        {
        }

        //find the file path
        public CliLocDAO(string filename) : this(filename,Core.FindDataFile(filename))
        {
        }

        //master constructor, where you can specify the filename and file path
        public CliLocDAO(string filename,string filepath)
        {
            _Filename = filename;
            _FilePath = filepath;
        }

        // Read operation, which loads all the data into a Hashtable
        public Hashtable Read()
        {
            Hashtable clilocs = new Hashtable(); // Create a Hashtable

            if (_Filename == null || _Filename == "")
            {
                _Filename = "cliloc.enu"; // default filename
            }

            if (File.Exists(_FilePath))
            {
                using (FileStream stream = new FileStream(_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Reader setup
                    BinaryReader reader = new BinaryReader(stream);

                    // Read the six header bytes to seek forward
                    for (int i = 0; i < 6; i++)
                    {
                        reader.ReadByte();
                    }

                    // Begin reading the cliloc contents. Text is encoded in UTF8 format
                    System.Text.Encoding encoding = System.Text.Encoding.UTF8;

                    int index = 0;

                    // Read until the end of the file or a problem occurs
                    while (stream.Position < stream.Length)
                    {
                        try
                        {
                            // Read string from binary file with UTF8 encoding
                            index = reader.ReadInt32();

                            // Read unused byte to seek reader ahead
                            reader.ReadByte();

                            // Read in string length and then read the string
                            short strlen = reader.ReadInt16();
                            byte[] buffer = new byte[strlen];
                            reader.Read(buffer, 0, strlen);

                            // Parse the string from the UTF8 encoded format
                            string text = encoding.GetString(buffer);

                            // Add the new entry to the list
                            clilocs.Add($"CliLoc_{index}", new CliLocEntry(index, text));
                        }
                        catch (EndOfStreamException) // Catch if we hit the end of the file
                        {
                            Console.WriteLine("End of CliLoc file reached.");
                            break; // Exit the loop
                        }
                        catch (Exception ex) // Catch any other potential errors during reading
                        {
                            Console.WriteLine($"Error reading CliLoc file: {ex.Message}");
                            break; // Exit the loop
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("CliLoc load error: file doesn't exist");
                return null;
            }

            return clilocs;
        }
    }
}
