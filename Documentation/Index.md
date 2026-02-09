# Documentation ReForgeRPG

Bienvenue dans la documentation officielle de **ReForgeRPG**, un moteur de jeu 2D et son éditeur associés, conçus pour la création de RPG performants en C#.

## 📚 Sommaire

### 1. [Architecture du Moteur (Engine)](./Engine.md)
Découvrez le fonctionnement interne du moteur, du système d'entités à la gestion de la physique.
- Système d'Entités, Tilemaps et Comportements (Behaviors)
- Gestion de projet via le `ProjectManager`
- Moteur de Physique et Collisions (AABB et Triggers)
- Cycle de vie (Update/Draw)
- Sérialisation des Scènes et Polymorphisme

### 2. [Manuel de l'Éditeur (Editor)](./Editor.md)
Apprenez à utiliser l'interface de l'éditeur pour concevoir vos niveaux sans coder.
- Présentation de l'interface (MenuBar, Panneaux)
- Outils de dessin (Map Painter) : Pinceau et Rectangle
- Inspecteur, Hiérarchie et Sélection multiple
- Explorateur d'assets et types de ressources
- Gestion des calques (Layers)
- Mode Play/Stop et synchronisation d'état

### 3. [Tutoriels](./Tutorials/Index.md)
Des guides étape par étape pour prendre en main l'outil.
- [Créer sa première scène](./Tutorials/FirstScene.md)
- [Ajouter des comportements personnalisés](./Tutorials/CustomBehaviors.md)
- [Gérer les collisions et triggers](./Tutorials/Collisions.md)

---

## 🛠 Installation rapide

1. **Prérequis** : .NET 10 SDK, un IDE (Rider recommandé).
2. **Ouverture** : Ouvrez `ReForgeRPG.sln`.
3. **Lancement** : Exécutez le projet `Reforge.Editor` pour commencer la création.
