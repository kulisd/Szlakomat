namespace Szlakomat.Scoring.Domain.Projections;

public class UserCategoryProjection
{
    public Guid UserId { get; set; }

    public required string Category { get; set; }

    public int Clicks30Days { get; set; }

    public int Purchases90Days { get; set; }

    public int Skips30Days { get; set; }

    public double AverageRating { get; set; }
}
