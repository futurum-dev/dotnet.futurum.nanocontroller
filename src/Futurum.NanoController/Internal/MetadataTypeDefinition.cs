namespace Futurum.NanoController.Internal;

public record MetadataTypeDefinition(Type RequestType, Type ResponseType, Type HandlerInterfaceType, Type HandlerType);