# Tutoriel : Ajouter des comportements personnalisés

Le moteur ReForgeRPG est conçu pour être facilement étendu par le code. Voici comment créer votre propre script.

## 1. Création de la classe

Tous les comportements doivent hériter de la classe `Behavior` située dans le namespace `ReForge.Engine.World`.

Créez un nouveau fichier `.cs` dans votre projet (par exemple dans un dossier `Behaviors`) :

```csharp
using ReForge.Engine.World;
using System.Numerics;

public class MonComportement : Behavior
{
    // Cette propriété sera automatiquement visible dans l'éditeur !
    public float Amplitude { get; set; } = 50f;
    
    public override void Update(float deltaTime)
    {
        // Logique de mise à jour (ex: rotation, mouvement, IA)
    }

    public override Behavior Clone()
    {
        return new MonComportement { Amplitude = this.Amplitude };
    }
}
```

## 2. Types supportés par l'Inspecteur

Pour que vos propriétés soient modifiables dans l'éditeur, elles doivent être **publiques** et utiliser l'un des types suivants :
- `float`, `int`, `bool`, `string`
- `Vector2` (System.Numerics)

## 3. Communication entre comportements

Vous pouvez envoyer des messages à d'autres comportements sur la même entité :

```csharp
// Envoyer un événement
Owner.BroadcastEvent("MonEvenement", donnée);

// Recevoir un événement (dans votre classe)
public override void OnReceivedEvent(string eventName, object? data)
{
    if (eventName == "MonEvenement") {
        // Faire quelque chose
    }
}
```

## 4. Utilisation dans l'Éditeur

1. Compilez votre projet.
2. Ouvrez l'Éditeur.
3. Sélectionnez une entité.
4. Dans l'Inspecteur, cliquez sur **Ajouter**.
5. Votre classe `MonComportement` apparaîtra automatiquement dans la liste !
