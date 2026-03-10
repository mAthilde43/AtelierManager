# 🔥 FIREBASE POUR TON JEU UNITY - Guide Ultra Simple

## C'est quoi Firebase ?

**Firebase = Une base de données gratuite sur internet** (par Google)

Au lieu de sauvegarder sur l'ordi du joueur (PlayerPrefs), tu sauvegardes sur internet !

---

## 📋 CE QUE TU DOIS FAIRE (5 minutes)

### ÉTAPE 1 : Créer un projet Firebase (2 min)

1. Va sur **https://console.firebase.google.com**
2. Connecte-toi avec ton compte Google
3. Clique **"Créer un projet"**
4. Nomme-le **"AtelierManager"** (ou ce que tu veux)
5. Désactive Google Analytics (pas besoin)
6. Clique **"Créer le projet"**

### ÉTAPE 2 : Activer la base de données (1 min)

1. Dans le menu à gauche, clique **"Build"** → **"Realtime Database"**
2. Clique **"Créer une base de données"**
3. Choisis **"Europe"** (ou le plus proche de toi)
4. **IMPORTANT** : Choisis **"Mode test"** (sinon ça bloque tout)
5. Clique **"Activer"**

### ÉTAPE 3 : Copier ton URL (30 sec)

1. Tu vois ton URL en haut de la page, ça ressemble à :
   ```
   https://ateliermanager-12345-default-rtdb.europe-west1.firebasedatabase.app
   ```
2. **Copie cette URL !**

### ÉTAPE 4 : Configurer Unity (1 min)

1. Ouvre Unity et va dans ta **scène principale** (celle où le jeu démarre)

2. **Créer un GameObject vide :**
   - Dans le menu en haut : **GameObject** → **Create Empty**
   - OU clic droit dans la **Hierarchy** (panneau à gauche) → **Create Empty**
   - OU raccourci : **Cmd + Shift + N** (Mac) / **Ctrl + Shift + N** (Windows)

3. **Renomme-le "FirebaseManager"** :
   - Clique sur le nouveau GameObject dans la Hierarchy
   - Appuie sur **Enter** (ou **F2**) pour renommer
   - Tape : `FirebaseManager`

4. **Ajoute le script :**
   - Sélectionne ton GameObject "FirebaseManager"
   - Dans l'**Inspector** (panneau à droite), clique sur **Add Component**
   - Tape `FirebaseManager` dans la recherche
   - Clique dessus pour l'ajouter

5. **Colle ton URL Firebase :**
   - Dans l'Inspector, tu vois maintenant le composant FirebaseManager
   - Dans le champ **"Firebase Database URL"**, colle ton URL Firebase

6. **Répète pour FirebaseSaveManager :**
   - **GameObject** → **Create Empty**
   - Renomme-le `FirebaseSaveManager`
   - **Add Component** → cherche `FirebaseSaveManager` → ajoute-le

### ÉTAPE 5 : C'est fini ! 🎉

---

## 🎮 COMMENT UTILISER DANS TON CODE

### Sauvegarder la partie :
```csharp
// Quelque part dans ton code (après une vente, fin de journée, etc.)
FirebaseSaveManager.Instance.SaveToFirebase();
```

### Charger la partie :
```csharp
// Au démarrage du jeu
FirebaseSaveManager.Instance.LoadFromFirebase();
```

### Sauvegarde automatique (locale + Firebase) :
```csharp
// Fait les deux d'un coup !
FirebaseSaveManager.Instance.AutoSave();
```

---

## 📊 VOIR TES DONNÉES

1. Va sur **https://console.firebase.google.com**
2. Ouvre ton projet
3. Va dans **"Realtime Database"**
4. Tu vois toutes les sauvegardes de tous les joueurs ! 🎉

Les données ressemblent à ça :
```
sauvegardes/
  joueur_a1b2c3d4/
    money: 5000
    level: 7
    experience: 450
    materialQuantities: [10, 5, 3, ...]
    ...
```

---

## ⚠️ IMPORTANT - Mode Test

Le **"mode test"** expire après 30 jours. Pour un vrai jeu, il faudra configurer des règles de sécurité. Mais pour ton projet d'école, le mode test suffit largement !

---

## 🆘 PROBLÈMES COURANTS

### "Firebase pas prêt" dans la console
→ Vérifie que ton URL est correcte dans FirebaseManager

### "Erreur de connexion"
→ Vérifie ta connexion internet
→ Vérifie que le mode test est activé

### Les données ne s'enregistrent pas
→ Attends quelques secondes, c'est pas instantané
→ Rafraîchis la page Firebase

---

## 📝 RÉSUMÉ TECHNIQUE

| Technologie | Utilisation |
|-------------|-------------|
| **Firebase Realtime Database** | Base de données NoSQL en temps réel |
| **API REST** | Communication HTTP (GET, PUT, DELETE) |
| **JSON** | Format des données |
| **UnityWebRequest** | Pour faire les requêtes HTTP depuis Unity |

C'est exactement ce qu'on te demande :
- ✅ API REST (les requêtes HTTP vers Firebase)
- ✅ Base de données (Firebase Realtime Database)
- ✅ JSON (format des données)

---

## 🎓 POUR TON RAPPORT/SOUTENANCE

Tu peux dire :
> "J'utilise Firebase Realtime Database comme backend. Les données sont stockées en JSON et accessibles via une API REST. J'utilise UnityWebRequest pour communiquer avec le serveur Firebase via des requêtes HTTP (PUT pour sauvegarder, GET pour charger, DELETE pour supprimer)."

Ça couvre tout ce qu'on te demande ! 💪


