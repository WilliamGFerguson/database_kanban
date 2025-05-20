using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using MySql.Data.MySqlClient;

namespace KanbanDonnees.DAO.Mysql;

public class CarteMysqlDao : MysqlBaseDao, ICarteDao
{
    private UtilisateurMysqlDao UtilisateurDao;

    public CarteMysqlDao(string chaineDeConnexion, UtilisateurMysqlDao utilisateurDao) : base(chaineDeConnexion)
    {
        UtilisateurDao = utilisateurDao;
    }

    public Carte? Select(int id)
    {
        Carte? carte = null;
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM carte WHERE id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();

            using MySqlDataReader reader = commande.ExecuteReader();
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
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM carte WHERE liste_id = @liste_id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@liste_id", listeId);
            commande.Prepare();

            using MySqlDataReader reader = commande.ExecuteReader();
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
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = 
                "INSERT INTO carte (titre, description, echeance, ordre, liste_id) " +
                "VALUES (@titre, @description, @echeance, @ordre, @liste_id)";
            using MySqlCommand commande = new MySqlCommand(query, connection);
            commande.Parameters.AddWithValue("@titre", carte.Titre);
            commande.Parameters.AddWithValue("@description", carte.Description);
            commande.Parameters.AddWithValue("@echeance", carte.Echeance);
            commande.Parameters.AddWithValue("@ordre", carte.Ordre);
            commande.Parameters.AddWithValue("@liste_id", carte.ListeId);
            commande.Prepare();

            int rangeesAffectees = commande.ExecuteNonQuery();
            if (rangeesAffectees != 1) throw new Exception($"Impossible d'insérer la carte '{carte.Titre}'");

            carte.Id = (int)commande.LastInsertedId;
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
        using MySqlConnection connection = OuvrirConnexion();
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
        using MySqlConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "DELETE FROM carte WHERE id = @id";
            using MySqlCommand commande = new MySqlCommand(query, connection);
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

    private Carte BuildEntity(MySqlDataReader reader)
    {
        Carte carte = new Carte(
                reader.GetInt32("id"),
                reader.GetString("titre"),
                reader.GetString("description"),
                reader["echeance"] == DBNull.Value ? null : reader.GetDateTime("echeance"),
                reader.GetInt32("ordre"),
                reader.GetInt32("liste_id"),
                UtilisateurDao.SelectAllByCarteId(reader.GetInt32("id"))
            );

        return carte;
    }

    private void UpdateCarte(Carte carte, MySqlConnection connection)
    {
        string queryCarte = "UPDATE carte " +
                "SET titre = @titre, description = @description, echeance = @echeance, ordre = @ordre, liste_id = @liste_id " +
                "WHERE id = @id";
            using MySqlCommand cmdCarte = new MySqlCommand(queryCarte, connection);
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

    private void UpdateCarteUser(Carte carte, MySqlConnection connection)
    {
        string deleteQuery = "DELETE FROM carte_utilisateur WHERE carte_id = @carteId;";

        using MySqlCommand deleteCommande = new MySqlCommand(deleteQuery, connection);
        deleteCommande.Parameters.AddWithValue("@carteId", carte.Id);
        deleteCommande.Prepare();
        deleteCommande.ExecuteNonQuery();

        foreach (Utilisateur u in carte.Responsables)
        {
            string insertQuery = "INSERT INTO carte_utilisateur VALUES (@userId, @carteId)";
            using MySqlCommand insertCommande = new MySqlCommand(insertQuery, connection);
            insertCommande.Parameters.AddWithValue("@userId", u.Id);
            insertCommande.Parameters.AddWithValue("@carteId", carte.Id);
            insertCommande.Prepare();
            insertCommande.ExecuteNonQuery();
        }
    }
}