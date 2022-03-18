using System.Reflection;

using Futurum.Core.Option;
using Futurum.Core.Result;

namespace Futurum.NanoController.Internal;

public interface IMetadataCache
{
    IEnumerable<MetadataTypeDefinition> MetadataTypeDefinitions { get; }

    void Register(IEnumerable<Assembly> assemblies);

    Result<Type> Get<TResponse>(INanoControllerRequest<TResponse> request);
}

public class MetadataCache : IMetadataCache
{
    private readonly List<MetadataTypeDefinition> _metadataTypeDefinitions = new();
    private readonly Dictionary<Type, Type> _cache = new();

    public IEnumerable<MetadataTypeDefinition> MetadataTypeDefinitions => _metadataTypeDefinitions.AsReadOnly();

    public void Register(IEnumerable<Assembly> assemblies)
    {
        foreach (var metadataTypeDefinition in MetadataTypeService.Get(assemblies))
        {
            Register(metadataTypeDefinition);
        }
    }

    private void Register(MetadataTypeDefinition metadataTypeDefinition)
    {
        _cache.Add(metadataTypeDefinition.RequestType, typeof(INanoControllerDispatcher<,>).MakeGenericType(metadataTypeDefinition.RequestType, metadataTypeDefinition.ResponseType));

        _metadataTypeDefinitions.Add(metadataTypeDefinition);
    }

    public Result<Type> Get<TResponse>(INanoControllerRequest<TResponse> request) =>
        _cache.TryGetValue(request.GetType(), () => $"Unable to get Router Dispatcher type for - '{request.GetType().FullName}'");
}