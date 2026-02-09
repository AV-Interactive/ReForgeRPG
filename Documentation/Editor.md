# Reforge.Editor - Manuel d'utilisation

L'éditeur ReForge est un outil WYSIWYG (What You See Is What You Get) permettant de créer des niveaux de RPG.

## 1. Structure de l'Interface

L'interface est divisée en plusieurs panneaux dockables :

- **Hierarchy (Haut-Gauche)** : Liste toutes les entités présentes dans la scène actuelle. Cliquez sur une entité pour la sélectionner.
- **Content Browser (Bas-Gauche)** : Votre explorateur d'assets. Affiche les textures (.png) et les scènes (.json).
- **Viewport (Centre)** : La vue de votre jeu. C'est ici que vous dessinez et manipulez les objets.
- **Layer Control (Bas-Gauche)** : Permet de choisir sur quelle couche vous travaillez (Background, World, Foreground).
- **Inspector (Droite)** : Affiche et permet de modifier les propriétés de l'entité sélectionnée (Position, Tags, Comportements).

## 2. Outils et Navigation

### Barre de Menu
- **Fichier > Sauvegarder la Scène** : Enregistre votre travail dans `Assets/Scenes/01.json`.
- **Bouton Play/Stop** : Bascule entre le mode édition et le mode simulation. En mode Play, la physique et les scripts s'activent.
- **Bouton Pinceau/Selection** :
    - **Pinceau (Drawing)** : Cliquez dans le Viewport pour placer l'asset sélectionné dans le Content Browser.
    - **Selection** : Cliquez sur un objet dans le Viewport pour le sélectionner et voir ses propriétés dans l'Inspecteur.

### Le Map Painter (Pinceau)
1. Sélectionnez une texture dans le **Content Browser**.
2. Assurez-vous d'être en mode **Pinceau**.
3. Choisissez une couche dans le **Layer Control**.
4. Cliquez dans le **Viewport** pour placer l'objet. Le placement est automatiquement aligné sur une grille de 32x32 pixels.

## 3. L'Inspecteur de Comportements

L'inspecteur est l'outil le plus puissant de l'éditeur :
- **Position** : Ajustez précisément les coordonnées de l'entité.
- **Tags** : Ajoutez des tags pour identifier vos objets (ex: "Player", "Solid").
- **Comportements** :
    - Vous pouvez ajouter de nouveaux composants via le bouton **Ajouter**.
    - Chaque paramètre public de votre script (float, int, bool, Vector2) est automatiquement affiché et modifiable en temps réel.
    - **ActionTrigger** : Permet de configurer des actions (Téléportation, Destruction) sans coder, en les liant à des événements de collision.
