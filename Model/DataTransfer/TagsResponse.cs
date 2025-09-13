using Model.DataAccess.Entities;

namespace Model.DataTransfer;

public class TagsResponse
{
    public List<Tag> Items { get; set; } = [];
    public bool HasMore { get; set; }
    public int QuotaMax { get; set; }
    public int QuotaRemaining { get; set; }
}