# ReForge Editor - Manuel d'utilisation

Bienvenue dans l'éditeur ReForge, l'outil de création de scènes pour le moteur ReForgeRPG. Ce document sert de manuel d'utilisation pour vous aider à prendre en main les fonctionnalités de l'éditeur.

## 🖥️ Interface de l'Éditeur

L'interface est divisée en plusieurs panneaux dockables :

1.  **Game View (Centre)** : La fenêtre principale où vous visualisez et éditez votre scène.
2.  **Content Browser (Bas)** : Explorateur de fichiers pour vos assets (textures, etc.).
3.  **Hierarchy (Gauche)** : Liste toutes les entités présentes dans la scène actuelle.
4.  **Inspector (Droite)** : Affiche et permet de modifier les propriétés de l'entité sélectionnée.
5.  **Layer Control** : Permet de choisir sur quelle couche vous travaillez.

---

## 🛠️ Outils et Contrôles

### Barre de Menu
- **Fichier > Sauvegarder la Scène** : Enregistre l'état actuel de la scène dans un fichier JSON.
- **Play / Stop** : Bascule entre le mode édition et le mode test (exécution des comportements/physiques).
- **Pinceau / Selection** : Alterne entre l'outil de dessin et l'outil de sélection d'entités.

### Utilisation du Pinceau (Map Painter)
1. Sélectionnez l'outil **Pinceau** dans la barre de menu.
2. Dans le **Content Browser**, cliquez sur une texture pour la sélectionner.
3. Choisissez le calque cible dans **Layer Control**.
4. Cliquez (ou maintenez le clic) dans la **Game View** pour placer l'asset sur la grille.

### Sélection et Inspection
1. Sélectionnez l'outil **Selection** dans la barre de menu.
2. Cliquez sur une entité dans la **Game View** ou dans la **Hierarchy**.
3. L'entité sélectionnée est mise en surbrillance.
4. Modifiez ses propriétés (Position, ZIndex, etc.) directement dans l'**Inspector**.

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
