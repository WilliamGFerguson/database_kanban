namespace KanbanDonnees.Entities;

public class Tableau
{
    private int _id;
    private string _nom;
    private List<Liste> _listes;

    public Tableau(int id, string nom, List<Liste>? listes = null)
    {
        Id = id;
        Nom = nom;
        Listes = listes ?? new List<Liste>();
    }

    public Tableau(string nom) : this(0, nom)
    {
    }

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string Nom
    {
        get => _nom;
        set => _nom = value ?? throw new ArgumentNullException(nameof(value));
    }

    public List<Liste> Listes
    {
        get => _listes;
        set => _listes = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public override string ToString()
    {
        string listes = "";
        foreach (Liste liste in Listes)
        {
            listes += liste.ToString();
        }
        return $"ID: {Id}\nNom: {Nom}\nListes:\n{listes}";
    }
}