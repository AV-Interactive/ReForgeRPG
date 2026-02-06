using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ReForge.Engine.World;

namespace ReForge.Engine.Core;

public static class SceneSerializer
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true,
        UnknownTypeHandling = (JsonUnknownTypeHandling)JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { CreateBehaviorResolver }
        }
    };

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
                    // 1. On restaure la texture
                    if (!string.IsNullOrEmpty(entity.TexturePath))
                    {
                        entity.Texture = engine.AssetManager.GetTexture(entity.TexturePath);
                    }

                    // 2. TRANSFERT CRUCIAL : Du JSON vers le moteur
                    // On récupère les Behaviors que le JSON a mis dans la propriété publique
                    if (entity.Behaviors != null)
                    {
                        // On crée une copie temporaire pour ne pas modifier la liste pendant qu'on boucle
                        var behaviorsToRestore = entity.Behaviors.ToList();
                    
                        // On vide la liste pour passer par AddBehavior (qui gère l'Owner et la liste privée)
                        entity.Behaviors.Clear(); 

                        foreach (var behavior in behaviorsToRestore)
                        {
                            // Cette méthode va remplir _behaviors ET mettre l'Owner
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
