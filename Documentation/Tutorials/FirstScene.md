# Tutoriel : Créer sa première scène

Ce tutoriel vous guide dans la création d'un niveau simple avec un décor et un personnage contrôlable.

## Étapes

### 1. Préparation
- Lancez `Reforge.Editor`.
- Dans le **Content Browser**, repérez vos images (.png). Si vous n'en avez pas, placez-les dans le dossier `GameExample/Assets/`.

### 2. Dessiner le sol
- Sélectionnez l'outil **Pinceau** dans la barre de menu.
- Dans le **Layer Control**, sélectionnez **Background**.
- Choisissez une texture de sol dans le **Content Browser**.
- Cliquez et glissez dans le **Viewport** pour peindre votre sol.

### 3. Ajouter le joueur
- Dans le **Layer Control**, passez sur la couche **World**.
- Sélectionnez la texture de votre personnage.
- Cliquez une seule fois dans le monde pour le placer.

### 4. Rendre le joueur mobile
- Basculez sur l'outil **Selection**.
- Cliquez sur votre personnage dans le Viewport (ou dans la Hierarchy).
- Dans l'**Inspecteur** (à droite), cliquez sur le bouton **Ajouter** en bas.
- Choisissez `InputMovable`.
- Vous verrez maintenant une propriété `Speed` apparaître. Vous pouvez modifier sa valeur.

### 5. Tester
- Cliquez sur le bouton **Play** dans la barre de menu.
- Utilisez les flèches directionnelles de votre clavier : votre personnage se déplace !
- Cliquez sur **Stop** pour revenir au mode édition.

### 6. Sauvegarder
- Allez dans **Fichier > Sauvegarder la Scène**. 
- Votre scène est enregistrée dans `Assets/Scenes/01.json`.
