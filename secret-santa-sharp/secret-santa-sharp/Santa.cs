namespace secret_santa_sharp
{
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
