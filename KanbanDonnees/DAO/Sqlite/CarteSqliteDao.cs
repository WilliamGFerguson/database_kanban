using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public class CarteSqliteDao : SqliteBaseDao, ICarteDao
{
    private readonly UtilisateurSqliteDao UtilisateurDao;

    public CarteSqliteDao(string nouveauCheminBd, UtilisateurSqliteDao utilisateurDao) : base(nouveauCheminBd)
    {
        UtilisateurDao = utilisateurDao;
    }

    public Carte? Select(int id)
    {
        Carte? carte = null;
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM carte WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();
            if (reader.Read())
            {
                carte = BuildEntity(reader);
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
        return carte;
    }

    public List<Carte> SelectAllByListeId(int listeId)
    {
        List<Carte> cartes = new List<Carte>();
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM carte WHERE liste_id = @liste_id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@liste_id", listeId);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                cartes.Add(BuildEntity(reader));
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
        return cartes;
    }

    public Carte Insert(Carte carte)
    {
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query =
                "INSERT INTO carte (titre, description, echeance, ordre, liste_id) " +
                "VALUES (@titre, @description, @echeance, @ordre, @liste_id)";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@titre", carte.Titre);
            commande.Parameters.AddWithValue("@description", carte.Description);
            commande.Parameters.AddWithValue("@echeance", carte.Echeance);
            commande.Parameters.AddWithValue("@ordre", carte.Ordre);
            commande.Parameters.AddWithValue("@liste_id", carte.ListeId);
            commande.Prepare();

            int rangeesAffectees = commande.ExecuteNonQuery();
            if (rangeesAffectees != 1) throw new Exception($"Impossible d'insérer la carte '{carte.Titre}'");

            carte.Id = LastInsertedId(connection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return carte;
    }
    public Carte Update(Carte carte)
    {
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            UpdateCarte(carte, connection);
            UpdateCarteUser(carte, connection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return carte;
    }

    public bool Delete(int id)
    {
        int rangeesAffectees = 0;
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "DELETE FROM carte WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);

            commande.Prepare();

            rangeesAffectees = commande.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return rangeesAffectees == 1;
    }

    private Carte BuildEntity(SqliteDataReader reader)
    {
        Carte carte = new Carte(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader["echeance"] == DBNull.Value ? null : reader.GetDateTime(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                UtilisateurDao.SelectAllByCarteId(reader.GetInt32(0))
            );

        return carte;
    }

    private void UpdateCarte(Carte carte, SqliteConnection connection)
    {
        string queryCarte = "UPDATE carte " +
                "SET titre = @titre, description = @description, echeance = @echeance, ordre = @ordre, liste_id = @liste_id " +
                "WHERE id = @id";
        using SqliteCommand cmdCarte = new SqliteCommand(queryCarte, connection);
        cmdCarte.Parameters.AddWithValue("@id", carte.Id);
        cmdCarte.Parameters.AddWithValue("@titre", carte.Titre);
        cmdCarte.Parameters.AddWithValue("@description", carte.Description);
        cmdCarte.Parameters.AddWithValue("@echeance", carte.Echeance);
        cmdCarte.Parameters.AddWithValue("@ordre", carte.Ordre);
        cmdCarte.Parameters.AddWithValue("@liste_id", carte.ListeId);
        cmdCarte.Prepare();

        int rangeesAffectees = cmdCarte.ExecuteNonQuery();
        if (rangeesAffectees != 1) throw new Exception($"Impossible de mettre à jour la carte {carte.Id}.");
    }

    private void UpdateCarteUser(Carte carte, SqliteConnection connection)
    {
        string deleteQuery = "DELETE FROM carte_utilisateur WHERE carte_id = @carteId;";

        using SqliteCommand deleteCommande = new SqliteCommand(deleteQuery, connection);
        deleteCommande.Parameters.AddWithValue("@carteId", carte.Id);
        deleteCommande.Prepare();
        deleteCommande.ExecuteNonQuery();

        foreach (Utilisateur u in carte.Responsables)
        {
            string insertQuery = "INSERT INTO carte_utilisateur VALUES (@userId, @carteId)";
            using SqliteCommand insertCommande = new SqliteCommand(insertQuery, connection);
            insertCommande.Parameters.AddWithValue("@userId", u.Id);
            insertCommande.Parameters.AddWithValue("@carteId", carte.Id);
            insertCommande.Prepare();
            insertCommande.ExecuteNonQuery();
        }
    }
}
