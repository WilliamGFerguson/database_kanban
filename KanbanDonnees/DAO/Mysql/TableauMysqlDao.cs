using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using MySql.Data.MySqlClient;

namespace KanbanDonnees.DAO.Mysql;

public class TableauMysqlDao : MysqlBaseDao, ITableauDao
{
    private ListeMysqlDao ListeDao;

    public TableauMysqlDao(string chaineDeConnexion, ListeMysqlDao listeDao) : base(chaineDeConnexion)
    {
        ListeDao = listeDao;
    }

    public Tableau? Select(int id)
    {
        Tableau? tableau = null;
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM tableau WHERE id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();

            using MySqlDataReader reader = commande.ExecuteReader();

            if (reader.Read())
            {
                tableau = BuildEntity(reader);
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
        return tableau;
    }

    public List<Tableau> SelectAll()
    {
        List<Tableau> tableaux = new List<Tableau>();
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM tableau";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Prepare();

            using MySqlDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                tableaux.Add(BuildEntity(reader));
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
        return tableaux;
    }

    public Tableau Insert(Tableau tableau)
    {
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "INSERT INTO tableau (nom) VALUES (@nom)";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@nom", tableau.Nom);
            commande.Prepare();

            int rowsAffected = commande.ExecuteNonQuery();
            if (rowsAffected != 1) throw new InvalidOperationException($"Impossible d'insérer le tableau '{tableau.Nom}'.");

            tableau.Id = (int)commande.LastInsertedId;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return tableau;
    }

    public Tableau Update(Tableau tableau)
    {
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "UPDATE tableau SET nom = @nom WHERE id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@nom", tableau.Nom);
            commande.Parameters.AddWithValue("@id", tableau.Id);
            commande.Prepare();

            int rowsAffected = commande.ExecuteNonQuery();
            if (rowsAffected != 1) throw new InvalidOperationException($"Impossible de mettre à jour le tableau {tableau.Nom}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return tableau;
    }

    public bool Delete(int id)
    {
        int rangeeAffectee = 0;
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "DELETE FROM tableau WHERE id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();
            rangeeAffectee = commande.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return rangeeAffectee == 1;
    }

    private Tableau BuildEntity(MySqlDataReader reader)
    {
        return new Tableau(
                reader.GetString("nom")
            );
    }
}