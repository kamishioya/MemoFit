// Services/DatabaseService.cs
using SQLite;
using MemoFit.Models;

namespace MemoFit.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;

    private async Task Init()
    {
        if (_db != null) return;

        // プラットフォームごとに適切なパスを取得
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "memofit.db3"
        );

        _db = new SQLiteAsyncConnection(dbPath);
        await _db.CreateTableAsync<Memo>();
    }

    public async Task<List<Memo>> GetMemosAsync()
    {
        await Init();
        return await _db!.Table<Memo>().ToListAsync();
    }

    public async Task<Memo?> GetMemoByIdAsync(int id)
    {
        await Init();
        return await _db!.FindAsync<Memo>(id);
    }

    public async Task<int> SaveMemoAsync(Memo memo)
    {
        await Init();
        return memo.Id != 0
            ? await _db!.UpdateAsync(memo)
            : await _db!.InsertAsync(memo);
    }

    public async Task<int> DeleteMemoAsync(Memo memo)
    {
        await Init();
        return await _db!.DeleteAsync(memo);
    }
}