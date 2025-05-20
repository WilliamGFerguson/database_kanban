using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public class TableauSqliteDao : SqliteBaseDao, ITableauDao
{
    private readonly ListeSqliteDao ListeDao;

    public TableauSqliteDao(string nouveauCheminBd, ListeSqliteDao listeSqliteDao) : base(nouveauCheminBd)
    {
        ListeDao = listeSqliteDao;
    }

    public Tableau? Select(int id)
    {
        Tableau? tableau = null;
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM tableau WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();

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
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM tableau";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();
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
        using SqliteConnection connection = OuvrirConnexion();
        using SqliteTransaction transaction = connection.BeginTransaction();
        try
        {
            connection.Open();
            string query = "INSERT INTO tableau (nom) VALUES (@nom)";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@nom", tableau.Nom);
            commande.Prepare();

            int rowsAffected = commande.ExecuteNonQuery();
            if (rowsAffected != 1) throw new InvalidOperationException($"Impossible d'insérer le tableau '{tableau.Nom}'.");

            tableau.Id = LastInsertedId(connection);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            transaction.Rollback();
        }
        finally
        {
            connection.Close();
        }
        return tableau;
    }

    public Tableau Update(Tableau tableau)
    {
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "UPDATE tableau SET nom = @nom WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
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
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "DELETE FROM tableau WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
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

    private Tableau BuildEntity(SqliteDataReader reader)
    {
        return new Tableau(
                reader.GetInt32(0),
                reader.GetString(1),
                ListeDao.SelectAllByTableauId(reader.GetInt32(0))
            );
    }
}
