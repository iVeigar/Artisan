using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;

namespace Artisan.CraftingLogic.Solvers;

public class ThiriaSolverSettings
{
    public bool AllowSpecialistActions = true;
    public bool CheckDelineations = true; // 图纸
    [NonSerialized]
    public IDalamudTextureWrap? expertIcon;

    public ThiriaSolverSettings()
    {
        var tex = Svc.PluginInterface.UiBuilder.LoadUld("ui/uld/RecipeNoteBook.uld");
        expertIcon = tex.LoadTexturePart("ui/uld/RecipeNoteBook_hr1.tex", 14);
    }

    public bool Draw()
    {
        ImGui.TextWrapped($"The expert recipe solver is not an alternative to the standard solver. This is used exclusively with expert recipes.");
        if (expertIcon != null)
        {
            ImGui.TextWrapped($"This solver only applies to recipes with the");
            ImGui.SameLine();
            ImGui.Image(expertIcon.Handle, expertIcon.Size, new Vector2(0, 0), new Vector2(1, 1), new Vector4(0.94f, 0.57f, 0f, 1f));
            ImGui.SameLine();
            ImGui.TextWrapped($"icon in the crafting log.");
        }
        bool changed = false;
        changed |= ImGui.Checkbox($"允许专家技能", ref AllowSpecialistActions);
        ImGuiComponents.HelpMarker("勾选后若是专家职业则求解器会考虑使用专家技能");
        ImGui.Indent();
        changed |= ImGui.Checkbox($"检查能工巧匠图纸数量", ref CheckDelineations);
        ImGuiComponents.HelpMarker("勾选后，至少持有5个图纸时求解器才会考虑使用专家技能");
        ImGui.Unindent();
        return changed;
    }
}
