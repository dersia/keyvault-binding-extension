namespace SESExample 
{
    public class CustomerRegistered
    {
        internal CustomerRegistered() { }
        public CustomerRegistered(string? b2CId)
        {
            if (string.IsNullOrWhiteSpace(b2CId)) throw new ArgumentNullException(nameof(b2CId));
            B2CId = b2CId;
        }
        public string B2CId { get; set; }
        public string? Givenname { get; set; }
        public string? Surname { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? JobTitle { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? StreetAddress { get; set; }
        public string? Name { get; set; }
        public IEnumerable<string>? EmailAddresses { get; set; }
    }
}