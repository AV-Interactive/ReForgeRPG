using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ReForge.Engin.Core;
using ReForge.Engine.World;
using ReForge.Engine.World.Components;

namespace ReForge.Engine.Core;

public static class SceneSerializer
{
    static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true,
        UnknownTypeHandling = (JsonUnknownTypeHandling)JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { CreateBehaviorResolver, CreateEntityResolver }
        }
    };

    static void CreateEntityResolver(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type == typeof(Entity))
        {
            var polymorphismOptions = new JsonPolymorphismOptions();
        
            polymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Tilemap), "Tilemap"));
            
            typeInfo.PolymorphismOptions = polymorphismOptions;
        }
    }

    static void CreateBehaviorResolver(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type == typeof(Behavior))
        {
            var behaviorTypes = Assembly.GetAssembly(typeof(Behavior))!
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Behavior)) && !t.IsAbstract);
            
            var polymorphismOptions = new JsonPolymorphismOptions();

            foreach (var type in behaviorTypes)
            {
                polymorphismOptions.DerivedTypes.Add(new JsonDerivedType(type, type.Name));
            }

            typeInfo.PolymorphismOptions = polymorphismOptions;
        }
    }

    public static void Save(Scene scene, string filePath)
    {
        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            string json = JsonSerializer.Serialize(scene.Entities, _options);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Scène sauvegardée avec succès !");
        } catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la sauvegarde du fichier {filePath}: {e.Message}");
        }
    }
    
    public static void Load(Scene scene, Engine engine, string filePath)
    {
        if (!File.Exists(filePath)) return;

        try
        {
            string json = File.ReadAllText(filePath);
            var loadedEntities = JsonSerializer.Deserialize<List<Entity>>(json, _options);

            if (loadedEntities != null)
            {
                scene.Entities.Clear();

                foreach (var entity in loadedEntities)
                {
                    // Réhydratation de la texture via chemin relatif
                    if (entity.Sprite != null && !string.IsNullOrEmpty(entity.Sprite.TexturePath))
                    {
                        // On combine avec le root du projet pour trouver le fichier
                        string fullPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, entity.Sprite.TexturePath);
                        entity.Sprite.Texture = engine.AssetManager.GetTexture(fullPath);
                        
                        // Synchronisation spécifique Tilemap: la texture interne de rendu doit être renseignée
                        if (entity is Tilemap tilemap)
                        {
                            tilemap.Texture = entity.Sprite.Texture;
                            if (tilemap.TileSize <= 0) 
                            {
                                tilemap.TileSize = ProjectManager.CurrentProject.TileSize;
                            }
                        }
                    }

                    if (entity.Behaviors != null)
                    {
                        var behaviorsToRestore = entity.Behaviors.ToList();
                    
                        entity.Behaviors.Clear(); 

                        foreach (var behavior in behaviorsToRestore)
                        {
                            entity.AddBehavior(behavior); 
                        }
                    }

                    scene.AddEntity(entity);
                }
                Console.WriteLine("Scène chargée et Behaviors réhydratés !");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur : {e.Message}");
        }
    }
}
