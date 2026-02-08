# ReForge.Engine

Le cœur du moteur de jeu ReForgeRPG. Ce module contient toute la logique fondamentale nécessaire pour faire tourner un jeu 2D.

## 🏗️ Architecture

Le moteur suit une approche orientée objets avec un système de composants simplifiés appelés **Behaviors**.

### Core
- **Engine** : Classe principale qui orchestre l'initialisation, la boucle de jeu (`Update`/`Draw`) et le nettoyage.
- **AssetManager** : Gère le chargement et la mise en cache des textures pour éviter les fuites de mémoire et optimiser les performances.
- **SceneSerializer** : Système de sauvegarde et chargement de scènes au format JSON avec support du polymorphisme pour les Behaviors.

### World (Monde)
- **Scene** : Contient et gère le cycle de vie des entités.
- **Entity** : L'objet de base du jeu. Chaque entité possède :
    - Une position, une texture et un ZIndex (profondeur).
    - Des **Tags** pour l'identification.
    - Une liste de **Behaviors**.
- **Behavior** : Classe de base pour ajouter de la logique aux entités.
    - `Update(float deltaTime)` : Appelée à chaque frame.
    - `OnCollisionEnter/Stay/Exit` : Événements de physique.

### Physics (Physique)
- **CollisionSystem** : Gère la détection globale des collisions.
- **BoxCollider** : Un Behavior spécial qui donne une boîte de collision à une entité.
- **Triggers** : Support des collisions sans résolution physique pour déclencher des événements.

## 🚀 Utilisation rapide

```csharp
// 1. Initialisation
var engine = new Engine(1280, 720, "Mon Jeu");
engine.Initialize();

// 2. Création d'une entité
var player = new Entity(new Vector2(100, 100), "assets/player.png", "Joueur");
player.AddBehavior(new BoxCollider());
player.AddBehavior(new InputMovable());

// 3. Ajout à la scène
engine.CurrentScene.AddEntity(player);

// 4. Lancement
engine.Run();
```

## 🛠️ Dépendances
- [Raylib-cs](https://github.com/ChrisDill/Raylib-cs) : Rendu et entrées.
- `System.Text.Json` : Sérialisation des scènes.
