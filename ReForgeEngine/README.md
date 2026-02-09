# ReForgeEngine

Le cœur du moteur de jeu ReForgeRPG. Ce module contient toute la logique fondamentale nécessaire pour faire tourner un jeu 2D.

## 🏗️ Architecture

Le moteur suit une approche orientée objets avec un système de composants simplifiés appelés **Behaviors**.

### Core
- **Engine** : Classe principale qui orchestre l'initialisation, la boucle de jeu (`Update`/`Draw`) et le nettoyage.
- **ProjectManager** : Gère la structure des projets, les chemins d'assets et la persistance globale.
- **AssetManager** : Gère le chargement et la mise en cache des textures pour éviter les fuites de mémoire et optimiser les performances.
- **SceneSerializer** : Système de sauvegarde et chargement de scènes au format JSON avec support du polymorphisme pour les Behaviors.

### World (Monde)
- **Scene** : Contient et gère le cycle de vie des entités.
- **Entity** : L'objet de base du jeu. Chaque entité possède :
    - Un **TransformComponent** (Position automatique).
    - Une texture et un ZIndex (profondeur).
    - Des **Tags** pour l'identification.
    - Une liste de **Behaviors**.
- **Tilemap** : Système de rendu par tuiles optimisé supportant plusieurs couches (`TileLayer`).
- **Behavior** : Classe de base pour ajouter de la logique aux entités.
    - `Update(float deltaTime)` : Appelée à chaque frame.
    - `OnCollisionEnter/Stay/Exit` : Événements de physique.
    - `OnReceivedEvent` : Communication inter-comportements.
- **Comportements intégrés** :
    - `BoxCollider` : Physique et triggers.
    - `InputMovable` : Déplacement clavier.
    - `Oscillator` : Mouvement sinusoïdal.
    - `Velocity` : Gestion vectorielle de la vitesse.
    - `Follow` : Suivi d'une cible (Tag).
- **TransformComponent** : Gère la position de l'entité. Ajouté automatiquement à chaque entité.
- **ActionTrigger** : Permet de déclencher des événements spécifiques lors d'interactions ou d'états de jeu.

### Physics (Physique)
- **CollisionSystem** : Gère la détection globale et la résolution des collisions (AABB).
- **BoxCollider** : Un Behavior spécial qui donne une boîte de collision à une entité. Gère les événements `OnCollisionEnter`, `OnCollisionStay`, et `OnCollisionExit`.
- **Triggers** : Support des collisions sans résolution physique (`IsTrigger = true`) pour déclencher des événements.

## 🚀 Utilisation rapide

```csharp
// 1. Initialisation
var engine = new Engine(1280, 720, "Mon Jeu");
engine.Initialize();

// 2. Création d'une entité
var player = new Entity(new Vector2(100, 100), engine.AssetManager.GetTexture("assets/player.png"), "Joueur", "assets/player.png");
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
