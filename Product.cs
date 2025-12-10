using System.ComponentModel.DataAnnotations;

public class Product
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public bool IsDiscontinued { get; set; }
}

