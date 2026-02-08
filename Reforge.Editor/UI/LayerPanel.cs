using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;

namespace Reforge.Editor.UI;

public class LayerPanel
{
    public void Draw(EditorContext ctx)
    {
        float posY = ctx.MenuBarHeight + ctx.HierarchyHeight + ctx.BrowserContentHeight;
        float remainingHeight = Raylib.GetScreenHeight() - posY;

        ImGui.SetNextWindowPos(new Vector2(0, posY), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(ctx.SidebarWidth, remainingHeight), ImGuiCond.Always);

        if (ImGui.Begin("Layer Control", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            int layer = ctx.CurrentLayer;
            if(ImGui.RadioButton("Background", ref layer, 0)) ctx.CurrentLayer = layer;
            if(ImGui.RadioButton("World", ref layer, 1)) ctx.CurrentLayer = layer;
            if(ImGui.RadioButton("Foreground", ref layer, 2)) ctx.CurrentLayer = layer;
        }
        ImGui.End();
    }
}
