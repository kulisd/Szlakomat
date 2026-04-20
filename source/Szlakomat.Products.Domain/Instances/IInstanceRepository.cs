namespace Szlakomat.Products.Domain.Instances;

internal interface IInstanceRepository
{
    void Save(IInstance instance);
    IInstance? FindById(InstanceId id);
    IInstance? FindByStringId(string id);
    IReadOnlyList<IInstance> FindAll();
}
