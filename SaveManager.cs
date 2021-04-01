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

            FileStream fileStream = File.Open(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(Main.musicVolume);
            writer.Write(Main.soundEffectVolume);
            fileStream.Close();
        }

        public void LoadGame()
        {
            if (!File.Exists(path))
                return;

            FileStream fileStream = File.Open(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(fileStream);
            Main.musicVolume = (float)reader.ReadDouble();
            Main.soundEffectVolume = (float)reader.ReadDouble();
            fileStream.Close();
        }
    }
}
