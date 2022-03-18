using Futurum.Core.Result;

namespace Futurum.NanoController;

public interface IMapperDto<TDto, TDomain>
{
    TDto MapToDto(TDomain domain);
    
    Result<TDomain> MapToDomain(TDto dto);
}