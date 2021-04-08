using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Journey_Of_The_Ship
{
    public class SaveManager
    {
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Journey_Of_The_Ship\\SaveGame.txt";

        /*public void SaveGame()
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            FileStream fileStream = File.OpenWrite(path);
            BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(Main.musicVolume);
            writer.Flush();
            writer.Write(Main.soundEffectVolume);
            writer.Flush();
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

            BinaryReader reader = new BinaryReader(fileStream);
            Main.musicVolume = (float)reader.ReadDouble();
            Main.soundEffectVolume = (float)reader.ReadDouble();
            reader.Close();
            fileStream.Close();
        }*/


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
            reader.Close();
            fileStream.Close();
        }
    }
}
