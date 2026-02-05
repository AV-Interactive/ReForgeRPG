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
var player = engine.LoadTexture("Assets/Sprites/player.png");
var enemyTexture = engine.LoadTexture("Assets/Sprites/enemy.png");
```
Les textures pour le joueur et l'ennemi sont chargées à partir des fichiers image situés dans le dossier `Assets/Sprites/`.

### 3. Création des entités
```csharp
Entity playerEntity = new Entity(new Vector2(400, 300), player, "Player");
Entity enemy = new Entity(new Vector2(100, 300), enemyTexture, "Enemy");
```
Deux entités sont créées :
- `playerEntity` : Placée au centre de l'écran (400, 300).
- `enemy` : Placée à la position (100, 300).

### 4. Ajout de comportements (Behaviors)
Des comportements sont attachés aux entités pour définir leur logique :

*   **Joueur** : On lui ajoute `InputMovable` avec une vitesse de 250, ce qui permet de le contrôler via les entrées clavier.
    ```csharp
    playerEntity.AddBehavior(new InputMovable { Speed = 250});
    ```
*   **Ennemi** : On lui ajoute `Oscillator` pour qu'il effectue un mouvement de va-et-vient.
    ```csharp
    enemy.AddBehavior(new Oscillator
    {
        Direction = new Vector2(1, 0),
        Distance = 100f,
        Speed = 150f
    });
    ```

### 5. Enregistrement et lancement
```csharp
engine.AddEntity(playerEntity);
engine.AddEntity(enemy);

engine.Run();
```
Les entités sont ajoutées au moteur de jeu via `AddEntity`. Enfin, `engine.Run()` démarre la boucle principale du jeu.
