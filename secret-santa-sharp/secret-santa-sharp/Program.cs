using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace secret_santa_sharp
{
   class Program
   {
      static void Main(string[] args)
      {
         var dryRun = !(args.Length == 1 && args[0] == "hohoho");
         Console.WriteLine($"Running secret santa matcher! {(dryRun ? "Dry Run" : "")}");
         var pickSettings = PickingSettings.LoadSettings();

         MatchSantas(pickSettings);

         if (!dryRun)
         {
            File.WriteAllText(
               $"ps-{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.json",
               JsonConvert.SerializeObject(pickSettings, Formatting.Indented));
         }

         TwilioClient.Init(pickSettings.TwilioSid, pickSettings.TwilioAuthToken);
         SendMessages(pickSettings, dryRun);
      }

      private static void SendMessages(PickingSettings ps, bool dryRun)
      {
         foreach (var matchId in ps.Pairing)
         {
            var santa = ps.GetSantaById(matchId.Key);
            var match = ps.GetSantaById(matchId.Value);

            var msg = $"Hello {santa.Name}, you will be secret santa for {match.Name}! Merry Christmas!";
            Console.WriteLine(msg);
            Console.WriteLine($"Number={santa.PhoneNumber}");

            if (!dryRun) SendText(msg, ps.SenderNumber, santa.PhoneNumber);
         }
      }

      private static void SendText(string body, string fromNumber, string toNumber)
      {
         Console.WriteLine($"Send text to {toNumber}");
         var msg = MessageResource.Create(
               body: body,
               from: new Twilio.Types.PhoneNumber(fromNumber),
               to: new Twilio.Types.PhoneNumber(toNumber)
         );
         Console.WriteLine(msg.Sid);
      }

      private static void MatchSantas(PickingSettings pickSettings)
      {
         var ran = new Random((int)DateTime.Now.Ticks);
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
   }
}
