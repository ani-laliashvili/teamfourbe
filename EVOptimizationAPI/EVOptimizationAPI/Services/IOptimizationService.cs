using EVOptimizationAPI.Dtos;

namespace EVOptimizationAPI.Services
{
    public interface IEVOptimizationService
    {
        OptimizationResultDto OptimizeCharging(OptimizationInputDto input);
    }
}
