namespace CAA_TestApp.Models
{
    public interface IAuditable
    {
        string OrderedBy { get; set; }
        DateTime? OrderedOn { get; set; }
        string TookBy { get; set; }
        DateTime? TookOn { get; set; }
        string ReturnedBy { get; set; }
        DateTime? ReturnedOn { get; set; }
    }
}
