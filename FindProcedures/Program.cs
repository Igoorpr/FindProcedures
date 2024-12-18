using System.Text.RegularExpressions;
using System.Text;

class Program
{
    static void Main()
    {
        try
        {
            string filepath = "FileFound.txt";
            string filepath2 = "FileNotFound.txt";

            string dir1 = @"C:\Users\PC\Documents\Path\"; // Base path
            string dir2 = @"C:\Users\PC\Documents\Path\Compare"; // Campare path

            List<string> lstfilesfound = new List<string>();
            List<string> lstfilesnotfound = new List<string>();

            //finds all .sql files in the directory
            HashSet<string> filesDir1 = new HashSet<string>(
            Directory.GetFiles(dir1, "*.sql", SearchOption.AllDirectories)
                      .Select(Path.GetFileName)
                      .Select(name => name.ToUpper())
            );

            HashSet<string> filesDir2 = new HashSet<string>(
            Directory.GetFiles(dir2, "*.sql", SearchOption.AllDirectories)
                        .Select(Path.GetFileName)
                        .Select(name => name.ToUpper()));

            List<string> finalfile = Directory.GetFiles(dir1, "*.sql", SearchOption.AllDirectories)
            .Where(arquivo => arquivo.Any(char.IsUpper))
            .ToList();

            List<string> filetocopy = Directory.GetFiles(dir2, "*.sql", SearchOption.AllDirectories)
            .Where(arquivo => arquivo.Any(char.IsUpper))
            .ToList();

            foreach (string file in filesDir1)
            {
                //Standardizing file names for comparison
                if (filesDir2.Contains(file.Replace("BRS_", "").Replace("PRD_", "")))
                {
                    lstfilesfound.Add(file);

                    int indice = 0;

                    indice = filetocopy.FindIndex(fileTest => fileTest.ToUpper().EndsWith((file.Replace("BRS_", "").Replace("PRD_", ""))));

                    if (indice > -1 && file.Contains("BRS_")) {
                        string text = File.ReadAllText(filetocopy[indice]);

                        string modifiedText = text.Replace("DB_BASE_", "DB_BASE_PRD_");

                        int indexend = finalfile.FindIndex(filete => filete.ToUpper().EndsWith(file.Replace(".sql", ".SQL")));

                        File.WriteAllText(finalfile[indexend], (RemoveAccents(modifiedText)).ToUpper());
                    }
                }
                else
                {
                    lstfilesnotfound.Add(file);
                }
            }

            if (File.Exists(filepath))
            {
                // Delete
                File.Delete(filepath);
            }

            if (File.Exists(filepath2))
            {
                File.Delete(filepath2);
            }

            //Generating files with the final result
            foreach (string filecount in lstfilesfound)
            {
                using (StreamWriter writer = new StreamWriter(filepath, true))
                {
                    writer.WriteLine(filecount);
                }
            }

            foreach (string filecount1 in lstfilesnotfound)
            {
                using (StreamWriter writer = new StreamWriter(filepath2, true))
                {
                    writer.WriteLine(filecount1);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

    }

    private static string RemoveAccents(string text)
    {
        string result = "";

        if (text.Length > 0)
        {
            try
            {
                var Encode8Bits = Encoding.GetEncoding(1251).GetBytes(text);
                var String7Bits = Encoding.ASCII.GetString(Encode8Bits);
                var RemoveAccent = new Regex("[^a-zA-Z0-9]=-_/");
                result = RemoveAccent.Replace(String7Bits, " ");
            }
            catch (Exception)
            {
                return result;
            }
        }

        return result;
    }
}

