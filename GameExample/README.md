# GameExample Documentation

Ce projet est un exemple simple d'utilisation du moteur **ReForgeRPG**. Il démontre comment initialiser le moteur, charger des assets et créer des entités avec des comportements personnalisés.

Le point d'entrée se trouve dans `Program.cs`.

## Contenu de la méthode Main

La méthode `Main` orchestre l'initialisation et le lancement du jeu à travers les étapes suivantes :

### 1. Initialisation du moteur
```csharp
var engine = new Engine(800, 600, "Mon premier jeu avec ReForgeRPG");
engine.Initialize();
```
On crée une instance de la classe `Engine` avec une résolution de 800x600 pixels et un titre de fenêtre. La méthode `Initialize()` prépare le moteur de jeu.

### 2. Chargement des ressources
```csharp
// Les textures sont gérées par l'AssetManager du moteur
// Elles sont chargées automatiquement lors de la création d'acteurs via la factory
```
Les textures pour le joueur et l'ennemi sont chargées à partir des fichiers image situés dans le dossier `Assets/Sprites/`.

### 3. Création des entités via la Factory
```csharp
var player = EntityFactory.CreateActor(engine, new Vector2(400,300), "Assets/Sprites/player.png", "Toine", new List<string> {"Player"});
player.AddBehavior(new InputMovable());

var enemy = EntityFactory.CreateActor(engine, new Vector2(100,100), "Assets/Sprites/enemy.png", "Slime", new List<string> {"Enemy"});
enemy.AddBehavior(new Oscillator());
```
On utilise `EntityFactory.CreateActor` pour créer nos entités avec leurs propriétés de base et un `BoxCollider` par défaut. On ajoute ensuite les comportements spécifiques :
- `player` : Reçoit `InputMovable` pour le contrôle au clavier.
- `enemy` : Reçoit `Oscillator` pour un mouvement automatique.

### 4. Configuration dans EntityFactory
La méthode `EntityFactory.CreateActor` centralise la configuration initiale :
- Charge la texture via l' `AssetManager`.
- Initialise l'entité avec son nom et ses tags.
- Ajoute automatiquement un `BoxCollider` pour la gestion des collisions.

### 5. Gestion des collisions
Grâce au `BoxCollider` ajouté par la factory, les entités peuvent interagir. La détection et la résolution des collisions (AABB) sont gérées automatiquement par le `CollisionSystem` du moteur.

### 6. Enregistrement et lancement
```csharp
engine.CurrentScene.AddEntity(player);
engine.CurrentScene.AddEntity(enemy);

engine.Run();
```
Les entités sont ajoutées à la scène actuelle. Enfin, `engine.Run()` démarre la boucle principale.
