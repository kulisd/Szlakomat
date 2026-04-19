using Szlakomat.Products.Domain.Common.Identifiers;
using Szlakomat.Products.Domain.Quantity;

namespace Szlakomat.Products.Domain.Instances;

/// <summary>
/// Batch opisuje zbiór ProductInstance konkretnego ProductType,
/// które są śledzone razem, zwykle w celach kontroli jakości.
///
/// Używane gdy:
/// - Tożsamość poszczególnego egzemplarza nie jest ważna, ale pochodzenie partii ma znaczenie
/// - Trzeba śledzić informacje o produkcji/kontroli jakości
/// - Powszechnie używane w żywnościach, chemikaliach i towarach wytwarzanych
///
/// Przykłady: partie żywności z datami ważności, części z tej samej serii produkcyjnej
/// </summary>
internal class Batch
{
    public BatchId Id { get; }
    public BatchName Name { get; }
    public IProductIdentifier BatchOf { get; }
    public Unit QuantityInBatch { get; }
    public DateTimeOffset? DateProduced { get; }
    public DateTimeOffset? SellBy { get; }
    public DateTimeOffset? UseBy { get; }
    public DateTimeOffset? BestBefore { get; }
    public ISerialNumber? StartSerialNumber { get; }
    public ISerialNumber? EndSerialNumber { get; }
    public string? Comments { get; }

    private Batch(BatchId? id,
                  BatchName? name,
                  IProductIdentifier? batchOf,
                  Unit? quantityInBatch,
                  DateTimeOffset? dateProduced,
                  DateTimeOffset? sellBy,
                  DateTimeOffset? useBy,
                  DateTimeOffset? bestBefore,
                  ISerialNumber? startSerialNumber,
                  ISerialNumber? endSerialNumber,
                  string? comments)
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(name);
        Guard.IsNotNull(batchOf);
        Guard.IsNotNull(quantityInBatch);

        Id = id;
        Name = name;
        BatchOf = batchOf;
        QuantityInBatch = quantityInBatch;
        DateProduced = dateProduced;
        SellBy = sellBy;
        UseBy = useBy;
        BestBefore = bestBefore;
        StartSerialNumber = startSerialNumber;
        EndSerialNumber = endSerialNumber;
        Comments = comments;
    }

    public static Builder CreateBuilder() => new();

    public override string ToString() =>
        $"Batch{{id={Id}, name={Name}, of={BatchOf}, quantity={QuantityInBatch}}}";

    public class Builder
    {
        private BatchId? _id;
        private BatchName? _name;
        private IProductIdentifier? _batchOf;
        private Unit? _quantityInBatch;
        private DateTimeOffset? _dateProduced;
        private DateTimeOffset? _sellBy;
        private DateTimeOffset? _useBy;
        private DateTimeOffset? _bestBefore;
        private ISerialNumber? _startSerialNumber;
        private ISerialNumber? _endSerialNumber;
        private string? _comments;

        public Builder Id(BatchId id)
        {
            _id = id;
            return this;
        }

        public Builder Name(BatchName name)
        {
            _name = name;
            return this;
        }

        public Builder BatchOf(IProductIdentifier batchOf)
        {
            _batchOf = batchOf;
            return this;
        }

        public Builder QuantityInBatch(Unit quantityInBatch)
        {
            _quantityInBatch = quantityInBatch;
            return this;
        }

        public Builder DateProduced(DateTimeOffset dateProduced)
        {
            _dateProduced = dateProduced;
            return this;
        }

        public Builder SellBy(DateTimeOffset sellBy)
        {
            _sellBy = sellBy;
            return this;
        }

        public Builder UseBy(DateTimeOffset useBy)
        {
            _useBy = useBy;
            return this;
        }

        public Builder BestBefore(DateTimeOffset bestBefore)
        {
            _bestBefore = bestBefore;
            return this;
        }

        public Builder StartSerialNumber(ISerialNumber startSerialNumber)
        {
            _startSerialNumber = startSerialNumber;
            return this;
        }

        public Builder EndSerialNumber(ISerialNumber endSerialNumber)
        {
            _endSerialNumber = endSerialNumber;
            return this;
        }

        public Builder Comments(string comments)
        {
            _comments = comments;
            return this;
        }

        public Batch Build() =>
            new(_id!, _name!, _batchOf!, _quantityInBatch!, _dateProduced, _sellBy, _useBy, _bestBefore,
                _startSerialNumber, _endSerialNumber, _comments);
    }
}
