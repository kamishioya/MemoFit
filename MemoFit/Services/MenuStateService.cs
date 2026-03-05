namespace MemoFit.Services;

public enum SortOrder
{
    NewestFirst,
    OldestFirst,
    TitleAsc
}

public class MenuStateService
{
    public string SearchTerm { get; private set; } = "";
    public SortOrder SortOrder { get; private set; } = SortOrder.NewestFirst;
    public bool IsMenuOpen { get; private set; } = false;

    public event Action? OnChange;

    public void OpenMenu()
    {
        IsMenuOpen = true;
        OnChange?.Invoke();
    }

    public void CloseMenu()
    {
        IsMenuOpen = false;
        OnChange?.Invoke();
    }

    public void SetSearchTerm(string term)
    {
        SearchTerm = term;
        OnChange?.Invoke();
    }

    public void SetSortOrder(SortOrder order)
    {
        SortOrder = order;
        OnChange?.Invoke();
    }
}
