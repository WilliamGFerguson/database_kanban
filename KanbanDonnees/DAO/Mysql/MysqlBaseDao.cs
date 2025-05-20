using MySql.Data.MySqlClient;

namespace KanbanDonnees.DAO.Mysql;
public abstract class MysqlBaseDao
{
    private readonly string chaineDeConnexion;

    protected MysqlBaseDao(string nouvelleChaineDeConnexion)
    {
        chaineDeConnexion = nouvelleChaineDeConnexion;
    }

    protected MySqlConnection OuvrirConnexion()
    {
        return new MySqlConnection(chaineDeConnexion);
    }
}