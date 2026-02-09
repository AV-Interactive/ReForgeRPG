# Reforge.Editor - Manuel d'utilisation

L'éditeur ReForge est un outil WYSIWYG (What You See Is What You Get) permettant de créer des niveaux de RPG.

## 1. Structure de l'Interface

L'interface est divisée en plusieurs panneaux dockables :

- **Hierarchy (Gauche)** : Liste toutes les entités de la scène. Permet la sélection (simple ou multiple), le renommage et la suppression.
- **Content Browser (Bas)** : Explorateur d'assets. Affiche les textures (.png) et les scènes (.json). Cliquez sur une texture pour l'utiliser avec le Pinceau.
- **Viewport (Centre)** : Vue interactive du jeu. Supporte le Gizmo de sélection et le rendu en temps réel.
- **Layer Control (Droite)** : Permet de choisir le calque de travail (Background, World, Foreground) et de gérer la visibilité.
- **Inspector (Droite)** : Modification des propriétés (Position, Nom, Tags) et configuration des `Behaviors`.

## 2. Outils et Navigation

### Barre de Menu
- **Fichier > Sauvegarder la Scène** : Enregistre l'état actuel au format JSON.
- **Bouton Play/Stop** : Bascule entre le mode édition (Editing) et le mode simulation (Playing).
- **Bouton Pinceau (B) / Selection (S)** :
    - **Pinceau (Drawing)** : Raccourci `B`. Cliquez pour placer l'asset sélectionné.
    - **Selection** : Raccourci `S`. Cliquez sur un objet pour le manipuler via le Gizmo.

### Le Map Painter (Pinceau)
1. Sélectionnez une texture dans le **Content Browser**.
2. Assurez-vous d'être en mode **Pinceau** (raccourci `B`).
3. Dans la barre de menu, choisissez le mode : **Pinceau** (un par un) ou **Rectangle** (remplissage).
4. Choisissez une couche dans le **Layer Control**.
5. Cliquez dans le **Viewport** pour placer l'objet. Le placement est automatiquement aligné sur une grille de 32x32 pixels.

### Sélection et Manipulation
1. Passez en mode **Selection** (raccourci `S`).
2. Cliquez sur un objet pour le sélectionner. Utilisez `Ctrl + Clic` pour la **sélection multiple**.
3. Déplacez les objets directement via le Gizmo ou modifiez les valeurs dans l'Inspecteur.

## 3. L'Inspecteur de Comportements

L'inspecteur est l'outil le plus puissant de l'éditeur :
- **Position** : Ajustez précisément les coordonnées de l'entité.
- **Tags** : Ajoutez des tags pour identifier vos objets (ex: "Player", "Solid").
- **Comportements** :
    - Vous pouvez ajouter de nouveaux composants via le bouton **Ajouter**.
    - Chaque paramètre public de votre script (float, int, bool, Vector2) est automatiquement affiché et modifiable en temps réel.
    - **ActionTrigger** : Permet de configurer des actions (Téléportation, Destruction) sans coder, en les liant à des événements de collision.

## 4. Architecture Interne

L'éditeur repose sur un **EditorContext** centralisé qui assure la cohérence entre les panneaux :
- **Sélection** : Gère la liste des entités sélectionnées et le Gizmo.
- **État du Moteur** : Synchronise le mode Play/Stop avec le cœur du moteur.
- **Gestion des Outils** : Alterne entre le `MapPainter` et l'outil de sélection.
