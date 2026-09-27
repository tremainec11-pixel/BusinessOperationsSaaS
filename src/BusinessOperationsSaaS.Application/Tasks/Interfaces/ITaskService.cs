using BusinessOperationsSaaS.Application.Tasks.DTOs;

namespace BusinessOperationsSaaS.Application.Tasks.Interfaces;

public interface ITaskService
{
    Task<List<TaskResponse>> GetAllAsync(Guid companyId);

    Task<TaskResponse?> GetByIdAsync(
        Guid companyId,
        Guid taskId);

    Task<TaskResponse> CreateAsync(
        Guid companyId,
        CreateTaskRequest request);

    Task<TaskResponse?> UpdateAsync(
        Guid companyId,
        Guid taskId,
        UpdateTaskRequest request);

    Task<bool> DeleteAsync(
        Guid companyId,
        Guid taskId);
}

