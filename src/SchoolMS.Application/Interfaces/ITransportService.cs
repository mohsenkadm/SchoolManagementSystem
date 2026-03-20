using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface ITransportService
{
    Task<List<TransportRouteDto>> GetAllRoutesAsync();
    Task<TransportRouteDto?> GetRouteByIdAsync(int id);
    Task<TransportRouteDto> CreateRouteAsync(TransportRouteDto dto);
    Task<TransportRouteDto> UpdateRouteAsync(TransportRouteDto dto);
    Task DeleteRouteAsync(int id);
}

