using Newtonsoft.Json;
using DiscordRegisterManagementBot.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRegisterManagementBot.config
{
    public class JsonReader
    {
        private const string JsonDosyaAdi = "data.json";
        public async Task<T> ReadJson<T>(string fileName) where T : class
        {
            T data = null;
            using (StreamReader sr = new StreamReader(fileName))
            {
                string json = await sr.ReadToEndAsync();
                data = JsonConvert.DeserializeObject<T>(json);
            }

            return data;

        }
        public  void EkleVeKaydetAsync(Settings yeniAyar)
        {

            GuildSettings ayarlarListesi;

            if (File.Exists(JsonDosyaAdi))
            {
                var json = File.ReadAllText(JsonDosyaAdi);
                ayarlarListesi = JsonConvert.DeserializeObject<GuildSettings>(json);


            }
            else
            {
                ayarlarListesi = new GuildSettings();
            }

            ayarlarListesi.settings.Add(yeniAyar);

            var yeniJson = JsonConvert.SerializeObject(ayarlarListesi, Formatting.Indented);
            File.WriteAllText(JsonDosyaAdi, yeniJson);
        }
    }


}
