using Szlakomat.Scoring.Domain.Projections;

namespace Szlakomat.Scoring.Domain.Rules;

public interface IRuleNode
{
    double Evaluate(UserCategoryProjection projection);
}
