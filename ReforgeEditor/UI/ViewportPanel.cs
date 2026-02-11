using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World.Behaviors;

namespace Reforge.Editor.UI;

public class ViewportPanel
{
    RenderTexture2D _viewportRes;
    Camera2D _worldCamera;
    
    public Vector2 WindowPosition { get; private set; }

    public void Draw(Engine engine, EditorContext ctx, EditorApp app)
    {
        float windowWidth = Raylib.GetScreenWidth() - ctx.SidebarWidth - ctx.InspectorWidth;
        float windowHeight = Raylib.GetScreenHeight() - ctx.MenuBarHeight;
        
        UpdateResolution(windowWidth, windowHeight);

        if (_viewportRes.Id == 0) return;

        // Gestion du Zoom
        if (ImGui.IsWindowHovered() && ImGui.GetIO().MouseWheel != 0)
        {
            ctx.Zoom += ImGui.GetIO().MouseWheel * 0.1f;
            if (ctx.Zoom < 0.1f) ctx.Zoom = 0.1f;
            if (ctx.Zoom > 10.0f) ctx.Zoom = 10.0f;
        }

        Vector2 mousePos = ImGui.GetMousePos();
        Vector2 relativeMousePos = mousePos - WindowPosition;

        // Calcul de la position dans le monde en tenant compte du zoom et du target de la caméra
        // En mode 2D, ScreenToWorld est nécessaire. Raylib propose GetScreenToWorld2D.
        // Mais nous sommes dans une texture.
        
        _worldCamera.Offset = Vector2.Zero;
        _worldCamera.Zoom = ctx.Zoom;
        _worldCamera.Rotation = 0;

        // Déplacement par flèches (vitesse arbitraire)
        float panSpeed = 200f / ctx.Zoom; // Plus on zoom, plus le déplacement clavier est précis
        float dt = Raylib.GetFrameTime();
        if (ImGui.IsKeyDown(ImGuiKey.RightArrow)) ctx.CameraTarget += new Vector2(panSpeed * dt, 0);
        if (ImGui.IsKeyDown(ImGuiKey.LeftArrow)) ctx.CameraTarget -= new Vector2(panSpeed * dt, 0);
        if (ImGui.IsKeyDown(ImGuiKey.UpArrow)) ctx.CameraTarget -= new Vector2(0, panSpeed * dt);
        if (ImGui.IsKeyDown(ImGuiKey.DownArrow)) ctx.CameraTarget += new Vector2(0, panSpeed * dt);

        // Déplacement par Pan (Touchpad / Bouton milieu souris)
        if (ImGui.IsWindowHovered() && (ImGui.IsMouseDown(ImGuiMouseButton.Middle) || ImGui.IsMouseDown(ImGuiMouseButton.Right)))
        {
            Vector2 delta = ImGui.GetIO().MouseDelta;
            if (delta != Vector2.Zero)
            {
                ctx.CameraTarget -= delta / ctx.Zoom;
            }
        }

        var cameraEntity = engine.CurrentScene.Entities.FirstOrDefault(e => e.GetBehavior<CameraFollow>() != null);
        if (ctx.State == EditorApp.EditorState.Playing && cameraEntity != null)
        {
            _worldCamera.Target = cameraEntity.Position;
            _worldCamera.Offset = new Vector2(windowWidth / 2, windowHeight / 2);
        }
        else
        {
            _worldCamera.Target = ctx.CameraTarget;
        }

        Vector2 worldMousePos = Raylib.GetScreenToWorld2D(relativeMousePos, _worldCamera);
        Vector2 snappedPos = EditorMath.SnapToGrid(worldMousePos);
        
        var hoveredEntity = ctx.EditorSelector.GetEntityAt(engine.CurrentScene, worldMousePos, ctx.CurrentLayer);

        Raylib.BeginTextureMode(_viewportRes);
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_worldCamera);
            
            engine.ExternalCameraActive = true;
            engine.Render();
            engine.ExternalCameraActive = false;

            if (ctx.State == EditorApp.EditorState.Editing)
            {
                ctx.MapPainter.DrawGrid(_viewportRes, _worldCamera);
                
                // On dessine aussi les entités vides en mode caméra si on est en train d'éditer
                foreach (var entity in engine.CurrentScene.Entities)
                {
                    var sprite = entity.Sprite;
                    if (sprite == null || sprite.Texture.Id == 0)
                    {
                        Rectangle rect = new Rectangle(
                            entity.Position.X, 
                            entity.Position.Y, 
                            EditorConfig.TileSize, 
                            EditorConfig.TileSize
                        );
                
                        Raylib.DrawRectangleRec(rect, Raylib.Fade(Color.SkyBlue, 0.4f));
                        Raylib.DrawRectangleLinesEx(rect, 1, Raylib.Fade(Color.SkyBlue, 0.9f));
                    }
                }

                ctx.MapPainter.DrawPreview(engine, ctx);
                
                foreach (var entity in ctx.SelectedEntities)
                {
                    bool isHovered = (entity == hoveredEntity);
                    ctx.Gizmo.Draw(entity, isHovered);
                }
            }
            
        Raylib.EndMode2D();
        
        Raylib.EndTextureMode();
        
        ImGui.SetNextWindowPos(new Vector2(ctx.SidebarWidth, ctx.MenuBarHeight));
        ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));

        if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            WindowPosition = ImGui.GetCursorScreenPos();
            
            app.HandleEditorTools(WindowPosition, worldMousePos);

            IntPtr textureId = (IntPtr)_viewportRes.Texture.Id;
            ImGui.Image(textureId, new Vector2(windowWidth, windowHeight), new Vector2(0, 1), new Vector2(1, 0));

            // Indication du zoom en overlay
            var drawList = ImGui.GetWindowDrawList();
            Vector2 overlayPos = WindowPosition + new Vector2(10, 10);
            
            string zoomText = $"Zoom: {ctx.Zoom:P0}";
            drawList.AddText(overlayPos, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.8f)), zoomText);
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
