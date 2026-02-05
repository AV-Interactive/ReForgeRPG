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
Entity enemy = new Entity(new Vector2(250, 300), enemyTexture, "Enemy");
```
Deux entités sont créées :
- `playerEntity` : Placée au centre de l'écran (400, 300).
- `enemy` : Placée à la position (250, 300).

### 4. Ajout de comportements (Behaviors)
Des comportements sont attachés aux entités pour définir leur logique :

*   **Mouvement du Joueur** : On lui ajoute `InputMovable` avec une vitesse de 250, ce qui permet de le contrôler via les entrées clavier.
    ```csharp
    playerEntity.AddBehavior(new InputMovable { Speed = 250});
    ```
*   **Mouvement de l'Ennemi** : On lui ajoute `Oscillator` pour qu'il effectue un mouvement de va-et-vient.
    ```csharp
    enemy.AddBehavior(new Oscillator
    {
        Direction = new Vector2(1, 0),
        Distance = 150f,
        Speed = 150f
    });
    ```

### 5. Gestion des collisions
On ajoute des hitboxes (collisionneurs) aux entités pour détecter les interactions :

```csharp
var playerCollider = new BoxCollider();
playerCollider.OnCollisionEnter = (other) =>
{
    Console.WriteLine($"Toine, tu viens de toucher {other.Name} !");
};
playerEntity.AddBehavior(playerCollider);
enemy.AddBehavior(new BoxCollider());
```
- Le joueur possède un `BoxCollider` avec un événement `OnCollisionEnter` qui affiche un message dans la console lorsqu'il touche une autre entité.
- L'ennemi possède également un `BoxCollider` pour pouvoir être détecté lors d'une collision.

### 6. Enregistrement et lancement
```csharp
engine.AddEntity(playerEntity);
engine.AddEntity(enemy);

engine.Run();
```
Les entités sont ajoutées au moteur de jeu via `AddEntity`. Enfin, `engine.Run()` démarre la boucle principale du jeu.
