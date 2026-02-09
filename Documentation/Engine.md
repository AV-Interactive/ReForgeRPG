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
- `ActionTrigger` : Exécute des commandes (`ActionCommand`) lors d'événements de collision.

## 3. Physique et Collisions (`CollisionSystem`)

Le système de physique est géré de manière globale :
1. **Détection** : Utilise Raylib pour vérifier les chevauchements de rectangles (`Rectangle`).
2. **Résolution** : Si deux entités ne sont pas des "Triggers", le moteur calcule le vecteur de pénétration minimal et repousse l'entité A pour sortir de l'entité B.
3. **Événements** : Déclenche `OnCollisionEnter`, `OnCollisionStay`, et `OnCollisionExit`.

## 4. Gestion des Scènes et Sérialisation

Une `Scene` contient une liste d'entités.
Le `SceneSerializer` utilise `System.Text.Json` avec un support pour le polymorphisme des comportements. Cela permet de sauvegarder l'état complet d'un niveau dans un fichier `.json` et de le recharger exactement dans le même état (y compris les propriétés modifiées dans l'inspecteur).

## 5. Cycle de Vie du Moteur

Le `Engine` centralise la boucle de jeu :
1. `Update` :
    - Mise à jour de la scène actuelle.
    - Mise à jour de chaque entité.
    - Exécution de la logique des `Behaviors`.
    - Calcul des collisions.
2. `Render` :
    - Nettoyage de l'écran.
    - Tri des entités par `ZIndex`.
    - Dessin des entités à l'écran.
