# EMIT — Backend ASP.NET Core MVC + PostgreSQL + Authentification

## ⚠️ À FAIRE AVANT D'INSÉRER LE MOINDRE CODE

### 1. Installer les outils nécessaires
1. **Visual Studio 2022** (édition Community suffit) avec la charge de travail
   **"Développement web ASP.NET et web"** cochée (Visual Studio Installer).
2. **.NET SDK 8.0** — vérifiable avec :
   ```
   dotnet --version
   ```
3. **PostgreSQL** installé et lancé. Notez le mot de passe du user `postgres`.
4. (Optionnel) **pgAdmin** pour visualiser la base de données.

### 2. Créer le projet dans Visual Studio
1. `Fichier > Nouveau > Projet`
2. **"Application Web ASP.NET Core (Modèle-Vue-Contrôleur)"** — PAS Vide, PAS API.
3. Nommez-le **EMIT** (les fichiers utilisent `namespace EMIT`).
4. Framework cible : **.NET 8.0**.
5. Type d'authentification lors de la création du projet : **Aucun**
   (on utilise notre propre système ci-dessous, pas Identity complet).
6. Créer.

### 3. Installer les packages NuGet (avant d'ajouter le code)
Console du Gestionnaire de package NuGet :
```powershell
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.Tools
```
*(`PasswordHasher` utilisé pour le login/inscription fait partie du framework partagé ASP.NET Core — aucun package supplémentaire n'est nécessaire.)*

### 4. Créer la base de données PostgreSQL
```sql
CREATE DATABASE emit_db;
```

---

## 📂 Où placer les fichiers fournis

```
EMIT/
├── Models/
│   ├── RoleUtilisateur.cs
│   ├── Utilisateur.cs        (contient désormais IdClasse nullable)
│   ├── Enseignant.cs
│   ├── Niveau.cs              (NOUVEAU)
│   ├── Classe.cs              (mis à jour : lien vers Niveau)
│   ├── Matiere.cs
│   ├── Salle.cs
│   └── Cours.cs
├── Data/
│   └── ApplicationDbContext.cs   (mis à jour)
├── Services/
│   └── CoursValidationService.cs
├── ViewModels/                   (NOUVEAU)
│   ├── RegisterViewModel.cs
│   └── LoginViewModel.cs
├── Controllers/
│   ├── AccountController.cs   (NOUVEAU : login/inscription/déconnexion)
│   ├── CoursController.cs
│   ├── SallesController.cs
│   └── NiveauxController.cs   (NOUVEAU)
├── Views/
│   ├── Account/                (NOUVEAU)
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── AccesRefuse.cshtml
│   └── Cours/
│       ├── Index.cshtml
│       └── Create.cshtml
├── Program.cs         (remplace celui généré par défaut — active l'authentification)
└── appsettings.json    (remplace celui généré par défaut)
```

⚠️ **Si vous aviez déjà copié les fichiers d'une réponse précédente** : supprimez
l'ancien fichier `Models/Etudiant.cs` s'il existe (il a été fusionné dans `Utilisateur.cs`).

⚠️ **Modifiez `appsettings.json`** avec votre vrai mot de passe PostgreSQL.

---

## 🔧 Après avoir inséré tous les fichiers

### 5. Ajouter le lien de connexion dans le menu (facultatif mais utile)
Dans `Views/Shared/_Layout.cshtml`, ajoutez dans la barre de navigation :
```html
<li class="nav-item">
    <a class="nav-link" asp-controller="Account" asp-action="Login">Connexion</a>
</li>
<li class="nav-item">
    <a class="nav-link" asp-controller="Account" asp-action="Register">Inscription</a>
</li>
```

### 6. Scaffolder les vues manquantes (Classes, Matières)
Clic droit sur `Controllers` > `Ajouter` > `Contrôleur` >
**"Contrôleur MVC avec vues, utilisant Entity Framework"** > choisir le modèle
(`Classe`, `Matiere`) et `ApplicationDbContext`. Répétez pour chacun.

### 7. Créer et appliquer les migrations
```powershell
Add-Migration InitialCreate
Update-Database
```

### 8. Créer au moins un Niveau et une Classe avant de tester l'inscription "Étudiant"
Lancez l'app (F5), allez sur `/Niveaux/Create` puis `/Classes/Create`
(une fois scaffoldé) pour avoir des données de test.

### 9. Tester l'inscription et la connexion
- `/Account/Register` : créez un compte (Administrateur, Enseignant ou Étudiant).
  Le formulaire affiche dynamiquement le champ "Classe" si Étudiant, ou
  "Plafond d'heures" si Enseignant.
- `/Account/Login` : connectez-vous avec l'email/mot de passe créés.

---

## ✅ Ce qui est implémenté

| Fonctionnalité | Détails | Statut |
|---|---|---|
| RG01-RG04 | Inscription avec héritage exclusif, email unique, étudiant lié à une classe | ✔️ |
| RG05 | Classe reliée obligatoirement à un Niveau | ✔️ |
| RG09, RG11-RG14 | Règles métier des cours (`CoursValidationService`) | ✔️ |
| Mots de passe | Hachés avec `PasswordHasher<Utilisateur>` (jamais stockés en clair) | ✔️ |
| Connexion | Cookie d'authentification avec claim `Role` | ✔️ |
| RG02 | Seul l'Administrateur peut créer/modifier/supprimer Cours, Salles, Niveaux, Comptes | ✔️ |
| RG02 | L'inscription publique en Administrateur est bloquée ; un admin est créé automatiquement au démarrage | ✔️ |
| RG03 | Enseignant/Étudiant : accès en lecture seule à LEUR planning uniquement | ✔️ |
| Gestion des comptes | `UtilisateursController` (lister, créer un admin, modifier, supprimer) | ✔️ |

## 🔑 Connexion administrateur par défaut
Au premier lancement, un compte Administrateur est créé automatiquement avec les
identifiants définis dans `appsettings.json` (section `AdminSeed`) :
- Email : `admin@emit.local`
- Mot de passe : `ChangeMoi123!`

**⚠️ Changez ce mot de passe immédiatement en production**, ou modifiez les valeurs
dans `appsettings.json` avant le tout premier lancement.

## ⚠️ Ce qui reste à faire vous-même
- **Scaffolder Classes et Matières** (voir étape 6) : pensez à ajouter manuellement
  `[Authorize(Roles = "Administrateur")]` sur ces contrôleurs générés, comme fait
  pour `SallesController`/`NiveauxController`, sinon ils resteront accessibles à tous.
- **Page "Mon compte"** pour qu'un Enseignant/Étudiant modifie son propre mot de
  passe — pas encore implémentée. Dites-moi si vous la voulez.
- **Menu de navigation adapté au rôle** : dans `Views/Shared/_Layout.cshtml`, vous
  pouvez conditionner l'affichage des liens avec `User.IsInRole("Administrateur")`,
  par exemple :
  ```html
  @if (User.Identity?.IsAuthenticated == true)
  {
      <li class="nav-item"><span class="nav-link">Bonjour, @User.Identity.Name</span></li>
      @if (User.IsInRole("Administrateur"))
      {
          <li class="nav-item"><a class="nav-link" asp-controller="Utilisateurs" asp-action="Index">Comptes</a></li>
      }
      <li class="nav-item">
          <form asp-controller="Account" asp-action="Logout" method="post" class="form-inline">
              <button type="submit" class="nav-link btn btn-link">Déconnexion</button>
          </form>
      </li>
  }
  else
  {
      <li class="nav-item"><a class="nav-link" asp-controller="Account" asp-action="Login">Connexion</a></li>
  }
  ```
