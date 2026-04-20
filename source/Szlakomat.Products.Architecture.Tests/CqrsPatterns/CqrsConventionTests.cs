using MediatR;
using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.CqrsPatterns;

public class CqrsConventionTests : TestBase
{
    [Fact]
    public void Commands_ShouldImplement_IRequest()
    {
        // Act
        var commandTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.IsPublic
                        && t.IsClass
                        && !t.IsAbstract
                        && t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .ToList();

        // Assert
        commandTypes.Should().NotBeEmpty("there should be commands/queries implementing IRequest<>");
    }

    [Fact]
    public void Handlers_ShouldBe_Sealed()
    {
        // Arrange
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        // Act
        var sealedResults = handlers
            .Select(handler => new
            {
                handler,
                isSealed = handler.IsSealed
            })
            .ToList();

        // Assert
        handlers.Should().NotBeEmpty("there should be MediatR handlers");

        foreach (var check in sealedResults)
        {
            check.isSealed.Should().BeTrue($"{check.handler.Name} should be sealed to prevent inheritance");
        }
    }

    [Fact]
    public void Handlers_ShouldHave_SingleResponsibility()
    {
        // Arrange
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        // Act
        var responsibilityResults = handlers
            .Select(handler => new
            {
                handler,
                handlerInterfaces = handler.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .ToList()
            })
            .ToList();

        // Assert
        foreach (var check in responsibilityResults)
        {
            check.handlerInterfaces.Should().HaveCount(1,
                $"{check.handler.Name} should handle exactly one command/query, but handles {check.handlerInterfaces.Count}");
        }
    }

    [Fact]
    public void Handlers_ShouldReside_InSameNamespace_AsCommand()
    {
        // Arrange
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        // Act
        var namespaceResults = handlers
            .Select(handler => new
            {
                handler,
                handlerInterface = handler.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)),
                commandType = handler.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .GetGenericArguments()[0]
            })
            .Select(x => new
            {
                x.handler,
                x.commandType,
                namespaceMatch = x.handler.Namespace == x.commandType.Namespace
            })
            .ToList();

        // Assert
        foreach (var check in namespaceResults)
        {
            check.namespaceMatch.Should().BeTrue(
                $"{check.handler.Name} should be in the same namespace as {check.commandType.Name}");
        }
    }

    [Fact]
    public void EveryRequest_ShouldHave_RegisteredHandler()
    {
        // Arrange
        var requests = ApplicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                        && t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .ToList();

        var handledRequestTypes = ApplicationAssembly.GetTypes()
            .SelectMany(t => t.GetInterfaces())
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .Select(i => i.GetGenericArguments()[0])
            .ToHashSet();

        // Act
        var orphans = requests
            .Where(r => !handledRequestTypes.Contains(r))
            .Select(r => r.FullName)
            .ToList();

        // Assert
        orphans.Should().BeEmpty("every IRequest<T> must have a matching IRequestHandler<T, _> in the Application assembly");
    }
}
