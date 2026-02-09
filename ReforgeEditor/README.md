# ReForge Editor - Manuel d'utilisation

Bienvenue dans l'éditeur ReForge, l'outil de création de scènes pour le moteur ReForgeRPG. Ce document sert de manuel d'utilisation pour vous aider à prendre en main les fonctionnalités de l'éditeur.

## 🖥️ Interface de l'Éditeur

L'interface est divisée en plusieurs panneaux dockables :

1.  **Game View (Centre)** : Fenêtre principale où vous visualisez et éditez votre scène.
2.  **Content Browser (Bas)** : Explorateur de fichiers pour vos assets (textures, etc.). Cliquez sur une texture pour la sélectionner.
3.  **Hierarchy (Gauche)** : Liste toutes les entités présentes dans la scène actuelle. Permet la sélection et la gestion de la visibilité.
4.  **Inspector (Droite)** : Affiche et permet de modifier les propriétés de l'entité sélectionnée (Transform, Nom, Tags, Behaviors).
5.  **Layer Control** : Panneau pour choisir la couche de travail active (Background, World, Foreground).
6.  **MenuBar (Haut)** : Accès rapide aux fichiers, outils et contrôle de l'état du moteur.

---

## 🛠️ Outils et Contrôles

### Barre de Menu
- **Fichier > Sauvegarder la Scène** : Enregistre l'état actuel de la scène dans un fichier JSON.
- **Play / Stop** : Bascule entre le mode édition (Editing) et le mode test (Playing). Les modifications physiques et les comportements ne sont actifs qu'en mode Play.
- **Pinceau (B) / Selection (S)** : Alterne entre l'outil de dessin et l'outil de sélection d'entités.
- **Modes de peinture** : Dans le menu, vous pouvez alterner entre le mode **Pinceau** (un par un) et le mode **Rectangle** (remplissage de zone).
- **Toggle State** : Synchronisation de l'état Playing/Editing entre l'interface et le moteur.

### Utilisation du Pinceau (Map Painter)
1. Sélectionnez l'outil **Pinceau** (raccourci `B`).
2. Dans le **Content Browser**, cliquez sur une texture pour la sélectionner.
3. Choisissez le calque cible dans **Layer Control**.
4. **Mode Pinceau** : Cliquez (ou maintenez le clic) dans la **Game View** pour placer l'asset sur la grille (calé sur 32x32 par défaut).
5. **Mode Rectangle** : Cliquez pour définir le point de départ, maintenez et relâchez pour remplir la zone rectangulaire avec l'asset sélectionné.

### Sélection et Inspection
1. Sélectionnez l'outil **Selection** (raccourci `S`).
2. Cliquez sur une entité dans la **Game View** ou dans la **Hierarchy**.
3. L'entité sélectionnée est mise en surbrillance par un **Gizmo** rectangulaire.
4. Support de la sélection multiple (via la Hierarchy).
5. Modifiez ses propriétés (Nom, Position, ZIndex, etc.) directement dans l'**Inspector**.

---

## 🏗️ Architecture Interne

L'éditeur utilise un **EditorContext** centralisé qui partage l'état entre les différents panneaux :
- État de sélection (`SelectedEntities`).
- État de l'application (Édition vs Jeu).
- Paramètres de vue (Largeurs des panneaux, hauteur du menu).
- Instance du `MapPainter` et du `Gizmo`.

---

## 🧊 Gestion des Couches (Layers)

L'éditeur gère trois couches de rendu pour organiser la profondeur de votre scène :

- **Background (0)** : Pour les décors de fond, sols, etc.
- **World (1)** : La couche principale où se trouvent les obstacles, le joueur et les interactions.
- **Foreground (2)** : Pour les éléments qui doivent passer devant tout le reste (toits, feuillage, UI fixe).

*Note : Vous ne pouvez interagir (sélectionner ou peindre) qu'avec la couche actuellement sélectionnée dans le panneau Layer Control.*

---

## 💾 Sauvegarde et Chargement

- Les scènes sont sauvegardées dans le dossier `Assets/Scenes`.
- Le format de fichier utilisé est le JSON, ce qui permet une édition manuelle si nécessaire.
- Lors du lancement, l'éditeur charge automatiquement la scène par défaut (généralement `01.json`).

---

## ⌨️ Raccourcis Utiles (À venir)
*Les raccourcis clavier sont en cours de développement.*
