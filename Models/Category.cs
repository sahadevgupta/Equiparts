namespace Equiparts.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public List<SubCategory> SubCategories { get; set; } = [];
}