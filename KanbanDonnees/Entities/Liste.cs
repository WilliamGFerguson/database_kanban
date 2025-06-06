namespace KanbanDonnees.Entities;

public class Liste
{
    private int _id;
    private string _nom;
    private int _ordre;
    private int _tableauId;
    private List<Carte> _cartes;

    // Constructeur par défaut nécessaire pour le mapper
    public Liste()
    {
    }

    public Liste(int id, string nom, int ordre, int tableauId, List<Carte>? cartes = null)
    {
        Id = id;
        Nom = nom;
        Ordre = ordre;
        TableauId = tableauId;
        Cartes = cartes ?? new List<Carte>();
    }

    public Liste(string nom, int ordre, int tableauId) : this(0, nom, ordre, tableauId)
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
    
    public List<Carte> Cartes
    {
        get => _cartes;
        set => _cartes = value ?? throw new ArgumentNullException(nameof(value));
    }

    public int Ordre
    {
        get => _ordre;
        set => _ordre = value;
    }

    public int TableauId
    {
        get => _tableauId;
        set => _tableauId = value;
    }

    public override string ToString()
    {
        return $"{Id} - {Nom} - Ordre: {Ordre} - TableauId: {TableauId}\n";
    }
}