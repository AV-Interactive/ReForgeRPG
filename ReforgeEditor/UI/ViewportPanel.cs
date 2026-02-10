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
        
        Vector2 mousePos = ImGui.GetMousePos();
        Vector2 relativeMousePos = mousePos - WindowPosition;
        Vector2 snappedPos = EditorMath.SnapToGridRelativePos(relativeMousePos);
        var hoveredEntity = ctx.EditorSelector.GetEntityAt(engine.CurrentScene, snappedPos, ctx.CurrentLayer);

        Raylib.BeginTextureMode(_viewportRes);
        
        var cameraEntity = engine.CurrentScene.Entities.FirstOrDefault(e => e.GetBehavior<CameraFollow>() != null);

        if (cameraEntity != null && ctx.State != EditorApp.EditorState.Editing)
        {
            _worldCamera.Target = cameraEntity.Position;
            _worldCamera.Offset = new Vector2(windowWidth / 2, windowHeight / 2);
            _worldCamera.Zoom = 1.0f;
            _worldCamera.Rotation = 0;
            
            Raylib.BeginMode2D(_worldCamera);
            
                engine.ExternalCameraActive = true;
                engine.Render();
                engine.ExternalCameraActive = false;

                if (ctx.State == EditorApp.EditorState.Editing)
                {
                    ctx.MapPainter.DrawGrid(_viewportRes);
                    
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
        }
        else
        {
            engine.ExternalCameraActive = ctx.State == EditorApp.EditorState.Editing;
            engine.Render();
            engine.ExternalCameraActive = false;

            if (ctx.State == EditorApp.EditorState.Editing)
            {
                ctx.MapPainter.DrawGrid(_viewportRes);
        
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
        }
        
        Raylib.EndTextureMode();
        
        ImGui.SetNextWindowPos(new Vector2(ctx.SidebarWidth, ctx.MenuBarHeight));
        ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));

        if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            WindowPosition = ImGui.GetCursorScreenPos();
            
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
