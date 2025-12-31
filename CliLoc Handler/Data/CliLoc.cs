using System;
using System.Collections;
using System.Collections.Generic;
using Server;

//this is the client localization text handler tool
namespace Solaris.CliLocHandler
{
    public class CliLoc
    {
        public static int MaxEntry { get { return 3011032; } }

        //this is used to guess the space required for cliloc text on a gump.  Basically it's the average pixels per character
        public static int PixelsPerCharacter { get { return 7; } }
        private static readonly object _lockObject = new object(); // Create a lock object for thread safety
        private static Hashtable _CliLocs;

        public static Hashtable CliLocs
        {
            get
            {
                if (_CliLocs == null)
                {
                    lock (_lockObject) // Use a lock to ensure thread safety
                    {
                        if (_CliLocs == null) // Double-check to avoid unnecessary initialization
                        {
                            _CliLocs = new CliLocDAO().Read(); // Initialize only if it's still null
                        }
                    }
                }

                return _CliLocs;
            }
        }

        public void Initialize()
        {
            CliLocDAO dao = new CliLocDAO();
            Hashtable clilocs = dao.Read();

            if (clilocs != null)
            {
                // Access a CliLocEntry by its index
                CliLocEntry entry = (CliLocEntry)clilocs["CliLoc_123"];

                // ... (Do something with the entry)
            }
        }

        //this method provides a direct access object property list for a specified object
        public static DirectObjectPropertyList GetDirectPropertyList(object obj)
        {
            //fetch the object property list for this object
            ObjectPropertyList opl = null;

            if (obj is Item)
            {
                Item item = (Item)obj;

                opl = new ObjectPropertyList(item);
                item.GetProperties(opl);
            }
            else if (obj is Mobile)
            {
                Mobile mobile = (Mobile)obj;

                opl = new ObjectPropertyList(mobile);
                mobile.GetProperties(opl);
            }

            if (opl == null)
            {
                //if there was a problem with this process, just return null
                return null;
            }

            return new DirectObjectPropertyList(opl);
        }

        //this method gets the name for an object from the object property list.
        public static string GetName(object obj)
        {
            return GetPropertiesList(obj)[0];
        }

        //generate the full object property list 
        public static List<string> GetPropertiesList(object obj)
        {
            if (obj is Type)
            {
                try
                {
                    //create the object
                    object typeobj = Activator.CreateInstance((Type)obj);

                    //find its name using the instanced object
                    List<string> recurseproperties = GetPropertiesList(typeobj);

                    //clean up by removing this object
                    if (typeobj is Item)
                    {
                        ((Item)typeobj).Delete();
                    }
                    else if (typeobj is Mobile)
                    {
                        ((Mobile)typeobj).Delete();
                    }

                    return recurseproperties;
                }
                catch (Exception e)
                {
                    //if there was a problem with this process, just return the type name

                    return new List<string>(new string[] { e.Message });
                }
            }

            DirectObjectPropertyList dopl = GetDirectPropertyList(obj);

            if (dopl == null)
            {
                return new List<string>(new string[] { "null" }); ;
            }

            List<string> properties = new List<string>();

            foreach (DOPLEntry doplentry in dopl)
            {
                properties.Add(CliLoc.LocToString(doplentry.Index,doplentry.Arguments));
            }

            return properties;
        }

        //the main method used for producing useful strings
        public static string LocToString(int index)
        {
            // Access the CliLocs property, which handles initialization
            Hashtable clilocs = CliLocs;

            if (clilocs == null)
            {
                return "CliLoc not loaded!";
            }

            // Use key lookup for efficient access
            string key = $"CliLoc_{index}";
            if (clilocs.ContainsKey(key))
            {
                return ((CliLocEntry)clilocs[key]).Text;
            }

            return null;
        }

        //the special case, where there are arguments to insert in the string
        public static string LocToString(int index,string args)
        {
            if (args == null || args == "")
            {
                return LocToString(index);
            }

            string basestring = LocToString(index);

            //parse the string for any argument identifiers
            while (basestring != null && basestring.IndexOf("~") > -1)
            {
                //this determines the string that needs replacing
                string replacestring = FindReplace("~",basestring);

                int argsdivider = args.IndexOf("\t");

                //here's the string that will replace it
                string argstring = argsdivider == -1 ? args : args.Substring(0,argsdivider);

                //rethreaded
                if (argstring.IndexOf("#") == 0)
                {
                    int recurseloc = Convert.ToInt32(argstring.Substring(1,argstring.Length - 1));

                    argstring = LocToString(recurseloc);
                }

                basestring = basestring.Replace(replacestring,argstring);

                if (argsdivider > -1)
                    args = args.Substring(argsdivider + 1,args.Length - (argsdivider) - 1);
            }

            return basestring;
        }

        public static int GetMaxLength(List<string> strings)
        {
            if (strings == null)
            {
                return 0;
            }

            int maxlength = 0;

            foreach (string str in strings)
            {
                if (str == null)
                {
                    continue; // Skip null strings
                }
                maxlength = Math.Max(maxlength, str.Length);
            }

            return maxlength;
        }

        //useful find and replace method when inserting arguments in a localized string
        private static string FindReplace(string key,string fromstring)
        {
            string replacestring = "";

            if (fromstring.IndexOf(key) > -1)
            {
                replacestring += key;

                //chop off up to and including the first key
                fromstring = fromstring.Substring(fromstring.IndexOf(key) + 1,fromstring.Length - fromstring.IndexOf(key) - 1);

                //grab up to the second key
                replacestring += fromstring.Substring(0,fromstring.IndexOf(key) + 1);
            }

            return replacestring;
        }
    }
}
