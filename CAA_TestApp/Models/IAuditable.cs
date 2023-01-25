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
        string LocationChangedBy { get; set; }
        string LastLocation { get; set; }
        DateTime? LocationchangedOn { get; set; }
        string ShelfMoveBy { get; set; }
        DateTime? ShelfMoveOn { get;}
    }
}
