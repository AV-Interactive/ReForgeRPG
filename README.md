# ReForgeRPG

ReForgeRPG est un moteur de jeu 2D accompagné de son éditeur, conçu pour faciliter la création de RPG. Le projet est développé en C# avec .NET 10, en utilisant Raylib pour le rendu et ImGui pour l'interface de l'éditeur.

## 📚 Documentation

Une documentation complète est disponible dans le dossier [`/Documentation`](./Documentation/Index.md) :
- [**Documentation Technique du Moteur**](./Documentation/Engine.md)
- [**Manuel de l'Utilisateur de l'Éditeur**](./Documentation/Editor.md)
- [**Tutoriels pas à pas**](./Documentation/Tutorials/Index.md)

## 🚀 Composants du projet

Le projet est divisé en trois parties principales :

- **[ReForgeEngine](./ReForgeEngine)** : Le cœur du moteur de jeu (Entités, Physiques, Scènes). (Consultez la [Documentation Technique](./ReForgeEngine/README.md))
- **[ReforgeEditor](./ReforgeEditor)** : L'outil de création de scènes et de gestion d'assets. (Consultez le [Manuel d'utilisation](./ReforgeEditor/README.md))
- **[GameExample](./GameExample)** : Un projet de démonstration utilisant le moteur.

## 🛠️ Fonctionnalités

### ReForgeEngine (Moteur)
- **Système d'Entités** : Gestion d'entités avec position, textures, tags et comportements (Behaviors).
- **Gestion de Scènes** : Système de scènes permettant d'organiser et de mettre à jour les entités.
- **Asset Manager** : Chargement et mise en cache centralisée des textures.
- **Système de Collision** : Détection et résolution des collisions (AABB) avec support des Triggers et événements (Enter, Stay, Exit), ainsi qu'un système d'**ActionTrigger** pour les interactions.
- **Rendu Performant** : Basé sur Raylib, avec gestion du ZIndex pour la profondeur et support du redimensionnement de fenêtre.
- **Sérialisation** : Sauvegarde et chargement des scènes au format JSON (via `System.Text.Json`).

### ReforgeEditor (Éditeur)
- **Map Painter** : Outil de peinture sur grille (Tile-based) pour créer des environnements.
- **Système de Couches** : Gestion de la profondeur avec trois couches principales (Background, World, Foreground).
- **Content Browser** : Explorateur d'assets pour importer et sélectionner des ressources.
- **Hierarchy Panel** : Visualisation en temps réel et sélection des entités de la scène.
- **Inspector** : Modification dynamique des propriétés (Position, Nom, Tags, etc.) et visualisation des comportements.
- **Game View** : Aperçu interactif utilisant des textures de rendu (RenderTexture).
- **Mode Play/Stop** : Basculement instantané entre l'édition et la simulation physique.

## 💻 Technologies utilisées

- **Langage** : C# (14.0)
- **Framework** : .NET 10
- **Rendu** : [Raylib-cs](https://github.com/ChrisDill/Raylib-cs)
- **Interface Éditeur** : [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) & [rlImGui-cs](https://github.com/raylib-extras/rlImGui-cs)

## 📁 Structure du projet

- `ReForgeEngine/` : Source du moteur de jeu.
- `ReforgeEditor/` : Source de l'éditeur de scènes.
- `GameExample/` : Projet exemple démontrant l'utilisation du moteur.
- `ReForgeRPG.sln` : Solution globale pour le développement.

## ⚙️ Installation & Utilisation

1. Clonez le dépôt.
2. Ouvrez `ReForgeRPG.sln` avec Rider ou Visual Studio.
3. Compilez la solution.
4. Lancez `ReforgeEditor` pour commencer à créer vos scènes ou `GameExample` pour voir le moteur en action.
