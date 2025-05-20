using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public class UtilisateurSqliteDao : SqliteBaseDao, IUtilisateurDao
{
    public UtilisateurSqliteDao(string nouveauCheminBd) : base(nouveauCheminBd)
    {
    }

    public List<Utilisateur> SelectAll()
    {
        List<Utilisateur> utilisateurs = new List<Utilisateur>();
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM utilisateur";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Prepare();
            using SqliteDataReader reader = command.ExecuteReader();
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
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query =
                "SELECT * FROM utilisateur u " +
                "INNER JOIN carte_utilisateur c ON c.utilisateur_id = u.id " +
                "WHERE c.carte_id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();
            using SqliteDataReader reader = commande.ExecuteReader();
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

    private Utilisateur BuildEntity(SqliteDataReader reader)
    {
        return new Utilisateur(
                reader.GetInt32(0),
                reader.GetString(1)
            );
    }
}
