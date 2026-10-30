namespace Apps.Magento.Models.Responses;

public class PaginationResponse<T>
{
    public virtual List<T> Items { get; set; } = new();
}