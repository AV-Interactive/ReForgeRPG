# GameExample Documentation

Ce projet est un exemple simple d'utilisation du moteur **ReForgeRPG**. Le point d'entrée se trouve dans `Program.cs`.

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
var playerTexture = engine.LoadTexture("Assets/Sprites/player.png");
var enemyTexture = engine.LoadTexture("Assets/Sprites/enemy.png");
```
Les textures pour le joueur et l'ennemi sont chargées à partir des fichiers image situés dans le dossier `Assets/Sprites/`.

### 3. Création des entités via la Factory
```csharp
var player = EntityFactory.CreatePlayer(engine, new Vector2(400, 300), playerTexture);
var e1 = EntityFactory.CreateEnemy(engine, new Vector2(130, 243), enemyTexture);
```
On utilise `EntityFactory` pour créer nos entités avec leurs comportements prédéfinis :
- `player` : Créé à (400, 300).
- `e1` : Un ennemi créé à (130, 243).

### 4. Configuration dans EntityFactory
La classe `EntityFactory` centralise la configuration des entités :

*   **Le Joueur** : Reçoit automatiquement le comportement `InputMovable` (vitesse 250) et un `BoxCollider`.
*   **L'Ennemi** : Reçoit un `Oscillator` (mouvement latéral de 150 unités) et un `BoxCollider`.

### 5. Gestion des collisions et destruction
Dans `EntityFactory.CreateEnemy`, une logique de collision est définie :
```csharp
collider.OnCollisionEnter = (other) =>
{
    if (other.Name == "Player")
    {
        engine.DestroyEntity(enemyEntity);
    }
};
```
Si l'ennemi entre en collision avec une entité nommée "Player", il est détruit du moteur de jeu.

### 6. Enregistrement et lancement
```csharp
engine.CurrentScene.AddEntity(player);
engine.CurrentScene.AddEntity(e1);

engine.Run();
```
Les entités sont ajoutées à la scène actuelle du moteur. Enfin, `engine.Run()` démarre la boucle principale.
