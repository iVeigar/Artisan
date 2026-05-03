using System;
using Wasmtime;

namespace ThiriaExpertSolver.Thiria;

public class ThiriaSession
{
    //private readonly Instance instance;
    private readonly Memory memory;
    public readonly ExportedFunctions exports;
    private readonly ActionWrapper actions;
    private readonly ConditionWrapper conditions;
    private int id;
    public bool Started => id > 0;
    public ThiriaSession(Instance instance)
    {
        //this.instance = instance;
        memory = instance.GetMemory("memory")!;
        exports = new(instance);
        actions = new(this);
        conditions = new(this);
    }

    public static ThiriaSession Create()
    {
        return WasmRunner.CreateSession();
    }
    public bool Start()
    {
        id = exports.request_create();
        return id > 0;
    }
    public string PtrToString(int ptr) => memory.ReadNullTerminatedString(ptr);

    public void SetPlayer(int level, int cp, int craft, int control, double goodMultiplier, bool manipulationAllowed, bool specialistAllowed)
    {
        try
        {
            if (level < 70)
            {
                throw new Exception("[SetPlayer] Must be at least level 70.");
            }
            exports.setPlayer(level, cp, craft, control, goodMultiplier, manipulationAllowed ? 1 : 0, specialistAllowed ? 1 : 0);
        }
        catch (Exception ex)
        {
            throw new Exception("[SetPlayer] Could not configure Solver.", ex);
        }
    }

    public void SetItem(int level, int durability, int progress, int quality, float progressDivider, float progressModifier, float qualityDivider, float qualityModifier, bool forceExpert)
    {
        try
        {
            exports.setItem(level, durability, progress, quality, progressDivider, progressModifier, qualityDivider, qualityModifier, forceExpert ? 1 : 0);
        }
        catch (Exception ex)
        {
            throw new Exception("[SetItem] Could not configure Solver.", ex);
        }
    }

    public string GetActionName()
    {
        return PtrToString(exports.request_getActionName(id));
    }

    public int? GetActionIndex(string name)
    {
        return actions.GetIndex(name);
    }

    public int? GetConditionIndex(string name)
    {
        return conditions.GetIndex(name);
    }

    public bool Recommend()
    {
        return exports.request_recommend(id) > 0;
    }

    public (double success, double step, double hp)? Simulate(bool firstStep)
    {
        if (exports.request_simulate(id, firstStep ? 10000 : 1000) > 0)
        {
            var success = exports.request_getSimulationSuccess(id);
            var step = exports.request_getSimulationSteps(id);
            var hq = exports.request_getSimulationHQ(id);
            return (success, step, hq);
        }
        return null;
    }

    public bool Perform(int actionIndex, bool success)
    {
        return exports.request_perform(id, actionIndex, success ? 1 : 0) > 0;
    }

    // 没用的函数
    public bool PerformRecommended(bool unknown)
    {
        return exports.request_performRecommended(id, unknown ? 1 : 0) > 0;
    }

    public int GetProgress()
    {
        return exports.request_getProgress(id);
    }

    public int GetQuality()
    {
        return exports.request_getQuality(id);
    }

    public int GetCP()
    {
        return exports.request_getCP(id);
    }

    public int GetDurability()
    {
        return exports.request_getDurability(id);
    }

    public bool GetHasNormalFinisher()
    {
        return exports.request_getHasNormalFinisher(id) > 0;
    }

    public bool GetHasPanicFinisher()
    {
        return exports.request_getHasPanicFinisher(id) > 0;
    }

    public int GetFinisherQuality()
    {
        return exports.request_getFinisherQuality(id);
    }

    public bool SetConditionIndex(int conditionIndex)
    {
        return exports.request_setConditionIndex(id, conditionIndex) > 0;
    }

    public bool ApplyStep(ThiriaStep step)
    {
        if (string.IsNullOrEmpty(step.actionName))
            return false;
        var actionIndex = actions.GetIndex(step.actionName);
        if (actionIndex is null)
            return false;
        int? conditionIndex = null;
        if (step.condition is not null)
        {
            conditionIndex = conditions.GetIndex(step.condition);
            if (conditionIndex is null)
                return false;
        }
        // Recommend() &&
        if (!Perform(actionIndex.Value, step.success ?? true))
            return false;
        if (conditionIndex.HasValue && !SetConditionIndex(conditionIndex.Value))
            return false;
        return true;
    }

    public ThiriaSolverResult Request(ThiriaStep? step)
    {
        if (id == 0)
        {
            return new ThiriaSolverResult
            {
                error = true,
                message = "必须先初始化求解器"
            };
        }
        try
        {
            if (step != null)
            {
                var success = ApplyStep(step);
                if (!success)
                {
                    return new ThiriaSolverResult
                    {
                        error = true,
                        message = "求解器无法应用提供的步骤"
                    };
                }
            }

            //(double success, double step, double hq)? ret = Simulate(step == null);

            if (!Recommend())
            {
                return new ThiriaSolverResult { done = true, message = "制作完成，无更多建议" };
            }

            //var msg = "";
            //if (ret != null)
            //{
            //    msg = $"{Percent(ret.Value.success)}% 完成率";
            //    if (ret.Value.hq >= 0)
            //        msg += $"  ({Percent(ret.Value.hq)}% HQ)";
            //}

            var actionName = GetActionName();
            return new ThiriaSolverResult { actionName = actionName, message = "" };
        }
        catch (Exception)
        {
            return new ThiriaSolverResult
            {
                error = true,
                message = "求解器出错，无法给出建议"
            };
        }
    }
    private static string Percent(double t)
    {
        var e = Math.Floor(100 * t);
        return e >= 100 ? ">99" : e <= 0 ? "<1" : e.ToString();
    }
}
