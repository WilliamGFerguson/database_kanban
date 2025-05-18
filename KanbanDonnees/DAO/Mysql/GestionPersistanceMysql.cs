using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using MySql.Data.MySqlClient;

namespace KanbanDonnees.DAO.Mysql;

public class GestionPersistanceMysql : IGestionPersistance
{
    private readonly string ChaineDeConnexion;

    public IUtilisateurDao UtilisateurDao { get; set; }
    public ICarteDao CarteDao { get; set; }
    public IListeDao ListeDao { get; set; }
    public ITableauDao TableauDao { get; set; }

    public GestionPersistanceMysql(string chaineDeConnexion)
    {
        ChaineDeConnexion = chaineDeConnexion;

        UtilisateurDao = new UtilisateurMysqlDao(ChaineDeConnexion);
        CarteDao = new CarteMysqlDao(ChaineDeConnexion, (UtilisateurMysqlDao)UtilisateurDao);
        ListeDao = new ListeMysqlDao(ChaineDeConnexion, (CarteMysqlDao)CarteDao);
        TableauDao = new TableauMysqlDao(ChaineDeConnexion, (ListeMysqlDao)ListeDao);
    }

    public void CreerPersistanceEtInsererDonnees()
    {
        string basePath = AppContext.BaseDirectory;
        string rootProject = Directory.GetParent(basePath).Parent.Parent.Parent.Parent.FullName;
        string seedPath = Path.Combine(rootProject, "KanbanDonnees", "Resources", "bd-mysql.sql");

        if (!File.Exists(seedPath))
        {
            Console.WriteLine($"\nFichier seed non trouvé");
            return;
        }

        string parametresConnexion = SeparerParametresConnexion();
        using MySqlConnection connection = new MySqlConnection(parametresConnexion);
        try
        {
            string seed = File.ReadAllText(seedPath);
            connection.Open();
            using MySqlCommand commande = new MySqlCommand(seed, connection);
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

    public string SeparerParametresConnexion()
    {
        var parametres = ChaineDeConnexion.Split(";", StringSplitOptions.RemoveEmptyEntries);
        var paramSepares = new List<string>();

        foreach (var param in parametres)
        {
            if (!param.StartsWith("database=", StringComparison.OrdinalIgnoreCase))
            {
                paramSepares.Add(param);
            }
        }

        return string.Join(";", paramSepares);
    }
}