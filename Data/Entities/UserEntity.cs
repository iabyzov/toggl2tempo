namespace Data.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string TogglToken { get; set; }

        public string TempoToken { get; set; }
    }
}