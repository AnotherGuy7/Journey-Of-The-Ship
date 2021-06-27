using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Journey_Of_The_Ship
{
    public class SaveManager
    {
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Journey_Of_The_Ship\\SaveGame.txt";

        public void SaveGame()
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            FileStream fileStream = File.OpenWrite(path);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine(Main.musicVolume);
            writer.WriteLine(Main.soundEffectVolume);
            writer.WriteLine((int)Player.turretType);
            writer.WriteLine((int)Player.wingType);
            writer.WriteLine((int)Player.propellerType);
            writer.Close();
            fileStream.Close();
        }

        public void LoadGame()
        {
            if (!File.Exists(path))
                return;

            FileStream fileStream = File.OpenRead(path);

            if (fileStream.Length == 0)
            {
                fileStream.Close();
                return;
            }

            StreamReader reader = new StreamReader(fileStream);
            Main.musicVolume = (float)Convert.ToDouble(reader.ReadLine());
            Main.soundEffectVolume = (float)Convert.ToDouble(reader.ReadLine());
            Player.turretType = (Player.TurretTypes)Convert.ToInt32(reader.ReadLine());
            Player.wingType = (Player.WingTypes)Convert.ToInt32(reader.ReadLine());
            Player.propellerType = (Player.PropellerType)Convert.ToInt32(reader.ReadLine());
            reader.Close();
            fileStream.Close();
        }
    }
}
