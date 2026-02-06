using Request.Application.DTOs;
using Request.Application.ValueObjects;
using Request.Common.Paging;

namespace Request.Application.Interfaces;

public interface IRequestService
{
    Task<RequestResult> CreateRequest(CreateRequest newRequest);
    // Task<List<GetRequestResult>> GetRequests();
    Task<PagedResult<GetRequestResult>> GetRequests(GetRequestQuery query);
    Task<RequestResult> UpdateRequest(UpdateRequest updatedRequest);
    Task<RequestResult> DeleteRequest(int requestId);
}