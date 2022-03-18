using Futurum.Core.Linq;
using Futurum.Core.Result;

namespace Futurum.NanoController;

public record DataCollectionDto<T>(ICollection<T> Data)
{
    public long Count => Data.Count;

    public static DataCollectionDto<T> Create(IEnumerable<T> data) => new(data.AsList());
    public static DataCollectionDto<T> Create(ICollection<T> data) => new(data);
}

public static class DataCollectionDtoExtensions
{
    public static Task<Result<DataCollectionDto<T>>> ToDataCollectionDtoAsync<T>(this Task<Result<IEnumerable<T>>> result) =>
        result.MapAsync(DataCollectionDto<T>.Create);
}