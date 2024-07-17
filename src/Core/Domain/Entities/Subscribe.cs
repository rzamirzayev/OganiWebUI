namespace Domain.Entities
{
    public class Subscribe:ICreateEntity
    {
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

    }
}
