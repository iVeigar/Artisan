using System;
using Wasmtime;

namespace ThiriaExpertSolver.Thiria;
public class ExportedFunctions
{
    public readonly Action<int, int, int, int, double, int, int> setPlayer; // level cp 作业 加工 红球倍率 掌握 专家
    public readonly Action<int, int, int, int, float, float, float, float, int> setItem;  // 物品等级 耐久 进展 品质 pdiv plevel qdiv qlevel safety
    public readonly Func<int> action_getNumElements; // return count
    public readonly Func<int, int> action_getShortName; // return a ptr
    public readonly Func<int> condition_getNumElements; // return count
    public readonly Func<int, int> condition_getShortName; // return a ptr
    public readonly Func<int> request_create; // return session id
    //public readonly Action request_delete;
    public readonly Func<int, int> request_getActionName;
    public readonly Func<int, int> request_recommend;
    public readonly Func<int, int, int> request_simulate; // session_id, 1e4（第一步）或1e3（后续）
    public readonly Func<int, double> request_getSimulationSuccess;
    public readonly Func<int, double> request_getSimulationSteps;
    public readonly Func<int, double> request_getSimulationHQ;
    public readonly Func<int, int, int, int> request_perform;
    public readonly Func<int, int, int> request_performRecommended;
    public readonly Func<int, int> request_getProgress;
    public readonly Func<int, int> request_getQuality;
    public readonly Func<int, int> request_getCP;
    public readonly Func<int, int> request_getDurability;
    public readonly Func<int, int> request_getHasNormalFinisher;
    public readonly Func<int, int> request_getHasPanicFinisher;
    public readonly Func<int, int> request_getFinisherQuality;
    public readonly Func<int, int, int> request_setConditionIndex;
    //public readonly Action _initialize;
    public ExportedFunctions(Instance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        //_initialize = instance.GetAction(nameof(_initialize)) ?? throw new Exception($"GetFunction({nameof(_initialize)}) failed!");

        setPlayer = instance.GetFunction(nameof(setPlayer))?.WrapAction<int, int, int, int, double, int, int>() ?? throw new Exception($"GetFunction({nameof(setPlayer)}) failed!");
        setItem = instance.GetFunction(nameof(setItem))?.WrapAction<int, int, int, int, float, float, float, float, int>() ?? throw new Exception($"GetFunction({nameof(setItem)}) failed!");
        
        action_getNumElements = instance.GetFunction<int>(nameof(action_getNumElements)) ?? throw new Exception($"GetFunction({nameof(action_getNumElements)}) failed!");
        action_getShortName = instance.GetFunction<int, int>(nameof(action_getShortName)) ?? throw new Exception($"GetFunction({nameof(action_getShortName)}) failed!");
        
        condition_getNumElements = instance.GetFunction<int>(nameof(condition_getNumElements)) ?? throw new Exception($"GetFunction({nameof(condition_getNumElements)}) failed!");
        condition_getShortName = instance.GetFunction<int, int>(nameof(condition_getShortName)) ?? throw new Exception($"GetFunction({nameof(condition_getShortName)}) failed!");

        request_create = instance.GetFunction<int>(nameof(request_create)) ?? throw new Exception($"GetFunction({nameof(request_create)}) failed!");
        //request_delete = instance.GetAction(nameof(request_delete)) ?? throw new Exception($"GetFunction({nameof(request_delete)}) failed!");
        request_getActionName = instance.GetFunction<int, int>(nameof(request_getActionName)) ?? throw new Exception($"GetFunction({nameof(request_getActionName)}) failed!");
        request_recommend = instance.GetFunction<int, int>(nameof(request_recommend)) ?? throw new Exception($"GetFunction({nameof(request_recommend)}) failed!");
        request_simulate = instance.GetFunction<int, int, int>(nameof(request_simulate)) ?? throw new Exception($"GetFunction({nameof(request_simulate)}) failed!");
        request_getSimulationSuccess = instance.GetFunction<int, double>(nameof(request_getSimulationSuccess)) ?? throw new Exception($"GetFunction({nameof(request_getSimulationSuccess)}) failed!");
        request_getSimulationSteps = instance.GetFunction<int, double>(nameof(request_getSimulationSteps)) ?? throw new Exception($"GetFunction({nameof(request_getSimulationSteps)}) failed!");
        request_getSimulationHQ = instance.GetFunction<int, double>(nameof(request_getSimulationHQ)) ?? throw new Exception($"GetFunction({nameof(request_getSimulationHQ)}) failed!");
        request_perform = instance.GetFunction<int, int, int, int>(nameof(request_perform)) ?? throw new Exception($"GetFunction({nameof(request_perform)}) failed!");
        request_performRecommended = instance.GetFunction<int, int, int>(nameof(request_performRecommended)) ?? throw new Exception($"GetFunction({nameof(request_performRecommended)}) failed!");
        request_getProgress = instance.GetFunction<int, int>(nameof(request_getProgress)) ?? throw new Exception($"GetFunction({nameof(request_getProgress)}) failed!");
        request_getQuality = instance.GetFunction<int, int>(nameof(request_getQuality)) ?? throw new Exception($"GetFunction({nameof(request_getQuality)}) failed!");
        request_getCP = instance.GetFunction<int, int>(nameof(request_getCP)) ?? throw new Exception($"GetFunction({nameof(request_getCP)}) failed!");
        request_getDurability = instance.GetFunction<int, int>(nameof(request_getDurability)) ?? throw new Exception($"GetFunction({nameof(request_getDurability)}) failed!");
        request_getHasNormalFinisher = instance.GetFunction<int, int>(nameof(request_getHasNormalFinisher)) ?? throw new Exception($"GetFunction({nameof(request_getHasNormalFinisher)}) failed!");
        request_getHasPanicFinisher = instance.GetFunction<int, int>(nameof(request_getHasPanicFinisher)) ?? throw new Exception($"GetFunction({nameof(request_getHasPanicFinisher)}) failed!");
        request_getFinisherQuality = instance.GetFunction<int, int>(nameof(request_getFinisherQuality)) ?? throw new Exception($"GetFunction({nameof(request_getFinisherQuality)}) failed!");
        request_setConditionIndex = instance.GetFunction<int, int, int>(nameof(request_setConditionIndex)) ?? throw new Exception($"GetFunction({nameof(request_setConditionIndex)}) failed!");
    }
}


