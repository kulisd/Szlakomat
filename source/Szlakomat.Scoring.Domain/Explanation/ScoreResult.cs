namespace Szlakomat.Scoring.Domain.Explanation;

public class ScoreResult
{
    public double Score { get; set; }

    public List<string> Reasons { get; set; } = [];
}