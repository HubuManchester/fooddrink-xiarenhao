using SQLite;

namespace BiteRecord.Models;

[SQLite.Table("BiteRecords")]
public class BiteRecordModel
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }

    [SQLite.NotNull]
    public string RestaurantName { get; set; } = string.Empty;

    [SQLite.NotNull]
    public string DishName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public decimal PricePerPerson { get; set; }

    public string ReviewText { get; set; } = string.Empty;

    public string PhotoPath { get; set; } = string.Empty;

    public string Latitude { get; set; } = string.Empty;

    public string Longitude { get; set; } = string.Empty;

    public string LocationAddress { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string RatingDisplay => $"{Rating} ★";

    public string PriceDisplay => PricePerPerson > 0 ? $"¥{PricePerPerson:F0}/person" : "N/A";

    public string DateDisplay => CreatedDate.ToString("yyyy-MM-dd");

    public string AccessibleSummary => $"{RestaurantName}. {DishName}. Rating {Rating} stars. {PriceDisplay}. {ReviewText}";
}