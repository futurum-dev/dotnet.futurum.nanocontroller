using FluentAssertions;

using Xunit;

namespace Futurum.NanoController.Tests;

public class DataCollectionDtoTests
{
    [Fact]
    public void Create_IEnumerable()
    {
        var numbers = Enumerable.Range(0, 10);

        var dataCollectionDto = DataCollectionDto<int>.Create(numbers);

        dataCollectionDto.Count.Should().Be(numbers.Count());
        dataCollectionDto.Data.Should().BeEquivalentTo(numbers);
    }

    [Fact]
    public void Create_ICollection()
    {
        var numbers = Enumerable.Range(0, 10).ToList();

        var dataCollectionDto = DataCollectionDto<int>.Create(numbers);

        dataCollectionDto.Count.Should().Be(numbers.Count);
        dataCollectionDto.Data.Should().BeEquivalentTo(numbers);
    }
}