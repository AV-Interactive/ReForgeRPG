using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using Reforge.Editor.Tools;
using ReForge.Engine.Core;

namespace Reforge.Editor.UI;

public class ViewportPanel
{
    RenderTexture2D _viewportRes;
    public Vector2 WindowPosition { get; private set; }

    public void Draw(Engine engine, EditorContext ctx, EditorApp app)
    {
        float windowWidth = Raylib.GetScreenWidth() - ctx.SidebarWidth - ctx.InspectorWidth;
        float windowHeight = Raylib.GetScreenHeight() - ctx.MenuBarHeight;
        
        UpdateResolution(windowWidth, windowHeight);

        if (_viewportRes.Id == 0) return;
        
        Raylib.BeginTextureMode(_viewportRes);
        engine.Render();

        if (ctx.State == EditorApp.EditorState.Editing)
        {
            ctx.MapPainter.DrawGrid(_viewportRes);
            ctx.MapPainter.DrawPreview(engine);

            if (ctx.Hierarchy.SelectedEntity != null)
            {
                ctx.Gizmo.Draw(ctx.Hierarchy.SelectedEntity);
            }
        }
        
        Raylib.EndTextureMode();
        
        ImGui.SetNextWindowPos(new Vector2(ctx.SidebarWidth, ctx.MenuBarHeight));
        ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));

        if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            WindowPosition = ImGui.GetWindowPos();
            
            app.HandleEditorTools(WindowPosition);

            IntPtr textureId = (IntPtr)_viewportRes.Texture.Id;
            ImGui.Image(textureId, new Vector2(windowWidth, windowHeight), new Vector2(0, 1), new Vector2(1, 0));
        }
        
        ImGui.End();
    }

    void UpdateResolution(float width, float height)
    {
        if (width <= 0 || height <= 0) return;

        if (_viewportRes.Id == 0 || width != _viewportRes.Texture.Width || height != _viewportRes.Texture.Height)
        {
            if (_viewportRes.Id != 0) 
            {
                Raylib.UnloadRenderTexture(_viewportRes);
            }
            _viewportRes = Raylib.LoadRenderTexture((int)width, (int)height);
        }
    }
}
