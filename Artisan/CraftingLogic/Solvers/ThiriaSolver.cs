using System;
using System.Collections.Generic;
using System.Text.Json;
using Artisan.CraftingLogic.CraftData;
using Artisan.RawInformation.Character;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using ThiriaExpertSolver.Thiria;
namespace Artisan.CraftingLogic.Solvers;

public class ThiriaSolverDefinition : ISolverDefinition
{
    public string MouseoverDescription { get; set; } = "This is for expert recipes, it is not a more advanced standard recipe solver.";

    public IEnumerable<ISolverDefinition.Desc> Flavours(CraftState craft)
    {
        if (craft.CraftExpert)
            yield return new(this, 0, 0, "Thiria Expert Solver", craft.StatLevel < 70 ? "Requires Level 70" : "");
    }

    public Solver Create(CraftState craft, int flavour) => new ThiriaSolver();

    public IEnumerable<ISolverDefinition.Desc> Flavours()
    {
        yield return default;
    }
}

public class ThiriaSolver : Solver
{
    private readonly ThiriaSession session;
    private readonly Solver? _fallback;
    public ThiriaSolver()
    {
        session = ThiriaSession.Create();
        _fallback = new ExpertSolver();
    }
    public override Solver Clone() => new ThiriaSolver();

    public override Recommendation Solve(CraftState craft, StepState step) => SolveNextStep(P.Config.ThiriaSolverConfig, craft, step);

    private static string SkillsName(Skills action)
    {
        return action switch
        {
            Skills.None or Skills.TouchCombo or Skills.TouchComboRefined => "invalid",
            Skills.TricksOfTrade => "tricksOfTheTrade",
            Skills.MaterialMiracle => "invalid", // todo 奇迹之材会立即改变球色
            Skills.SteadyHand => "stellarSteadyHand",
            _ => JsonNamingPolicy.CamelCase.ConvertName(action.ToString())
        };
    }
    private static bool IsInitialState(CraftState craft, StepState step)
    {
        return step.Index == 1 // 工次
            && step.Condition == Condition.Normal // 白球
            && craft.StatCP == step.RemainingCP // 最终确认不加工次但消耗cp
            && (!craft.Specialist || // 专家技能不加工次不消耗cp
                (!step.HeartAndSoulActive // 未使用专心致志
                && step.InnovationLeft == 0)); // 未使用快速改革
                // && step.CarefulObservationLeft == 3)); // 设计变动使用次数无法被检测到
    }
    private static unsafe bool HasEnoughDelineations() =>
        InventoryManager.Instance()->GetInventoryItemCount(28724) >= 5;
    public Recommendation SolveNextStep(ThiriaSolverSettings cfg, CraftState craft, StepState step)
    {
        ThiriaSolverResult? result = null;
        if (IsInitialState(craft, step))
        {
            Svc.Log.Debug("初始化求解器");
            var specialistAllowed = cfg.AllowSpecialistActions && craft.Specialist && (!cfg.CheckDelineations || HasEnoughDelineations());
            session.SetPlayer(
                craft.StatLevel,
                craft.StatCP,
                craft.StatCraftsmanship,
                craft.StatControl,
                craft.SplendorCosmic ? 1.75 : 1.5,
                craft.UnlockedManipulation,
                specialistAllowed);

            session.SetItem(
                craft.CraftLevel,
                craft.CraftDurability,
                craft.CraftProgress,
                craft.CraftQualityMax, // todo 考虑CraftRequiredQuality
                craft.CraftProgressDivider / 100f,
                craft.CraftProgressModifier / 100f,
                craft.CraftQualityDivider / 100f,
                craft.CraftQualityModifier / 100f,
                true,
                (!craft.MissionHasSteadyHand || craft.CurrentSteadyHandCharges < 1) ? -1 : 0);

            if (!session.Start())
                throw new Exception(result?.message ?? "求解器会话启动失败");
            result = session.Request(null);
        }
        else if (session.Started)
        {
            result = session.Request(new()
            {
                actionName = SkillsName(step.PrevComboAction),
                condition = JsonNamingPolicy.CamelCase.ConvertName(step.Condition.ToString()),
                success = !step.PrevActionFailed
            });
        }
        else
        {
            Svc.Log.Error("求解器未成功初始化，无法继续求解");
            Svc.Log.Error($"step.Index = {step.Index} (expect 1)\nstep.Condition = {step.Condition} (expect Normal)\ncraft.StatCP {(craft.StatCP == step.RemainingCP ? "" : "!")}= step.RemainingCP (expect ==)\ncraft.Specialist = {craft.Specialist}{(craft.Specialist ? $"\nstep.HeartAndSoulActive = {step.HeartAndSoulActive} (expect false)\nstep.InnovationLeft = {step.InnovationLeft} (expect 0)" : "")}");
            return new(Skills.None, "似乎不是从零开始求解，不会给出建议");
        }

        if (!result.actionName.IsNullOrEmpty())
        {
            if (result.actionName.Equals("TricksOfTheTrade", StringComparison.OrdinalIgnoreCase))
                result.actionName = "TricksOfTrade";
            else if (result.actionName.Equals("stellarSteadyHand", StringComparison.OrdinalIgnoreCase))
                result.actionName = "SteadyHand";
            return new(Enum.Parse<Skills>(result.actionName, true), result.message.IsNullOrEmpty() ? "thiria" : result.message);
        }
        else
        {
            var _fallbackRec = _fallback.Solve(craft, step);
            _fallbackRec.Comment = "fallback to built-in expert solver";
            return _fallbackRec;
        }
    }
}
