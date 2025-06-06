﻿using KanbanDonnees.DAO.Interfaces;
using KanbanDonnees.Entities;
using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public class ListeSqliteDao : SqliteBaseDao, IListeDao
{
    private readonly CarteSqliteDao CarteDao;

    public ListeSqliteDao(string nouveauCheminBd, CarteSqliteDao CarteSqliteDao) : base(nouveauCheminBd)
    {
        CarteDao = CarteSqliteDao;
    }

    public Liste? Select(int id)
    {
        Liste? liste = null;
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM liste WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", id);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();
            if (reader.Read())
            {
                liste = BuildEntity(reader);
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
        return liste;
    }

    public List<Liste> SelectAllByTableauId(int tableauId)
    {
        List<Liste> listes = new List<Liste>();
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "SELECT * FROM liste WHERE tableau_id = @tableau_id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@tableau_id", tableauId);
            commande.Prepare();

            using SqliteDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                listes.Add(BuildEntity(reader));
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
        return listes;
    }

    public Liste Insert(Liste liste)
    {
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query =
                "INSERT INTO liste (nom, ordre, tableau_id) " +
                "VALUES (@nom, @ordre, @tableau_id)";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@nom", liste.Nom);
            commande.Parameters.AddWithValue("@ordre", liste.Ordre);
            commande.Parameters.AddWithValue("@tableau_id", liste.TableauId);
            commande.Prepare();

            int rowsAffected = commande.ExecuteNonQuery();
            if (rowsAffected != 1) throw new InvalidOperationException($"Impossible d'insérer la liste '{liste.Nom}'.");

            liste.Id = LastInsertedId(connection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return liste;
    }

    public Liste Update(Liste liste)
    {
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "UPDATE liste " +
                "SET nom = @nom, ordre = @ordre, tableau_id = @tableau_id " +
                "WHERE id = @id";
            using SqliteCommand commande = new SqliteCommand(query, connection);
            commande.Parameters.AddWithValue("@id", liste.Id);
            commande.Parameters.AddWithValue("@nom", liste.Nom);
            commande.Parameters.AddWithValue("@ordre", liste.Ordre);
            commande.Parameters.AddWithValue("@tableau_id", liste.TableauId);
            commande.Prepare();

            int rowsAffected = commande.ExecuteNonQuery();
            if (rowsAffected != 1) throw new InvalidOperationException($"Impossible de mettre à jour la liste {liste.Nom}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return liste;
    }

    public bool Delete(int id)
    {
        int rangeeAffectee = 0;
        using SqliteConnection connection = OuvrirConnexion();
        try
        {
            connection.Open();
            string query = "DELETE FROM liste WHERE id = @id";
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

    private Liste BuildEntity(SqliteDataReader reader)
    {
        return new Liste(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                CarteDao.SelectAllByListeId(reader.GetInt32(0))
            );
    }
}
