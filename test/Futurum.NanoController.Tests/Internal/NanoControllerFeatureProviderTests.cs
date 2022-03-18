using System.Reflection;

using FluentAssertions;

using Futurum.Core.Linq;
using Futurum.NanoController.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

using Xunit;

namespace Futurum.NanoController.Tests.Internal;

public class NanoControllerFeatureProviderTests
{
    [Fact]
    public void when_is_NanoController()
    {
        var nanoControllerFeatureProvider = new NanoControllerFeatureProvider();

        var controllerFeature = new ControllerFeature();

        nanoControllerFeatureProvider.PopulateFeature(EnumerableExtensions.Return(new TestSuccessApplicationPartTypeProvider()), controllerFeature);

        controllerFeature.Controllers.Count.Should().Be(1);
        controllerFeature.Controllers.Single().Should().Be(typeof(TestNanoController).GetTypeInfo());
    }

    [Fact]
    public void when_not_NanoController()
    {
        var nanoControllerFeatureProvider = new NanoControllerFeatureProvider();

        var controllerFeature = new ControllerFeature();

        nanoControllerFeatureProvider.PopulateFeature(EnumerableExtensions.Return(new TestFailureApplicationPartTypeProvider()), controllerFeature);

        controllerFeature.Controllers.Should().BeEmpty();
    }

    public class TestSuccessApplicationPartTypeProvider : ApplicationPart, IApplicationPartTypeProvider
    {
        public IEnumerable<TypeInfo> Types => EnumerableExtensions.Return(typeof(TestNanoController).GetTypeInfo());
        public override string Name { get; }
    }

    public class TestFailureApplicationPartTypeProvider : ApplicationPart, IApplicationPartTypeProvider
    {
        public IEnumerable<TypeInfo> Types => EnumerableExtensions.Return(typeof(object).GetTypeInfo());
        public override string Name { get; }
    }

    public record RequestDto;
    public record ResponseDto;
    public class TestNanoController : NanoController.Request<RequestDto>.Response<ResponseDto>.Get
    {
        public TestNanoController(INanoControllerRouter router) : base(router)
        {
        }

        public override Task<ActionResult<ResponseDto>> GetAsync(RequestDto request, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}