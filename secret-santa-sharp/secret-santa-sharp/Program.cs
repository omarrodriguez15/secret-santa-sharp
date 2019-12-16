using System;
using System.Linq;

using Twilio.Rest.Api.V2010.Account;

namespace secret_santa_sharp
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");

         var pickSettings = PickingSettings.LoadSettings();
         MatchSantas(pickSettings);

         //TwilioClient.Init(pickSettings.TwilioSid, pickSettings.TwilioAuthToken);
         SendMessages(pickSettings);
      }

      private static void SendMessages(PickingSettings ps)
      {
         foreach (var matchId in ps.Pairing)
         {
            var santa = ps.GetSantaById(matchId.Key);
            var match = ps.GetSantaById(matchId.Value);
            
            var msg = $"Hello {santa.Name}, you have matched with {match.Name}! Merry Christmas!";
            Console.WriteLine(msg);
            Console.WriteLine($"Number={match.PhoneNumber}");

            //SendText(msg, ps.SenderNumber, match.PhoneNumber);
         }
      }

      private static void SendText(string body, string fromNumber, string toNumber)
      {
         var msg = MessageResource.Create(
               body: body,
               from: new Twilio.Types.PhoneNumber(fromNumber),
               to: new Twilio.Types.PhoneNumber(toNumber)
         );
         Console.WriteLine(msg.Sid);
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
   }
}
