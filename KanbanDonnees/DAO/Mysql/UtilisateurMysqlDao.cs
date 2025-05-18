using MySql.Data.MySqlClient;
using KanbanDonnees.Entities;
using KanbanDonnees.DAO.Interfaces;

namespace KanbanDonnees.DAO.Mysql;

public class UtilisateurMysqlDao : MysqlBaseDao, IUtilisateurDao
{
    public UtilisateurMysqlDao(string chaineDeConnexion) : base(chaineDeConnexion)
    {
    }

    public List<Utilisateur> SelectAll()
    {
        List<Utilisateur> utilisateurs = new List<Utilisateur>();
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM utilisateur";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Prepare();
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                utilisateurs.Add(BuildEntity(reader));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }

        return utilisateurs;
    }

    public List<Utilisateur> SelectAllByCarteId(int id)
    {
        List<Utilisateur> utilisateurs = new List<Utilisateur>();
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = 
                "SELECT * FROM utilisateur u " +
                "INNER JOIN carte_utilisateur c ON c.utilisateur_id = u.id " +
                "WHERE c.carte_id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("id", id);
            commande.Prepare();
            using MySqlDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                utilisateurs.Add(BuildEntity(reader));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }

        return utilisateurs;
    }

    private Utilisateur BuildEntity(MySqlDataReader reader)
    {
        return new Utilisateur(
                reader.GetInt32("id"),
                reader.GetString("nom_complet")
            );
    }
}
