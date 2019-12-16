using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace secret_santa_sharp
{
   public class PickingSettings
   {
      public readonly List<Santa> Santas;
      public readonly Dictionary<int, int> Pairing = new Dictionary<int, int>();
      public readonly string TwilioSid;
      public readonly string TwilioAuthToken;
      public readonly string SenderNumber;

      private const string _settingsFileName = "picking-settings.json";

      public PickingSettings(List<Santa> santas, Dictionary<int, int> pairing, string twilioSid, string twilioAuthToken, string senderNumber)
      {
         Santas = santas;
         Pairing = pairing;
         TwilioSid = twilioSid;
         TwilioAuthToken = twilioAuthToken;
         SenderNumber = senderNumber;
      }

      public Santa GetSantaById(int id) => Santas.FirstOrDefault(x => x.Id == id);

      public void VisualizeMatches(PickingSettings pickSettings)
      {
         foreach (var match in pickSettings.Pairing)
         {
            Console.WriteLine($"Santa: {pickSettings.GetSantaById(match.Key)} match: {pickSettings.GetSantaById(match.Value)}");
         }
      }

      public static PickingSettings LoadSettings()
      {
         var fileContents = File.ReadAllText(_settingsFileName);
         return JsonConvert.DeserializeObject<PickingSettings>(fileContents);
      }
   }
}
