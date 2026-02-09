# Tutoriel : Gérer les collisions et triggers

Ce guide explique comment empêcher le joueur de traverser les murs et comment créer des zones d'interaction (triggers).

## 1. Créer un mur (Collision solide)

Pour qu'un objet soit solide :
1. Sélectionnez l'objet dans l'éditeur.
2. Ajoutez-lui le comportement `BoxCollider`.
3. Dans l'inspecteur, réglez la `Width` et la `Height` (si vous laissez à 0, la taille de la texture sera utilisée automatiquement).
4. Assurez-vous que la case `IsTrigger` est **décochée**.
5. Répétez l'opération pour le joueur.
6. En mode **Play**, le joueur sera maintenant bloqué par le mur.

## 2. Créer une zone de téléportation (Trigger)

Un trigger est une zone qui détecte une entrée mais ne bloque pas le mouvement.

1. Créez une entité (ex: un escalier ou une dalle).
2. Ajoutez-lui un `BoxCollider`.
3. Cochez la case **IsTrigger**.
4. L'éditeur va automatiquement ajouter un composant `ActionTrigger` caché (mais visible dans l'inspecteur).
5. Dans le composant `ActionTrigger`, ouvrez la section `OnEnterActions`.
6. Cliquez sur **+ Ajouter**.
7. Réglez l'action :
    - **Verbe** : `Teleport`
    - **Cibler soi-même** : Décoché (pour cibler l'entité qui entre dans la zone).
    - **Destination** : Réglez les coordonnées X et Y de destination.
8. Testez en mode **Play** : dès que le joueur touche la zone, il est téléporté !

## 3. Détruire un objet

Vous pouvez utiliser le même système pour créer des objets ramassables (pièces, clés) :
1. Créez l'objet avec un `BoxCollider` en mode `IsTrigger`.
2. Dans `OnEnterActions`, ajoutez une commande avec le verbe `Destroy`.
3. Cochez **Cibler soi-même**.
4. En mode **Play**, l'objet disparaîtra dès que le joueur le touchera.
