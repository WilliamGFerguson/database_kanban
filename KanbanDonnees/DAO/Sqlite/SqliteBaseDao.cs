using Microsoft.Data.Sqlite;

namespace KanbanDonnees.DAO.Sqlite;

public abstract class SqliteBaseDao
{
    private readonly string cheminBd;

    protected SqliteBaseDao(string nouveauCheminBd)
    {
        cheminBd = nouveauCheminBd;
    }

    protected SqliteConnection OuvrirConnexion()
    {
        return new SqliteConnection(cheminBd);
    }

    protected int LastInsertedId(SqliteConnection connexion)
    {
        string query = "SELECT last_insert_rowid()";
        using SqliteCommand commande = new SqliteCommand(query, connexion);
        return Convert.ToInt32(commande.ExecuteScalar());
    }
}
