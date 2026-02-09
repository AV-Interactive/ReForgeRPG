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
On crée une instance de la classe `Engine` avec une résolution de 800x600 pixels et un titre de fenêtre. La méthode `Initialize()` prépare le moteur de jeu (Fenêtre, OpenGL, etc.).

### 2. Chargement des ressources
Les textures sont gérées par l'**AssetManager** du moteur. Dans l'exemple, elles sont chargées via la factory :
```csharp
var tex = engine.AssetManager.GetTexture(texPath);
```
L'AssetManager assure que chaque texture n'est chargée qu'une seule fois en mémoire.

### 3. Création des entités via la Factory
```csharp
var player = EntityFactory.CreateActor(engine, new Vector2(400,300), "Assets/Sprites/player.png", "Toine", new List<string> {"Player"});
player.AddBehavior(new InputMovable());

var enemy = EntityFactory.CreateActor(engine, new Vector2(100,100), "Assets/Sprites/enemy.png", "Slime", new List<string> {"Enemy"});
enemy.AddBehavior(new Oscillator());
```
On utilise `EntityFactory.CreateActor` pour créer nos entités. On ajoute ensuite les comportements spécifiques :
- `player` : Reçoit `InputMovable` pour le contrôle au clavier (ZQSD / Flèches).
- `enemy` : Reçoit `Oscillator` pour un mouvement de va-et-vient automatique.

### 4. Configuration dans EntityFactory
La méthode `EntityFactory.CreateActor` centralise la configuration :
- Récupération de la texture via l'`AssetManager`.
- Initialisation de l'entité (Position, Texture, Nom, Chemin).
- Ajout automatique d'un **BoxCollider** (et par extension d'un **ActionTrigger** par le moteur).
- Ajout des tags d'identification.

### 5. Gestion des collisions
La détection et la résolution des collisions (AABB) sont gérées automatiquement par le moteur. Le `player` et l'`enemy` ayant tous deux un `BoxCollider`, ils se bloqueront mutuellement physiquement.

### 6. Enregistrement et lancement
```csharp
engine.CurrentScene.AddEntity(player);
engine.CurrentScene.AddEntity(enemy);

engine.Run();
```
Les entités sont ajoutées à la scène. `engine.Run()` démarre la boucle de jeu principale.
