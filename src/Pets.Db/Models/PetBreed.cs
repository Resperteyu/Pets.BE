namespace Pets.Db.Models
{

    public class PetBreed
    {
        public int Id { get; set; }

        public byte TypeId { get; set; }

        public string Title { get; set; }

        //public virtual PetType Type { get; set; }
    }
}
