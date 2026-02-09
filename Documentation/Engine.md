# ReForge.Engine - Documentation Technique

Le cœur du moteur ReForgeRPG est conçu pour être simple, modulaire et extensible. Il repose sur une architecture proche de l'ECS (Entity Component System), mais orientée objet via un système de "Behaviors".

## 1. Le Système d'Entités (`Entity`)

L'entité est l'objet de base dans le monde. Elle possède :
- **Transform** : Position (`Vector2`) et profondeur (`ZIndex`).
- **Texture** : Une référence à une `Texture2D` gérée par l'AssetManager.
- **Tags** : Une liste de chaînes de caractères pour identifier ou grouper des entités.
- **Behaviors** : Une liste de composants logiques qui définissent le comportement de l'entité.

## 2. Comportements (`Behavior`)

Un `Behavior` est une classe abstraite qui permet d'ajouter de la logique à une entité.
- `Initialize()` : Appelé lors de l'ajout du comportement à une entité.
- `Update(float deltaTime)` : Appelé à chaque frame.
- `OnReceivedEvent(string eventName, object? data)` : Système de communication entre comportements.

### Comportements intégrés :
- `BoxCollider` : Gère les collisions AABB et les triggers.
- `InputMovable` : Permet de déplacer l'entité avec les touches directionnelles.
- `Oscillator` : Fait osciller l'entité de manière sinusoïdale.
- `Velocity` : Ajoute une gestion de vitesse vectorielle.
- `Follow` : L'entité suit une cible identifiée par un Tag spécifique.
- `ActionTrigger` : Exécute des commandes (`ActionCommand`) lors d'événements de collision.

## 3. Tilemaps et Couches (`Tilemap`)

Le moteur supporte un système de `Tilemap` pour les décors :
- **TileLayer** : Chaque couche de tuiles contient une matrice d'IDs.
- **Gestion des couches** : Une Tilemap peut contenir plusieurs couches (Sol, Détails, Obstacles).
- **Rendu optimisé** : Seules les tuiles visibles et nécessaires sont dessinées, avec support des collisions par couche.

## 4. Physique et Collisions (`CollisionSystem`)

Le système de physique est géré de manière globale par le `CollisionSystem` :
1. **Détection** : Utilise Raylib pour vérifier les chevauchements de rectangles (`Rectangle`) via l'algorithme AABB.
2. **Résolution** : Si deux entités ne sont pas des "Triggers", le moteur calcule le vecteur de pénétration minimal et repousse les entités pour résoudre la collision physiquement.
3. **Événements** : Déclenche les méthodes `OnCollisionEnter`, `OnCollisionStay`, et `OnCollisionExit` sur les `Behavior` concernés.
4. **ActionTrigger** : Un composant spécial qui s'ajoute automatiquement lors de l'ajout d'un `BoxCollider`. Il permet de lier des actions (téléportation, destruction, sons) aux événements de collision sans écrire de code.

## 5. Gestion des Projets et Sérialisation

Le `ProjectManager` centralise la gestion du projet :
- **Fichiers `.reforge`** : Contiennent les paramètres du projet (chemins, scène de démarrage).
- **Gestion des Assets** : Organise les dossiers par types (Actors, Scenes, VFX, etc.).
- **Persistance** : Sauvegarde automatique du dernier projet ouvert.

Le `SceneSerializer` utilise `System.Text.Json` (introduit dans .NET 10) pour sauvegarder l'état complet d'un niveau.
- **Support Polymorphique** : Tous les types de `Behavior` sont correctement sérialisés et désérialisés.
- **Récupération des Assets** : Les textures sont rechargées via l'`AssetManager` en utilisant le chemin stocké (`TexturePath`).
- **Persistance** : Permet de sauvegarder l'état exact (positions, variables des comportements) pour une reprise immédiate.

## 6. Cycle de Vie du Moteur

Le `Engine` centralise la boucle de jeu :
1. `Update` :
    - Mise à jour de la scène actuelle.
    - Mise à jour de chaque entité et de ses comportements.
    - Calcul et résolution des collisions par le `CollisionSystem`.
    - Support du redimensionnement dynamique de la fenêtre.
2. `Render` :
    - Nettoyage de l'écran.
    - Tri des entités par `ZIndex`.
    - Dessin des entités à l'écran.
