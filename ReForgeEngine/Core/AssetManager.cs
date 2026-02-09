using Raylib_cs;

namespace ReForge.Engine.Core;

public class AssetManager
{
    Dictionary<string, Texture2D> _textures = new();
    
    public Texture2D GetTexture(string path)
    {
        if (!_textures.ContainsKey(path))
        {
            _textures[path] = Raylib.LoadTexture(path);
        }
        return _textures[path];
    }

    public void UnloadAll()
    {
        foreach (var texture in _textures.Values)
        {
            Raylib.UnloadTexture(texture);
        }
        _textures.Clear();
    }
}
