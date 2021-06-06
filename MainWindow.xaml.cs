using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Auto_Transcriber
{

    public partial class MainWindow : Window
    {
        private string rootPath;
        private string speakerFile;
        private string settingsFile;
        private string selectedFile;
        private string rpyFile;

        List<string> outputLog = new List<string>();

        public IDictionary<string, string> speakers = new Dictionary<string, string>()
        {
            { "NONE", "narrator" },
            { "MC", "u" },
            { "JULIA", "ju" },
            { "CAR", "car" },
            { "CAMERON", "ca" },
            { "MASON", "ma" },
            { "AUTUMN", "aut" },
            { "EMILY", "em" },
            { "MRS. ANDERSON", "an" },
            { "CHRIS", "ch" },
            { "NORA", "no" },
            { "RYAN", "ry" },
            { "MS. ROSE", "ro" },
            { "LAUREN", "la" },
            { "RILEY", "ri" },
            { "ELIJAH", "el" },
            { "IMRE", "imre" },
            { "AUBREY", "au" },
            { "SAM", "sam" },
            { "KAREN", "karen" },
            { "JOSH", "jo" },
            { "COURTNEY", "courtney" },
            { "JEREMY", "jeremy" },
            { "KATY", "katy" },
            { "SARAH", "sarah" },
            { "GRAYSON", "gr" },
            { "CHLOE", "cl" },
            { "TOM", "tom" },
            { "TUTORIAL", "tut" },
            { "MR. LEE", "lee" },
            { "BEN", "ben" },
            { "DR. EHRMANTRAUT", "ehr" },
            { "PENELOPE", "pe" },
            { "EVELYN", "ev" },
            { "ARRON", "aa" },
            { "SECURITY GUARD", "sec" },
            { "LINSEY", "li" },
            { "UNKNOWN", "unknown" },
            { "UBER DRIVER", "uber" },
            { "STORE CLERK", "clerk" },
            { "AMBER", "am" },
            { "KIM", "ki" },
            { "ADAM", "ad" },
            { "COUNSELOR", "co" },
            { "WAITER", "waiter" },
            { "HOST", "host" },
            { "LISA", "poet1" },
            { "MARTIN", "poet2" },
            { "PETER", "guya" },
            { "HARRY", "guyb" },
            { "FINN", "finn" },
            { "PERRY", "guyd" },
            { "SEBASTIAN", "se" },
            { "MARCUS", "guyc" },
            { "MATT", "matt" },
            { "CALEB", "cal" },
            { "COOPER", "coop" },
            { "KAI", "kai" },
            { "WESLEY", "wes" },
            { "PARKER", "par" },
            { "ANGELICA", "rg1" },
            { "ELISA", "rg2" },
            { "NERD", "nerd" },
            { "XAVI", "xav" },
            { "JAXON", "jax" },
            { "TEACHER", "teach" },
            { "CLASS", "class1" },
            { "SAMANTHA", "sa" },
            { "THE DEAN", "de" },
            { "JOE", "je" },
            { "SPEAKER ANNOUNCEMENT", "ann" },
            { "EMPLOYEE", "empl" },
            { "FEMALE VOICE", "unkfem" },
            { "JENNY", "jen" },
            { "MR. ROSE", "mrr" },
            { "JERRY", "jer" },
            { "RACHEL", "dg1" },
            { "ELANOR", "dg2" },
            { "REBECCA", "dg4" },
            { "BARTENDER", "bart" }
        };
        private IDictionary<string, string> settings = new Dictionary<string, string>()
        {
            {"GameVersion", "v10"}
        };

        public MainWindow()
        {
            InitializeComponent();

            ConvertFile.Visibility = Visibility.Hidden;

            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oscar", "Auto Transcriber");
            speakerFile = Path.Combine(rootPath, "speakers.json");
            settingsFile = Path.Combine(rootPath, "settings.json");

            Directory.CreateDirectory(rootPath);

            if (File.Exists(settingsFile))
            {
                addLog("Loading settings file...");
                settings = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(settingsFile));
            }
            else
            {
                addLog("No settings file found. Creating default settings file...");

                using (FileStream fs = File.Create(settingsFile))
                {
                    fs.Close();
                    File.WriteAllText(settingsFile, JsonSerializer.Serialize(settings));
                }
            }

            if (File.Exists(speakerFile))
            {
                addLog("Loading speaker file...");
                speakers = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(speakerFile));
            }
            else
            {
                addLog("No speaker file found. Creating default speaker file...");

                using (FileStream fs = File.Create(speakerFile))
                {
                    fs.Close();
                    JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(speakerFile, JsonSerializer.Serialize(speakers, jsonOptions));
                }
            }

            GameVersion.Text = settings["GameVersion"];
        }

        private void addLog(string text)
        {
            outputLog.Insert(0, text);
            WindowOutput.Text = String.Join("\n", outputLog.ToArray());
        }

        private void BrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "Script files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFile = openFileDialog.FileName;
                    ChosenFile.Text = $"Selected File: {selectedFile}";
                    ConvertFile.Visibility = Visibility.Visible;
                }
            }
            rpyFile = Path.ChangeExtension(selectedFile, ".rpy");
        }

        private void writeToRpyFile(string text)
        {
            using (StreamWriter sw = File.AppendText(rpyFile))
            {
                sw.WriteLine(text);
            }
        }

        private void phoneCode(string line)
        {
            string[] messageArray = line.Split(' ');
            string contact = "";
            string message = "";
            if (messageArray[1] != "KiwiiReply")
            {
                contact = messageArray[2];
                for (int i = 3; i < messageArray.Length; i++)
                {
                    message += $"{messageArray[i]} ";
                }
            }
            else
            {
                for (int i = 2; i < messageArray.Length; i++)
                {
                    message += $"{messageArray[i]} ";
                }
            }
            message.Trim(' ');

            if (messageArray[1] == "Message")
            {
                writeToRpyFile($"## $ contact_{contact}.newMessage(\"{message}\", queue=True)");
            }
            else if (messageArray[1] == "ImageMessage")
            {
                writeToRpyFile($"## $ contact_{contact}.newImgMessage(\"{message}\", queue=True)");
            }
            else if (messageArray[1] == "Reply")
            {
                writeToRpyFile($"## $ contact_{contact}.addReply(\"{message}\", func=None)");
            }
            else if (messageArray[1] == "ImageReply")
            {
                writeToRpyFile($"## $ contact_{contact}.addImgReply(\"{message}\", func=None)");
            }
            else if (messageArray[1] == "KiwiiPost")
            {
                writeToRpyFile($"## $ newKiwiiPost = KiwiiPost(\"{contact}\", image, \"{message}\", mentions=None)");
            }
            else if (messageArray[1] == "KiwiiComment")
            {
                writeToRpyFile($"## $ newKiwiiPost.newComment(\"{contact}\", \"{message}\", mentions=None, queue=True)");
            }
            else if (messageArray[1] == "KiwiiReply")
            {
                writeToRpyFile($"## $ newKiwiiPost.addReply(\"{message}\", func=None, mentions=None)");
            }
            else
            {
                writeToRpyFile($"### ERROR: {line}");
            }
        }

        private void ConvertFile_Click(object sender, RoutedEventArgs e)
        {
            speakers = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(speakerFile));

            StreamReader file = new StreamReader(selectedFile);

            string line;
            bool isSpeaker = false;
            string speaker = "";
            while ((line = file.ReadLine()) != null){
                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    writeToRpyFile("");
                    continue;
                }

                if (line.StartsWith("-") && line.EndsWith("-"))
                {
                    writeToRpyFile($"# {line}");
                    continue;
                }

                if (line.StartsWith("NEW"))
                {
                    phoneCode(line);
                    continue;
                }

                if (speakers.ContainsKey(line))
                {
                    isSpeaker = true;
                    speaker = speakers[line];
                    continue;
                }

                if (isSpeaker)
                {
                    writeToRpyFile($"{speaker} \"{line.Replace("\"", "\\\"")}\"\n");
                    writeToRpyFile($"scene {settings["GameVersion"]}");
                    writeToRpyFile("with dissolve");
                    isSpeaker = false;
                    continue;
                }

                writeToRpyFile($"### ERROR: {line}");
            }

            addLog($"File successfully converted.\nNew File: {rpyFile}");
        }

        private void EditSpeakers_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(speakerFile);
        }

        private void VersionNumber_Changed(object sender, TextChangedEventArgs e)
        {
            settings["GameVersion"] = GameVersion.Text;
            File.WriteAllText(settingsFile, JsonSerializer.Serialize(settings));
        }
    }
}
