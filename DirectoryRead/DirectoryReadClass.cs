using netWebFilterGlobal;
using System.Configuration;
using System.IO;
using System.Web;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DirectoryRead
{
    public class DirectoryReadClass : NetFilterBase, INetCodeBehind
    {
        public override void DoPostBack()
        {
            string result = "";
            bool main = true;

            string[] p = parameters.Split(',');
            string path = HttpUtility.UrlDecode(p[1]).Replace("/", "\\").Replace("$_$", ",");
            if (!path.StartsWith("\\")) path = "\\" + path;
            if (path != "\\")
            {
                result = "";
                main = false;
            }

            string startDir = ConfigurationManager.AppSettings[p[0]];
            int fileCounter = 0;
            string fileArray = "[ ";

            switch (postBackFunction)
            {
                case "list":
                    string[] dirs = Directory.GetDirectories(startDir + path);
                    string startLetter = "";
                    SortedDictionary<string, string> sortedList = new SortedDictionary<string, string>();
                    foreach (string dir in dirs)
                    {
                        int index = dir.LastIndexOf("\\");

                        try
                        {
                            if (dir.ToLower().Substring(index + 1).StartsWith("the "))
                            {
                                sortedList.Add(dir.Substring(index + 4).Trim() + ", The", dir);
                            }
                            else
                            {
                                sortedList.Add(dir.Substring(index + 1), dir);
                            }
                        } catch (Exception e)
                        {
                            int i = 0;
                        }
                    }

                    foreach (string dir in sortedList.Keys)
                    {
                        string newDir = dir.Replace(startDir + path, string.Empty).Replace("'", "*").Replace("\\", "/");
                        string realDir;
                        sortedList.TryGetValue(dir, out realDir);
                        realDir = realDir.Replace(startDir + path, string.Empty).Replace("'", "*").Replace("\\", "/");
                        string newStartLetter = newDir.Substring(0, 1);
                        int test = 0;
                        if (!int.TryParse(newStartLetter, out test) && newStartLetter != "*" && main)
                        {
                            if (newStartLetter != startLetter)
                            {
                                startLetter = newStartLetter;
                                result += "<a name=\"" + startLetter + "\" class=\"jumpTo\"></a>";
                            }
                        }
                        result += "<a class=\"dir\" onclick=\"music.listDir(&single" + realDir + "&single);\">" + newDir.Replace("*", "&single") +"</a><br/>";
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        string pattern = "*.mp3";
                        if (i == 1) pattern = "*.m4a";

                        string[] files = Directory.GetFiles(startDir + path, pattern, SearchOption.TopDirectoryOnly);
                        fileCounter = 0;
                        
                        foreach (string file in files)
                        {
                            string newFile = file.Replace(startDir + path, string.Empty).Replace("'", "*").Replace("\\", "/");
                            result += "<a class=\"file\" onclick=\"music.playFile(&single" + newFile + "&single," + fileCounter.ToString() + ");\">" + newFile.Replace("*", "&single") + "</a><br/>";
                            fileCounter++;
                            fileArray += "\"" + newFile + "\",";
                        }
                    }
                    WriteResult(result + "~~" + fileArray.Substring(0, fileArray.Length-1) + "]");
                    break;
                case "sub":
                    for (int i = 0; i < 2; i++)
                    {
                        string pattern = "*.mp3";
                        if (i == 1) pattern = "*.m4a";

                        string[] files = Directory.GetFiles(startDir + path, pattern, SearchOption.AllDirectories);
                        fileCounter = 0;

                        foreach (string file in files)
                        {
                            string newFile = file.Replace(startDir + path, string.Empty).Replace("'", "*").Replace("\\", "/");
                            result += "<a class=\"file\" onclick=\"music.playFile(&single" + newFile + "&single," + fileCounter.ToString() + ");\">" + newFile.Replace("*", "&single") + "</a><br/>";
                            fileCounter++;
                            fileArray += "\"" + newFile + "\",";
                        }
                    }
                    WriteResult(result + "~~" + fileArray.Substring(0, fileArray.Length - 1) + "]");
                    break;
                case "play":
                    string f = startDir + path;
                    File.WriteAllText("args.txt", f);
                    File.Copy(f, p[2] + ".mp3", true);
                    //File.Copy(startDir + "\\311\\Grassroots\\11 Lose.mp3", "current.mp3", true);
                    WriteResult("ok");
                    break;
                case "rand":
                    int number = Convert.ToInt32(p[2]);
                    List<String> songs = new List<string>();
                    Random random = new Random();
                    for (int x = 0; x < number; x++)
                    {
                        string song = GetRandomSong(songs, startDir + path, random);
                        songs.Add(song);
                        Thread.Sleep(10);
                    }

                    fileCounter = 0;

                    foreach (string file in songs)
                    {
                        string newFile = file.Replace(startDir + path, string.Empty).Replace("'", "*").Replace("\\", "/");
                        result += "<a class=\"file\" onclick=\"music.playFile(&single" + newFile + "&single," + fileCounter.ToString() + ");\">" + newFile.Replace("*", "&single") + "</a><br/>";
                        fileCounter++;
                        fileArray += "\"" + newFile + "\",";
                    }
                    WriteResult(result + "~~" + fileArray.Substring(0, fileArray.Length - 1) + "]");
                    break;
            }
        }

        private void WriteResult(string result)
        {
            if (string.IsNullOrEmpty(callbackFunction))
                _callingFilter.AddToBuffer("_0200_'" + result + "'");
            else
                _callingFilter.AddToBuffer("_5200_" + callbackFunction + "('" + result + "');");
        }

        private string GetRandomSong(List<string> songs, string path, Random random)
        {
            string result = string.Empty;

            List<string> available = new List<string>();

            available.AddRange(Directory.GetDirectories(path));
            for (int i = 0; i < 2; i++)
            {
                string pattern = "*.mp3";
                if (i == 1) pattern = "*.m4a";
                available.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
            }

            while (!songs.Contains(result))
            {
                int position = random.Next(available.Count - 1);

                if (!available[position].EndsWith(".mp3") && !available[position].EndsWith(".m4a"))
                {
                    if (available[position].LastIndexOf(".") == available[position].Length - 4) continue;
                    result = GetRandomSong(songs, available[position], random);
                    if (!songs.Contains(result)) break;
                }
                else
                {
                    result = available[position];
                    break;
                }
            }

            return result;
        }
    }
}
