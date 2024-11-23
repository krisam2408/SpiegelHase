namespace SpiegelHase.DataTransfer;

public sealed class ModelDictionary
{
    private readonly List<ModelEntry> m_entries = [];
    public List<ModelEntry> Entries => m_entries;

    public bool IsValid
    {
        get
        {
            foreach (ModelEntry entry in Entries)
            {
                if (!entry.IsValid)
                    return false;
            }
            return true;
        }
    }

    internal void AddEntry(ModelEntry entry)
    {
        if(!Entries.Contains(entry))
            Entries.Add(entry);
    }

    public void RemoveEntry(string key)
    {
        int? index = null;
        int len = Entries.Count;

        for(int i = 0; i < len; i++)
        {
            if(Entries[i].Key == key)
            {
                index = i;
                break;
            }
        }

        if (index is not null)
            Entries.RemoveAt(index.Value);
        
    }

    public IEnumerator<ModelEntry> GetEnumerator()
    {
        return Entries.GetEnumerator();
    }
}
