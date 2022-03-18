using FluentAssertions;

using Futurum.Core.Result;
using Futurum.Test.Result;

using Xunit;

namespace Futurum.NanoController.Tests;

public class DataCollectionDtoExtensionsTests
{
    [Fact]
    public async Task ToDataCollectionDtoAsync()
    {
        var numbers = Enumerable.Range(0, 10);

        var dataCollectionDto = await numbers.ToResultOkAsync().ToDataCollectionDtoAsync();

        dataCollectionDto.ShouldBeSuccessWithValueAssertion(x => x.Count.Should().Be(numbers.Count()));
        dataCollectionDto.ShouldBeSuccessWithValueAssertion(x => x.Data.Should().BeEquivalentTo(numbers));
    }
}