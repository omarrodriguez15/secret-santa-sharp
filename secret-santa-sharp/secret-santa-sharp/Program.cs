using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Twilio;
using Twilio.Rest.Api.V2010.Account;

using Newtonsoft.Json;

namespace secret_santa_sharp
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");

         var pickSettings = LoadSettings();
         MatchSantas(pickSettings);
         VisualizeMatches(pickSettings);

         //TwilioClient.Init(pickSettings.TwilioSid, pickSettings.TwilioAuthToken);
         //SendMessages(pickSettings);
      }

      private static void SendMessages(PickingSettings ps)
      {
         foreach (var matchId in ps.Pairing)
         {
            var santa = ps.GetSantaById(matchId.Key);
            var match = ps.GetSantaById(matchId.Value);
            Console.WriteLine($"Key: {santa} Value: {match}");
            
            var msg = $"Hello {santa.Name}, you have matched with {match.Name}! Merry Christmas!";
            var message = SendText(msg, ps.SenderNumber, match.PhoneNumber);
            Console.WriteLine(message.Sid);
         }
      }

      private static MessageResource SendText(string body, string fromNumber, string toNumber)
      {
         return MessageResource.Create(
                      body: body,
                      from: new Twilio.Types.PhoneNumber(fromNumber),
                      to: new Twilio.Types.PhoneNumber(toNumber)
                  );
      }

      private static void VisualizeMatches(PickingSettings pickSettings)
      {
         foreach (var match in pickSettings.Pairing)
         {
            Console.WriteLine($"Key: {pickSettings.GetSantaById(match.Key)} Value: {pickSettings.GetSantaById(match.Value)}");
         }
      }

      private static void MatchSantas(PickingSettings pickSettings)
      {
         var ran = new Random((int)DateTime.Now.ToBinary());
         var availableSantas = pickSettings.Santas.ToDictionary(k => k.Id);
         foreach (var santa in pickSettings.Santas)
         {
            var potentialSantas = availableSantas
               .Select(x => x.Value)
               .Where(x => x.Id != santa.Id && x.HouseId != santa.HouseId).ToList();

            var santaNdx = ran.Next(0, potentialSantas.Count);
            var santaMatch = potentialSantas[santaNdx];

            pickSettings.Pairing.Add(santa.Id, santaMatch.Id);

            availableSantas.Remove(santaMatch.Id);
         }
      }

      private static PickingSettings LoadSettings()
      {
         var settingsFileName = "picking-settings.json";
         var fileContents = File.ReadAllText(settingsFileName);
         return JsonConvert.DeserializeObject<PickingSettings>(fileContents);
      }
   }

   public class PickingSettings
   {
      public readonly List<Santa> Santas;
      public readonly Dictionary<int, int> Pairing = new Dictionary<int, int>();
      public readonly string TwilioSid;
      public readonly string TwilioAuthToken;
      public readonly string SenderNumber;

      public PickingSettings(List<Santa> santas, Dictionary<int, int> pairing, string twilioSid, string twilioAuthToken, string senderNumber)
      {
         Santas = santas;
         Pairing = pairing;
         TwilioSid = twilioSid;
         TwilioAuthToken = twilioAuthToken;
         SenderNumber = senderNumber;
      }

      public Santa GetSantaById(int id) => Santas.FirstOrDefault(x => x.Id == id);
   }

   public class Santa
   {
      //TODO: using int for id for ease of text editing
      public readonly int Id;
      public readonly string Name;
      public readonly int HouseId;
      public readonly string PhoneNumber;

      public Santa(int id, string name, int houseId, string phoneNumber)
      {
         Id = id;
         Name = name;
         HouseId = houseId;
         PhoneNumber = phoneNumber;
      }

      public bool ValidMatch(Santa potential)
      {
         return potential.Id != Id && potential.HouseId != HouseId;
      }

      public override string ToString()
      {
         return $"Name={Name} | HouseId={HouseId} | Id={Id} | phone#={PhoneNumber}";
      }
   }
}
