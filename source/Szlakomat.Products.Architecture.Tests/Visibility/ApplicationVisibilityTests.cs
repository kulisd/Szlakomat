using MediatR;
using Szlakomat.Products.Architecture.Tests.SeedWork;

namespace Szlakomat.Products.Architecture.Tests.Visibility;

public class ApplicationVisibilityTests : TestBase
{
    [Fact]
    public void CommandsAndQueries_ShouldBe_PublicRecords()
    {
        // Arrange
        var commandTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .ToList();
        commandTypes.Should().NotBeEmpty("there should be commands/queries in the application layer");

        // Act
        var nonPublicCommands = commandTypes.Where(t => !t.IsPublic).ToList();

        // Assert
        nonPublicCommands.Should().BeEmpty(
            $"these commands/queries should be public: {string.Join(", ", nonPublicCommands.Select(t => t.Name))}");
    }

    [Fact]
    public void Handlers_ShouldBe_InternalSealed()
    {
        // Arrange
        var handlerTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();
        handlerTypes.Should().NotBeEmpty("there should be handlers in the application layer");

        // Act
        var publicHandlers = handlerTypes.Where(t => t.IsPublic).ToList();
        var unsealedHandlers = handlerTypes.Where(t => !t.IsSealed).ToList();

        // Assert
        publicHandlers.Should().BeEmpty(
            $"these handlers should be internal: {string.Join(", ", publicHandlers.Select(t => t.Name))}");
        unsealedHandlers.Should().BeEmpty(
            $"these handlers should be sealed: {string.Join(", ", unsealedHandlers.Select(t => t.Name))}");
    }
}
