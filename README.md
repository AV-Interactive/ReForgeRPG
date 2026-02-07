# ReForgeRPG

ReForgeRPG est un moteur de jeu 2D accompagné de son éditeur, conçu pour faciliter la création de RPG. Le projet est développé en C# avec .NET 10, en utilisant Raylib pour le rendu et ImGui pour l'interface de l'éditeur.

## 🚀 Composants du projet

Le projet est divisé en trois parties principales :

- **[ReForge.Engine](./ReForge.Engine)** : Le cœur du moteur de jeu (Entités, Physiques, Scènes).
- **[Reforge.Editor](./Reforge.Editor)** : L'outil de création de scènes et de gestion d'assets. (Consultez le [Manuel d'utilisation](./Reforge.Editor/README.md))
- **[GameExample](./GameExample)** : Un projet de démonstration utilisant le moteur.

## 🛠️ Fonctionnalités

### ReForge.Engine (Moteur)
- **Système d'Entités** : Gestion d'entités avec position, textures et profondeur (ZIndex).
- **Gestion de Scènes** : Système de scènes permettant d'organiser les objets de jeu.
- **Asset Manager** : Chargement et gestion centralisée des textures.
- **Système de Collision** : Prise en charge des collisions de base (AABB).
- **Rendu Performant** : Basé sur Raylib pour un rendu 2D efficace.

### Reforge.Editor (Éditeur)
- **Map Painter** : Outil de peinture sur grille (Tile-based) pour créer des environnements.
- **Système de Couches** : Gestion de la profondeur avec trois couches principales (Background, World, Foreground).
- **Content Browser** : Explorateur d'assets pour importer et sélectionner des ressources.
- **Hierarchy Panel** : Visualisation en temps réel des entités présentes dans la scène.
- **Inspector** : Modification des propriétés des entités sélectionnées.
- **Game View** : Aperçu interactif du jeu directement dans l'éditeur.
- **Mode Play/Stop** : Basculement entre l'édition et l'exécution du jeu.

## 💻 Technologies utilisées

- **Langage** : C# (14.0)
- **Framework** : .NET 10
- **Rendu** : [Raylib-cs](https://github.com/ChrisDill/Raylib-cs)
- **Interface Éditeur** : [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) & [rlImGui-cs](https://github.com/raylib-extras/rlImGui-cs)

## 📁 Structure du projet

- `ReForge.Engine/` : Source du moteur de jeu.
- `Reforge.Editor/` : Source de l'éditeur de scènes.
- `GameExample/` : Projet exemple démontrant l'utilisation du moteur.
- `ReForgeRPG.sln` : Solution globale pour le développement.

## ⚙️ Installation & Utilisation

1. Clonez le dépôt.
2. Ouvrez `ReForgeRPG.sln` avec Rider ou Visual Studio.
3. Compilez la solution.
4. Lancez `Reforge.Editor` pour commencer à créer vos scènes ou `GameExample` pour voir le moteur en action.
