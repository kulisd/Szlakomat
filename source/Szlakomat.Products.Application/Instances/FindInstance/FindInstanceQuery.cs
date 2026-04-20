using MediatR;
using Szlakomat.Products.Application.Instances.Common;

namespace Szlakomat.Products.Application.Instances.FindInstance;

public record FindInstanceQuery(string InstanceId) : IRequest<InstanceView?>;
