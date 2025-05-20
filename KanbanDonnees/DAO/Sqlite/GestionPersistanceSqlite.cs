using KanbanDonnees.DAO.Interfaces;
using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public class GestionPersistanceSqlite : IGestionPersistance
{
    private readonly string CheminBd;

    public IUtilisateurDao UtilisateurDao { get; set; }
    public ICarteDao CarteDao { get; set; }
    public IListeDao ListeDao { get; set; }
    public ITableauDao TableauDao { get; set; }

    public GestionPersistanceSqlite(string nouveauCheminBd)
    {
        CheminBd = nouveauCheminBd;

        UtilisateurDao = new UtilisateurSqliteDao(CheminBd);
        CarteDao = new CarteSqliteDao(CheminBd, (UtilisateurSqliteDao)UtilisateurDao);
        ListeDao = new ListeSqliteDao(CheminBd, (CarteSqliteDao)CarteDao);
        TableauDao = new TableauSqliteDao(CheminBd, (ListeSqliteDao)ListeDao);
    }

    public void CreerPersistanceEtInsererDonnees()
    {
        string basePath = AppContext.BaseDirectory;
        string rootProject = Directory.GetParent(basePath).Parent.Parent.Parent.Parent.FullName;
        string seedPath = Path.Combine(rootProject, "KanbanDonnees", "Resources", "bd-sqlite.sql");

        if (!File.Exists(seedPath))
        {
            Console.WriteLine($"\nFichier seed non trouvé");
            return;
        }

        using SqliteConnection connection = new SqliteConnection(CheminBd);
        try
        {
            string seed = File.ReadAllText(seedPath);
            connection.Open();
            using SqliteCommand commande = new SqliteCommand(seed, connection);
            commande.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}
